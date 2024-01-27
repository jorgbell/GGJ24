using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    //singleton related
    private static GameManager instance;
    private static List<InputDevice> devices = new List<InputDevice>();
    public static List<InputDevice> Devices { get { return devices; } } //getter
    public static GameManager Instance //getter
    {
        get { return instance; }
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        //compreba si hay mandos conectados al iniciar la partida
        for (int i = 0; i < InputSystem.devices.Count; i++)
        {
            HandleDeviceChange(InputSystem.devices[i], InputDeviceChange.Added);
        }
    }
    private void OnEnable()
    {
        InputSystem.onDeviceChange += HandleDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= HandleDeviceChange;
    }


    public void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        //casteo para solo recibir mandos
        Gamepad castPad = device as Gamepad; Joystick castJoy = device as Joystick;
        if (castPad != null || castJoy != null)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    devices.Add(device);
                    Debug.Log("Nuevo dispositivo conectado: " + device.displayName);
                    if (ArenaManager.Instance != null)
                        ArenaManager.Instance.RegisterPlayer(device.deviceId);
                    break;

                case InputDeviceChange.Removed:
                    if (devices.Contains(device))
                    {
                        devices.Remove(device);
                        Debug.Log("Dispositivo desconectado: " + device.displayName);
                        if (ArenaManager.Instance != null)
                            ArenaManager.Instance.UnRegisterPlayer(device.deviceId);
                    }
                    else { Debug.LogError("Dispositivo no encontrado: " + device.displayName); }
                    break;
            }
        }
        else
            Debug.LogWarning("Device " + device.displayName + " is not a gamepad or joystick.");

    }



}
