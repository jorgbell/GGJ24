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
    [Header("Movement")]
    [SerializeField] float movementSpeed;

	[Header("Dash")]
	[SerializeField] float dashSpeed;
	[SerializeField] float dashTime;
	[SerializeField] float dashCooldownTime;

    [Header("Juggle")]
    [SerializeField] int maxJuggleAmmo = 5;
    private int juggleAmmo;
    private List<Juggle> playerJuggles = new List<Juggle>();
    [SerializeField] JuggleArea juggleArea;
    [SerializeField] Juggle jugglePrefab;

    public PlayerActions playerInput;
    private Vector3 axisvalue = new Vector3();
    private Queue<INPUTACTIONS> inputQueue = new Queue<INPUTACTIONS>();

    [SerializeField]
    private int playerID = 1;

    bool m_isInDash = false;
    float m_initialDashTime;
    float m_endDashTime;
    Vector2 m_dashDirection;

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
    }
    private void Start()
    {
        transform.position = MapBorders.Instance.GetRandomPositionInArea(transform.position.y);
        juggleAmmo = maxJuggleAmmo;

        for (int i = 0; i < maxJuggleAmmo; i++)
        {
            Juggle instantiatedJuggle = Instantiate(jugglePrefab, Vector3.zero, Quaternion.identity);
            instantiatedJuggle.SetPlayer(this);
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
                    Debug.Log(catchedInput.ToString());
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
                    Debug.Log(catchedInput.ToString());
                    break;
                case INPUTACTIONS.PAUSE:
                    Debug.Log(catchedInput.ToString());
                    break;
                default:
                    break;
            }
        }

        if (m_isInDash)
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
        if (!m_isInDash && Time.time > m_endDashTime + dashCooldownTime)
        {
            m_isInDash = true;
            m_initialDashTime = Time.time;
            m_dashDirection = axisvalue;
        }
	}

    private void HandleMovement()
	{
        if(axisvalue == Vector3.zero)
        {
            return;
        }

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
			return;
        }

        Vector3 finalPosition = this.transform.position + (new Vector3(m_dashDirection.x, 0, m_dashDirection.y)).normalized * dashSpeed * Time.deltaTime;

		this.transform.position = MapBorders.Instance.ClampVectorToArea(finalPosition);
	}
}
