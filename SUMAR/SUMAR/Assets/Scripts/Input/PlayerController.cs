using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

//enum para gestionar los tipos de inputs digitales (botones encendido-apagado)
public enum INPUTACTIONS { ATTACK, CATCH, THROW, DASH, TAUNT, PAUSE };

public class PlayerController : MonoBehaviour
{
    public PlayerActions playerInput;

    [Header("Movement")]
    [SerializeField] float movementSpeed;

	[Header("Dash")]
	[SerializeField] float dashSpeed;
	[SerializeField] float dashTime;
	[SerializeField] float dashCooldownTime;

    [Header("Taunt")]
    [SerializeField] float tauntTime;
    private SpriteRenderer spriteRenderer; //CAMBIARPORANIMACION

    [Header("Taunt")]
    [SerializeField] float stunTime;

    [Header("Animations")]
    [SerializeField] Animator animator;
    [SerializeField] AnimatorController[] playerControllers;

    [Header("Juggle")]
    [SerializeField] int maxJuggleAmmo = 5;
    private int juggleAmmo;
    private List<Juggle> playerJuggles = new List<Juggle>();
    [SerializeField] JuggleArea juggleArea;
    [SerializeField] Juggle jugglePrefab;
    [SerializeField] PointsManager pointsManager;
    private JugglePickupArea __targetPickupArea = null; //JANKY AF ESPERO QUE NO DE PROBLEMAS

    private Vector3 axisvalue = new Vector3();
    private Queue<INPUTACTIONS> inputQueue = new Queue<INPUTACTIONS>();

    [SerializeField]
    public int playerID = 1;
    private int? uniqueID = null;
    private bool __lookingRight = true;

    bool m_isInDash = false;
    float m_initialDashTime;
    float m_endDashTime;
    Vector2 m_dashDirection;

    bool m_isInTaunt = false;
    float m_initialTauntTime;

    bool m_isStunned = false;
    float m_initialStunTime;

    private void Awake()
    {
		axisvalue = new Vector3();

		playerInput = new PlayerActions();
        // por cada input que est� en el enum hacer esto
        playerInput.PlayerActionMap.Attack.performed += ctx => EnqueueActionInput(ctx, INPUTACTIONS.ATTACK);
        playerInput.PlayerActionMap.CatchBall.performed += ctx => EnqueueActionInput(ctx, INPUTACTIONS.CATCH);
        playerInput.PlayerActionMap.ThrowBall.performed += ctx => EnqueueActionInput(ctx, INPUTACTIONS.THROW);
        playerInput.PlayerActionMap.Dash.performed += ctx => EnqueueActionInput(ctx, INPUTACTIONS.DASH);
        playerInput.PlayerActionMap.Taunt.performed += ctx => EnqueueActionInput(ctx, INPUTACTIONS.TAUNT);
        playerInput.PlayerActionMap.Pause.performed += ctx => EnqueueActionInput(ctx, INPUTACTIONS.PAUSE);

        spriteRenderer = GetComponent<SpriteRenderer>();
        __lookingRight = true;

        pointsManager = GameObject.FindWithTag("PointsManager").GetComponent<PointsManager>();

        playerID = GameManager.Instance.getPlayerId();
        animator.runtimeAnimatorController = playerControllers[playerID];

		Vector3 spawnPoint = MapBorders.Instance.GetSpawnPoint(playerID).position;
		transform.position = new Vector3(spawnPoint.x, transform.position.y, spawnPoint.z);
    }

    private void Start()
    {
        juggleAmmo = maxJuggleAmmo;

        for (int i = 0; i < maxJuggleAmmo; i++)
        {
            Juggle instantiatedJuggle = Instantiate(jugglePrefab, Vector3.zero, Quaternion.identity);
            instantiatedJuggle.SetPlayer(this);
            instantiatedJuggle.setPointsManager(pointsManager);
            playerJuggles.Add(instantiatedJuggle);
        }
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }
    private void OnDisable()
    {
        playerInput.Disable();
    }


    private void Update()
    {       

        INPUTACTIONS catchedInput;
        while (inputQueue.TryDequeue(out catchedInput))
        {
            //Debug.Log(catchedInput.ToString());
            switch (catchedInput)
            {
                case INPUTACTIONS.ATTACK:

                    Debug.Log(catchedInput.ToString());

                    Juggle? juggleForAttack = GetAvailableJuggle();

                    if(juggleForAttack == null) break;

                    animator.SetTrigger("attack");
					AudioManager.instance.Play("goofyass3");
                    Vector3 shootingDirection = new Vector3(Vector3.Normalize(axisvalue).x, 0, Vector3.Normalize(axisvalue).y);
                    if (shootingDirection == Vector3.zero) 
                    {
                        if (__lookingRight == true) shootingDirection = new Vector3(1, 0, 0);
                        else shootingDirection = new Vector3(-1, 0, 0);
                    }
                    
                    juggleForAttack.Shoot(transform.position, shootingDirection);

					break;
                case INPUTACTIONS.CATCH:
                    if (__targetPickupArea == null) break;

                    bool pickedUpJuggle = __targetPickupArea.TryPickup(playerID);
                    if(pickedUpJuggle) {
                        juggleAmmo++;
                        pointsManager.catchBall(playerID);
                    }
                    break;
                case INPUTACTIONS.THROW:

                    Juggle? juggleToThrow = GetAvailableJuggle();

                    if(juggleToThrow == null) break;

                    Vector3 juggleTargetPosition = GetJugglePosition();
                    juggleToThrow.setTargetPosition(juggleTargetPosition, this.transform.position, false);

                    pointsManager.throwBall(playerID);
					animator.SetTrigger("hurl");
					AudioManager.instance.Play("hurl");

					break;
                case INPUTACTIONS.DASH:
                    OnDash();
					break;
                case INPUTACTIONS.TAUNT:
                    OnTaunt();
                    break;
                case INPUTACTIONS.PAUSE:
                    Debug.Log(catchedInput.ToString());
					//animator.SetTrigger("hit"); //TODO: provisional para hacer test
					//AudioManager.instance.Play("goofyass4");

                    MenuPausa.Instance.ToggleMenu();
					break;
                default:
                    break;
            }
        }

		if (Time.timeScale == 0.0f)
		{
			return;
		}


        if (m_isStunned){

            HandleStun();

        }else if (m_isInTaunt)
        {
            HandleTaunt();

        }else if(m_isInDash)
        {
            HandleDash();
        }
        else 
        { 
            HandleMovement();
        }
	}

