using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//enum para gestionar los tipos de inputs digitales (botones encendido-apagado)
public enum INPUTACTIONS { ATTACK, CATCH, THROW, DASH, TAUNT, PAUSE};

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed;

	[Header("Dash")]
	[SerializeField] float dashSpeed;
	[SerializeField] float dashTime;
	[SerializeField] float dashCooldownTime;

    public PlayerActions m_playerInput;
    private Vector3 m_axisvalue;
    private Queue<INPUTACTIONS> m_inputQueue = new Queue<INPUTACTIONS>();

    bool m_isInDash = false;
    float m_initialDashTime;
    float m_endDashTime;
    Vector2 m_dashDirection;

    private void Awake()
    {
		m_axisvalue = new Vector3();

		m_playerInput = new PlayerActions();
        // por cada input que esté en el enum hacer esto
        m_playerInput.PlayerActionMap.Attack.performed += ctx => EnqueueActionInput(INPUTACTIONS.ATTACK);
        m_playerInput.PlayerActionMap.CatchBall.performed += ctx => EnqueueActionInput(INPUTACTIONS.CATCH);
        m_playerInput.PlayerActionMap.ThrowBall.performed += ctx => EnqueueActionInput(INPUTACTIONS.THROW);
        m_playerInput.PlayerActionMap.Dash.performed += ctx => EnqueueActionInput(INPUTACTIONS.DASH);
        m_playerInput.PlayerActionMap.Taunt.performed += ctx => EnqueueActionInput(INPUTACTIONS.TAUNT);
        m_playerInput.PlayerActionMap.Pause.performed += ctx => EnqueueActionInput(INPUTACTIONS.PAUSE);
    }

    private void OnEnable()
    {
        m_playerInput.Enable();
    }

    private void OnDisable()
    {
        m_playerInput.Disable();
    }

    private void Update()
    {
        INPUTACTIONS catchedInput;
        while(m_inputQueue.TryDequeue(out catchedInput))
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
                    Debug.Log(catchedInput.ToString());
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

    public void EnqueueActionInput(INPUTACTIONS input)
    {
        m_inputQueue.Enqueue(input);
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        m_axisvalue = ctx.ReadValue<Vector3>();
	}

    public void OnDash()
    {
        if (!m_isInDash && Time.time > m_endDashTime + dashCooldownTime)
        {
            m_isInDash = true;
            m_initialDashTime = Time.time;
            m_dashDirection = m_axisvalue;
        }
	}

    private void HandleMovement()
	{
        if(m_axisvalue == Vector3.zero)
        {
            return;
        }

        Vector3 finalPosition = this.transform.position + (new Vector3(m_axisvalue.x, 0, m_axisvalue.y)).normalized * movementSpeed * Time.deltaTime;

		this.transform.position =  MapBorders.Instance.ClampVectorToArea(finalPosition);
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
