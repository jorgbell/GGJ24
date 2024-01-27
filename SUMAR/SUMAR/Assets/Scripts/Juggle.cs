using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JUGGLESTATE { ON_AIR, THROWN, AVAILABLE, ON_FLOOR };

public class Juggle : MonoBehaviour
{
    [SerializeField] float minTravelTime = 0.5f, maxTravelTime = 1f;
    [SerializeField] float minHeight = 10f, maxHeight = 30f;

    private float __travelTime = 0f, __maxTravelHeight = 0f;
    public PlayerController PlayerController;
    private Vector3 __targetPosition;
    public JUGGLESTATE state = JUGGLESTATE.AVAILABLE;

    public void SetPlayer(PlayerController playerController)
    {
        PlayerController = playerController;
    }

    public void setTargetPosition(Vector3 targetPosition, Vector3 startingPosition, bool isFrontalThrow = false)
    {
       this.transform.position = startingPosition;

        // isFrontalThrow is meant to indicate whether or not it's launched to a player.
        __travelTime = Random.Range(minTravelTime, maxTravelTime);
        __maxTravelHeight = Random.Range(minHeight, maxHeight);
        __targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);

        StartCoroutine(TravelToTarget());
    }

    IEnumerator TravelToTarget()
    {
        float elapsedTime = 0.0f, peakTime = __travelTime * 0.5f;
        Vector3 startingPosition = transform.position;
        state = JUGGLESTATE.ON_AIR;

        while (elapsedTime < __travelTime)
        {
            //Aqu� hay un bug. Si la bola spawnea a cierta altura, digamos a 1ud de altura. Con esta f�rmula n�nca llegar� a bajar de 1ud cuando est� bajando por "gravedad"
            float height = Mathf.Lerp(0, __maxTravelHeight, 1 - Mathf.Pow((elapsedTime - peakTime) / peakTime, 2));

            transform.position = new Vector3(
                Mathf.Lerp(startingPosition.x, __targetPosition.x, elapsedTime / __travelTime),
                startingPosition.y + height,
				Mathf.Lerp(startingPosition.z, __targetPosition.z, elapsedTime / __travelTime)

			);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
        state = JUGGLESTATE.ON_FLOOR;
        transform.position = __targetPosition;
    }
}
