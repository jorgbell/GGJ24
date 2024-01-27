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
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);


    }

    private void Start()
    {
        //if (Random.Range(0, 2) == 0)
        //    AudioManager.instance.Play("ost1");
        //else
        //    AudioManager.instance.Play("ost1");
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
