using System;
using UnityEditor;
using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausa;

	public static MenuPausa Instance;

	private void Awake()
	{
		Instance = this;
	}

	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape) )
		{
			ToggleMenu();
		}
	}

	public void ToggleMenu()
	{
		menuPausa.SetActive(!menuPausa.activeSelf);
		Time.timeScale = menuPausa.activeSelf ? 0.0f : 1.0f;
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode(); ;
#else
		Application.Quit();
#endif
	}

}
