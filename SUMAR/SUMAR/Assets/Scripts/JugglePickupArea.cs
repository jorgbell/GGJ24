using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JUGGLEPICKUPAREASTATE { IDLE, PICKABLE };

public class JugglePickupArea : MonoBehaviour
{
    [SerializeField] Juggle juggle;
    [SerializeField] int PlayerId;
    [SerializeField] SpriteRenderer sprite;

    public JUGGLEPICKUPAREASTATE state = JUGGLEPICKUPAREASTATE.IDLE;

    void Start()
    {
        sprite.enabled = false;
    }

    public void SetPlayerId(int playerId)
    {
        PlayerId = playerId;
    }

    public void SetActiveInPosition(Vector3 position)
    {
        this.transform.position = position;
        sprite.enabled = true;
        sprite.color = Color.white;
    }

    public void SetPickable(){
        state = JUGGLEPICKUPAREASTATE.PICKABLE;
        sprite.color = Color.red;
    }

    public void OnJuggleDropped(){
        state = JUGGLEPICKUPAREASTATE.IDLE;
        sprite.enabled = false;
    }

    public bool TryPickup(int playerId)
    {
        if (state == JUGGLEPICKUPAREASTATE.PICKABLE && playerId == PlayerId) 
        {
            state = JUGGLEPICKUPAREASTATE.IDLE;
            sprite.enabled = false;
            juggle.PickUpFromAir();
            return true;
        }

        return false;
    }
}
