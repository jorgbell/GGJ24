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

public class PlayerController : MonoBehaviour
{
    public PlayerActions playerInput;
    private Vector3 axisvalue = new Vector3();

    private void Awake()
    {
        playerInput = new PlayerActions();
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        Debug.Log("Axis movement change");
        Debug.Log(axisvalue);
        axisvalue = ctx.ReadValue<Vector3>();
    }

}
