using TMPro;
using UnityEditor;
using UnityEngine;

public class MenuWin : MonoBehaviour
{
	public PointsManager pointsManager;
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
		ToggleMenu();
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

	public void Menu()
	{
		GameManager.Instance.LoadScene("InitSceneDevelop");
	}

	public void EndlessMode()
	{
		ToggleMenu();
		pointsManager.SetEndlessMode(true);
	}
}
