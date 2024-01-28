using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    [SerializeField] private Image pointsImage;
    [SerializeField] private Image pointsRightBorder;

    private int points = 0;
    private int airborneBalls = 0;

    private int maxPoints = 50;

	private void Start()
	{
        pointsImage.transform.localScale = new Vector3((float)points / maxPoints, 1,1);
        pointsRightBorder.rectTransform.pivot = new Vector2(1f - (float)points / maxPoints, 0.5f);
    }

	public void throwBall(int playerIndex)
    {
        airborneBalls++;
    }

    public void catchBall(int playerIndex)
    {
        points += airborneBalls;
        airborneBalls--;
        pointsImage.rectTransform.localScale = new Vector3((float)points / maxPoints - 0.005f, 1, 1);
        pointsRightBorder.rectTransform.pivot = new Vector2(1f - (float)points / maxPoints, 0.5f);
    }

    public void dropBall(int playerIndex)
    {
        airborneBalls--;
        Debug.Log("se cayó la bola");
    }
}
