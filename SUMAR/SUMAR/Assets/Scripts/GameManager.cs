using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


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

    private void Start()
    {
        if(Random.Range(0,2) == 0)
            AudioManager.instance.Play("ost1");
        else
            AudioManager.instance.Play("ost1");
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
    public void LoadScene(string sceneName)
    {

        if (SceneExists(sceneName))
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        else
        {
            Debug.LogError("La escena '" + sceneName + "' no existe.");
        }
    }
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuildSettings = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneName == sceneNameInBuildSettings)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9f es el valor máximo cuando la carga está completa.
            Debug.Log("Cargando escena " + sceneName + " - Progreso: " + (progress * 100) + "%");

            yield return null;
        }
    }


}
