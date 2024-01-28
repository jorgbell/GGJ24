using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class MenuWin : MonoBehaviour
{
    public GameObject menuGanar;
	public TextMeshProUGUI menuText;

	public static MenuWin Instance;

	public void Awake()
	{
		Instance = this;
	}

	public void WinBro(int playerID)
	{
		string aux = playerID == 0 ? "rojo" : "azul";
		menuText.text = $"Has ganado jugador {aux}";

	}
	public void ToggleMenu()
	{
		menuGanar.SetActive(!menuGanar.activeSelf);
		Time.timeScale = menuGanar.activeSelf ? 0.0f : 1.0f;
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
