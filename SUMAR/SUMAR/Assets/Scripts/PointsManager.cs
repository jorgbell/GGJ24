using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    [SerializeField] private Image[] pointsImage;
    [SerializeField] private Image[] pointsRightBorder;

    private int[] points = new int[]{0,0};
    private int[] airborneBalls = new int[] { 0, 0 };

    [SerializeField] private int maxPoints = 50;

	private void Start()
	{
        updateUI();
    }

	public void throwBall(int playerIndex)
    {
        Debug.Log("Bola lanzada! "+ airborneBalls[playerIndex]);
        airborneBalls[playerIndex]++;
    }

    public void catchBall(int playerIndex)
    {
        Debug.Log("Pilla del aire " + airborneBalls[playerIndex]);
        points[playerIndex] += airborneBalls[playerIndex];

        airborneBalls[playerIndex]--;
        airborneBalls[playerIndex] = Mathf.Max(airborneBalls[playerIndex], 0);
        updateUI();

        if(points[playerIndex] >= maxPoints)
        {
            MenuWin.Instance.WinBro(playerIndex);
		}
	}

    public void dropBall(int playerIndex)
    {
        Debug.Log("se cayó " + airborneBalls[playerIndex]);
        //airborneBalls[playerIndex]--;
        airborneBalls[playerIndex] = Mathf.Max(airborneBalls[playerIndex], 0);
        points[playerIndex] -= 1;
        points[playerIndex] = Mathf.Max(points[playerIndex], 0);
        updateUI();
    }

    private void updateUI() {
        for (int i = 0; i < 2; i++)
        {
            pointsImage[i].transform.localScale = new Vector3((float)points[i] / maxPoints, 1, 1);
            pointsRightBorder[i].rectTransform.pivot = new Vector2(1f - (float)points[i] / maxPoints, 0.5f);
        }
    }
}
