using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// El funcionamiento del sistema de input es:
/// - Abrir el PlayerActions
/// - Añadir una nueva action dentro del action map
/// - Crear un metodo que reciba un callbackContext publico y asociarlo al script Player Input
/// - pillar el valor de la accion dentro del metodo
/// </summary>

public enum INPUTACTIONS { ATTACK, CATCH, THROW, DASH, TAUNT, PAUSE};

public class PlayerController : MonoBehaviour
{
    public PlayerActions playerInput;
    private Vector3 axisvalue = new Vector3();
    private Queue<INPUTACTIONS> inputQueue = new Queue<INPUTACTIONS>();


    private void Awake()
    {
        playerInput = new PlayerActions();
        // Asignar funciones a los eventos de las acciones
        playerInput.PlayerActionMap.Attack.performed += ctx => EnqueueActionInput(INPUTACTIONS.ATTACK);
        playerInput.PlayerActionMap.CatchBall.performed += ctx => EnqueueActionInput(INPUTACTIONS.CATCH);
        playerInput.PlayerActionMap.ThrowBall.performed += ctx => EnqueueActionInput(INPUTACTIONS.THROW);
        playerInput.PlayerActionMap.Dash.performed += ctx => EnqueueActionInput(INPUTACTIONS.DASH);
        playerInput.PlayerActionMap.Taunt.performed += ctx => EnqueueActionInput(INPUTACTIONS.TAUNT);
        playerInput.PlayerActionMap.Pause.performed += ctx => EnqueueActionInput(INPUTACTIONS.PAUSE);
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
        while(inputQueue.TryDequeue(out catchedInput))
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
    }

    public void EnqueueActionInput(INPUTACTIONS input)
    {
        Debug.Log(input);
    }



    public void EnqueueAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log(ctx.action.id.ToString());
            Debug.Log(ctx.action.name);
            Debug.Log(ctx.action.type.ToString());
        }

    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        Debug.Log("Axis movement change");
        Debug.Log(axisvalue);
        axisvalue = ctx.ReadValue<Vector3>();
    }

}
