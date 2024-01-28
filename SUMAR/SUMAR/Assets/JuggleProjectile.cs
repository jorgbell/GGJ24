using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleProjectile : MonoBehaviour
{
    [SerializeField] Juggle juggle; //Esto es una puta guarrada porque no supe como manejar la movida de los pivotes al tener GObjects anidados.

    public void TryPickUpFromFloor(int playerID)
    {
        juggle.TryPickupFromFloor(playerID);
    }

    public bool TryReceiveShot(int playerID)
    {
        return juggle.TryReceiveShot(playerID);
    }
}