    public Vector3 GetJugglePosition() //This little maneuver is going to cost us 100000 years.
    {
        Vector3 positionCandidate = juggleArea.SelectPoint() + this.transform.position;
        
        if(MapBorders.Instance.CheckPositionInBorders(positionCandidate) == true)
        {
            return positionCandidate; // A veces una chica
        }
        return GetJugglePosition();
    }

    private Juggle? GetAvailableJuggle()
    {
        for(int k = 0; k < maxJuggleAmmo; k++)
        {
            Juggle juggle = playerJuggles[k];
            if(juggle.state == JUGGLESTATE.AVAILABLE) return juggle;
        }

        return null;
    }

    public void EnqueueActionInput(InputAction.CallbackContext ctx, INPUTACTIONS input)
    {
        if (ctx.control.device.deviceId == uniqueID)
        {
            if (ctx.performed)
                inputQueue.Enqueue(input);
        }

    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        if (uniqueID == null) { uniqueID = ctx.control.device.deviceId; }
        if (ctx.performed)
        {
            axisvalue = ctx.ReadValue<Vector3>();
        }
    }

    public void OnDash()
    {
        if (isRunning && !m_isInDash && Time.time > m_endDashTime + dashCooldownTime && !m_isInTaunt)
        {
            m_isInDash = true;
			animator.SetBool("isInDash", true);
			m_initialDashTime = Time.time;
            m_dashDirection = axisvalue;
			AudioManager.instance.Play("dash");
            GameManager.Instance.cameraEffects.shakeDuration = 2;
		}
	}
    public void OnTaunt()
    {
        if (!m_isInDash)
        {
            m_isInTaunt = true;
			animator.SetBool("isInTaunt", true);
			m_initialTauntTime = Time.time;
            pointsManager.taunt(playerID);
			AudioManager.instance.Play("goofyass2");
		}
    }

    bool isRunning = false;
    private void HandleMovement()
	{
        if(axisvalue == Vector3.zero)
        {
            isRunning = false;
			animator.SetBool("isRunning", isRunning);
			return;
		}

        isRunning = true;
		animator.SetBool("isRunning", isRunning);

		Vector3 finalPosition = this.transform.position + (new Vector3(axisvalue.x, 0, axisvalue.y)).normalized * movementSpeed * Time.deltaTime;

		this.transform.position =  MapBorders.Instance.ClampVectorToArea(finalPosition);

        __lookingRight = (axisvalue.x > 0);

        if((axisvalue.x > 0 && transform.localScale.x > 0) || (axisvalue.x < 0 && transform.localScale.x < 0))
        {
            Vector3 scale = transform.localScale;
			scale.x *= -1;
            transform.localScale = scale;
		}
	}

    private void HandleDash()
    {
        if (m_initialDashTime + dashTime < Time.time)
        {
            m_isInDash = false;
			m_endDashTime = Time.time;

			animator.SetBool("isInDash", false);
			return;
		}

		Vector3 finalPosition = this.transform.position + (new Vector3(m_dashDirection.x, 0, m_dashDirection.y)).normalized * dashSpeed * Time.deltaTime;

		this.transform.position = MapBorders.Instance.ClampVectorToArea(finalPosition);
	}

    private void OnTriggerEnter(Collider other)
    {
        bool isJuggle = other.TryGetComponent(out JuggleProjectile juggleProjectile);

        if (isJuggle)
        {
            bool hasBeenShootWithJuggle = juggleProjectile.TryReceiveShot(playerID);

            if (hasBeenShootWithJuggle)
            {
                m_isStunned = true;
                m_initialStunTime = Time.time;
                animator.SetTrigger("hit");
                AudioManager.instance.Play("goofyass4");
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {
        bool isPickupArea = other.TryGetComponent(out JugglePickupArea pickup);
        bool isJuggle = other.TryGetComponent(out JuggleProjectile juggleProjectile);

        if (isPickupArea) // I know
        {
            __targetPickupArea = pickup;
        }
        else if (isJuggle)
        {
            juggleProjectile.TryPickUpFromFloor(playerID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        bool isPickupArea = other.TryGetComponent(out JugglePickupArea pickup);

        if (isPickupArea && pickup == __targetPickupArea) // Poetry
        {
            __targetPickupArea = null;
        }
    }

    private void HandleTaunt()
    {
        if (m_initialTauntTime + tauntTime < Time.time)
        {
            m_isInTaunt = false;
			animator.SetBool("isInTaunt", false);
			return;
        }
    }

    private void HandleStun()
    {
        if (m_initialStunTime + stunTime < Time.time)
        {
            m_isStunned = false;
            return;
        }
    }
}
