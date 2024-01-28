using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    //singleton related
    private static GameManager instance;
    public static GameManager Instance //getter
    {
        get { return instance; }
    }



	private int playerId = 0;


	public CameraEffects cameraEffects;
    [SerializeField] private Transform[] spawnPoints;
    
    
    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("GameManager instanced");
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            instance.cameraEffects = cameraEffects;
            Destroy(gameObject);
        }



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
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9f es el valor m�ximo cuando la carga est� completa.
            Debug.Log("Cargando escena " + sceneName + " - Progreso: " + (progress * 100) + "%");

            yield return null;
        }
    }

    public int getPlayerId() {

        return playerId++;
    }

    public Transform GetSpawnPoint(int id)
    {
        if(id < spawnPoints.Length)
        {
            return spawnPoints[id];
        }

        return null;
    }
}
