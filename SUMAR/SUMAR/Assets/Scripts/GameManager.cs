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

	private void Start()
	{
        AudioManager.instance.Play("menu");
	}

	public void LoadScene(string sceneName)
    {

        if (SceneExists(sceneName))
		{
            AudioManager.instance.StopAll();
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

        yield return new WaitForSeconds(0.2f);
        if (Random.Range(0, 2) == 0)
        {
            AudioManager.instance.Play("ost1");
        }
        else
        {
            AudioManager.instance.Play("ost2");
        }
    }

    public int getPlayerId() {

        return playerId++;
    }
}
