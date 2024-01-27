using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private JuggleArea juggleArea = null;
	[SerializeField] private Juggle juggle = null;
	[SerializeField] private Transform juggleParent;
	[SerializeField] private Transform juggleSpawnPoint;
	[SerializeField] private float catchRadius = 2f;
	[SerializeField] private int numberBalls = 5;

	[SerializeField] private Text numberBallsText;
	[SerializeField] private PointsManager pointsManager;

	private int actualBalls;

	private void Start()
	{
		actualBalls = numberBalls;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) && actualBalls > 0)
		{
			Vector3 juggleTargetPosition = juggleArea.SelectPoint() + juggleSpawnPoint.position;

			Juggle newJuggle = Instantiate(juggle, juggleSpawnPoint.position, this.transform.rotation, juggleParent);
			newJuggle.setPointsmanager(pointsManager);
			newJuggle.setTargetPosition(juggleTargetPosition);

			pointsManager.throwBall(0);

			actualBalls--;
			numberBallsText.text = "Bolas: "+actualBalls;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Juggle jug = other.GetComponent<Juggle>();

		if (jug != null)
		{
			if (jug.isFalling())
			{
				Debug.Log("Atrapa al aire");
				pointsManager.catchBall(0);
				Destroy(other.gameObject);
				actualBalls++;
			}
			else if (jug.isOnFloor())
			{
				Debug.Log("Atrapa del suelo");	//TODO: cambiar esto, las bolas deberian romper el combo al chocar contra el suelo				
				Destroy(other.gameObject);
				actualBalls++;
			}

			numberBallsText.text = "Bolas: " + actualBalls;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, catchRadius);
	}
}
