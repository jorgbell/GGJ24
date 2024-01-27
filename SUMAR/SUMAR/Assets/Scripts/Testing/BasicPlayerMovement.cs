using UnityEngine;

public class BasicPlayerMovement : MonoBehaviour
{
	public float moveSpeed = 5f; // Adjust the speed as needed

	void Update()
	{
		// Capture input from the player
		float horizontalInput = 0f;
		float verticalInput = 0f;

		// Check for horizontal movement
		if (Input.GetKey(KeyCode.D))
		{
			horizontalInput += 1f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			horizontalInput -= 1f;
		}

		// Check for vertical movement
		if (Input.GetKey(KeyCode.W))
		{
			verticalInput += 1f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			verticalInput -= 1f;
		}

		// Calculate the movement direction based on input
		Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

		// Calculate the movement amount based on speed and time
		Vector3 movementAmount = movementDirection * moveSpeed * Time.deltaTime;

		// Apply movement to the object
		transform.Translate(movementAmount, Space.World);
	}
}
