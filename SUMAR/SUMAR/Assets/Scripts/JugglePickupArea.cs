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
    }

    public void SetPickable(){
        state = JUGGLEPICKUPAREASTATE.PICKABLE;
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

	private void OnDrawGizmos()
	{
        if (sprite.enabled)
        {
            Gizmos.DrawWireSphere(this.transform.position, GetComponent<SphereCollider>().radius * 0.2f);
        }

	}
}
