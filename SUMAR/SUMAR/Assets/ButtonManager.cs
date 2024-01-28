using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
	public void Jugar()
	{
		GameManager.Instance.LoadScene("MainSceneDevelop");
	}

	public void Controler()
	{
		GameManager.Instance.ToggleControls();

	}

	public void Salir()
	{
		GameManager.Instance.QuitGame();
	}
}
