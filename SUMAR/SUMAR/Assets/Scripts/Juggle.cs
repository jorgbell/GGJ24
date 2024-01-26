using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juggle : MonoBehaviour
{
    [SerializeField] float minTravelTime = 0.5f, maxTravelTime = 1f;
    [SerializeField] float minHeight = 10f, maxHeight = 30f;

    private float __travelTime = 0f, __maxTravelHeight = 0f;
    private Vector3 __targetPosition;

    public void setTargetPosition(Vector3 targetPosition, bool isFrontalThrow = false)
    {
        // isFrontalThrow is meant to indicate whether or not it's launched to a player.
        __travelTime = Random.Range(minTravelTime, maxTravelTime);
        __maxTravelHeight = Random.Range(minHeight, maxHeight);
        __targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0);

        StartCoroutine(TravelToTarget());
    }

    IEnumerator TravelToTarget()
    {
        float elapsedTime = 0.0f, peakTime = __travelTime * 0.5f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < __travelTime)
        {
            float height = Mathf.Lerp(0, __maxTravelHeight, 1 - Mathf.Pow((elapsedTime - peakTime) / peakTime, 2));

            transform.position = new Vector3(
                Mathf.Lerp(startingPosition.x, __targetPosition.x, elapsedTime / __travelTime),
                Mathf.Lerp(startingPosition.y, __targetPosition.y, elapsedTime / __travelTime),
                startingPosition.z - height
            );

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = __targetPosition;
    }
}
