
using UnityEngine;

public class MapBorders : MonoBehaviour
{
	[Range(0, 20)]
	[SerializeField] private float leftBorder;
	[Range(0, 20)]
	[SerializeField] private float rightBorder;
	[Range(0, 10)]
	[SerializeField] private float topBorder;
	[Range(0, 10)]
	[SerializeField] private float bottomBorder;


	public static MapBorders Instance;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow; // Set the color of the rectangle

		// Define the vertices of the rectangle
		Vector3 topLeft = new Vector3(-leftBorder, 0, topBorder);
		Vector3 topRight = new Vector3(rightBorder, 0, topBorder);
		Vector3 bottomLeft = new Vector3(-leftBorder, 0, -bottomBorder);
		Vector3 bottomRight = new Vector3(rightBorder, 0, -bottomBorder);

		// Draw the rectangle using Gizmos.DrawLine
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);
	}

	private void Awake()
	{
		Instance = this;
	}

	public bool CheckPositionOutOfBorders(Vector2 point)
	{
		return point.x >= -leftBorder && point.x <= rightBorder && point.y >= -bottomBorder && point.y <= topBorder;
	}

	public Vector3 ClampVectorToArea(Vector3 point)
	{
		if (point.x < -leftBorder)
		{
			point.x = -leftBorder;
		}
		else if (point.x > rightBorder)
		{
			point.x = rightBorder;
		}

		if (point.z < -bottomBorder)
		{
			point.z = -bottomBorder;

		}
		else if (point.z > topBorder)
		{
			point.z = topBorder;
		}


		return point;
	}
}
