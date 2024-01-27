using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegacyPlayerController : MonoBehaviour
{
    [SerializeField] private JuggleArea juggleArea = null;
    [SerializeField] private Juggle juggle = null;
    [SerializeField] private Transform juggleParent;
    [SerializeField] private Transform juggleSpawnPoint;

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 juggleTargetPosition = juggleArea.SelectPoint() + juggleSpawnPoint.position;
                        
			Juggle newJuggle = Instantiate(juggle, juggleSpawnPoint.position, this.transform.rotation, juggleParent);
            newJuggle.setTargetPosition(juggleTargetPosition);
        }
    }
}
