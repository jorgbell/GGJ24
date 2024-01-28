using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
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

    [Header("Animations")]
    [SerializeField] Animator animator;

    [Header("Juggle")]
    [SerializeField] int maxJuggleAmmo = 5;
    private int juggleAmmo;
    private List<Juggle> playerJuggles = new List<Juggle>();
    [SerializeField] JuggleArea juggleArea;
    [SerializeField] Juggle jugglePrefab;
    private JugglePickupArea __targetPickupArea = null; //JANKY AF ESPERO QUE NO DE PROBLEMAS

    private Vector3 axisvalue = new Vector3();
    private Queue<INPUTACTIONS> inputQueue = new Queue<INPUTACTIONS>();

    [SerializeField]
    private int playerID = 1;

    bool m_isInDash = false;
    float m_initialDashTime;
    float m_endDashTime;
    Vector2 m_dashDirection;

    bool m_isInTaunt = false;
    float m_initialTauntTime;

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
    }

    private void Start()
    {
        transform.position = MapBorders.Instance.GetRandomPositionInArea(transform.position.y);
        juggleAmmo = maxJuggleAmmo;

        for (int i = 0; i < maxJuggleAmmo; i++)
        {
            Juggle instantiatedJuggle = Instantiate(jugglePrefab, Vector3.zero, Quaternion.identity);
            instantiatedJuggle.SetPlayer(playerID);
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
            Debug.Log("reading input queue...");
            switch (catchedInput)
            {
                case INPUTACTIONS.ATTACK:
                    Debug.Log(catchedInput.ToString());
                    break;
                case INPUTACTIONS.CATCH:
                    if (__targetPickupArea == null) break;

                    bool pickedUpJuggle = __targetPickupArea.TryPickup(playerID);
                    if(pickedUpJuggle) {
                        juggleAmmo++;
                        // Otorgar puntos imaginarios aquí
                    }
                    break;
                case INPUTACTIONS.THROW:
                    Juggle? juggleToThrow = GetAvailableJuggle();

                    if(juggleToThrow == null) break;

                    Vector3 juggleTargetPosition = GetJugglePosition();
                    juggleToThrow.setTargetPosition(juggleTargetPosition, this.transform.position, false);
                    break;
                case INPUTACTIONS.DASH:
                    Debug.Log(catchedInput.ToString());
                    OnDash();
					break;
                case INPUTACTIONS.TAUNT:
                    OnTaunt();
                    Debug.Log(catchedInput.ToString());
                    break;
                case INPUTACTIONS.PAUSE:
                    Debug.Log(catchedInput.ToString());
                    break;
                default:
                    break;
            }
        }

        if (m_isInTaunt)
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

    private Vector3 GetJugglePosition() //This little maneuver is going to cost us 100000 years.
    {
        Vector3 positionCandidate = juggleArea.SelectPoint();
        if(MapBorders.Instance.CheckPositionInBorders(positionCandidate) == true)
        {
            return positionCandidate + this.transform.position; // A veces una chica
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
        //if (ctx.performed)
            inputQueue.Enqueue(input);
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            axisvalue = ctx.ReadValue<Vector3>();
        }
    }

    public void OnDash()
    {
        if (!m_isInDash && Time.time > m_endDashTime + dashCooldownTime && !m_isInTaunt)
        {
            m_isInDash = true;
			animator.SetBool("isInDash", true);
			m_initialDashTime = Time.time;
            m_dashDirection = axisvalue;
        }
	}
    public void OnTaunt()
    {
        if (!m_isInDash)
        {
            m_isInTaunt = true;
			animator.SetBool("isInTaunt", true);
			m_initialTauntTime = Time.time;
        }
    }

    private void HandleMovement()
	{
        if(axisvalue == Vector3.zero)
        {
            animator.SetBool("isRunning", false);
			return;
		}
		animator.SetBool("isRunning", true);

		Vector3 finalPosition = this.transform.position + (new Vector3(axisvalue.x, 0, axisvalue.y)).normalized * movementSpeed * Time.deltaTime;

		this.transform.position =  MapBorders.Instance.ClampVectorToArea(finalPosition);

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
}
