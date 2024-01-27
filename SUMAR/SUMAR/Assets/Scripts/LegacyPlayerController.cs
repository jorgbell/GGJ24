using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegacyPlayerController : MonoBehaviour
{
    [SerializeField] private JuggleArea juggleArea = null;
    [SerializeField] private Juggle juggle = null;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 juggleTargetPosition = juggleArea.SelectPoint();

            Juggle newJuggle = Instantiate(juggle);
            newJuggle.setTargetPosition(juggleTargetPosition);
        }
    }
}
