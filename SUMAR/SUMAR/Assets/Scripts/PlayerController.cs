using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private JuggleArea juggleArea = null;
    [SerializeField] private Juggle juggle = null;
    [SerializeField] private Transform juggleParent;
    [SerializeField] private Transform juggleSpawnPoint;
    [SerializeField] private float catchRadius = 2f;
    [SerializeField] private GameObject ballParent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 juggleTargetPosition = juggleArea.SelectPoint() + juggleSpawnPoint.position;
                        
			Juggle newJuggle = Instantiate(juggle, juggleSpawnPoint.position, this.transform.rotation, juggleParent);
            newJuggle.setTargetPosition(juggleTargetPosition);
        }
    }

	private void OnTriggerEnter(Collider other)
	{
        Debug.Log(other.gameObject.name);
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, catchRadius);
    }
}
