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
        airborneBalls[playerIndex]++;
    }

    public void catchBall(int playerIndex)
    {
        points[playerIndex] += airborneBalls[playerIndex];
        airborneBalls[playerIndex]--;
        updateUI();
    }

    public void dropBall(int playerIndex)
    {
        airborneBalls[playerIndex]--;
        points[playerIndex] -= 2;
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
