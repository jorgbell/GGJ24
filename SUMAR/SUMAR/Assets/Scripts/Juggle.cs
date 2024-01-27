using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juggle : MonoBehaviour
{
    [SerializeField] float minTravelTime = 4f, maxTravelTime = 4f;
    [SerializeField] float minHeight = 20f, maxHeight = 20f;

    [SerializeField] PointsManager pointsManager;

    private float __travelTime = 0f, __maxTravelHeight = 0f;
    private Vector3 __targetPosition;
    private float __elapsedTime;

    public void setTargetPosition(Vector3 targetPosition, bool isFrontalThrow = false)
    {
        // isFrontalThrow is meant to indicate whether or not it's launched to a player.
        __travelTime = Random.Range(minTravelTime, maxTravelTime);
        __maxTravelHeight = Random.Range(minHeight, maxHeight);
        __targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);

        StartCoroutine(TravelToTarget());
    }

    public void setPointsmanager(PointsManager pm)
    {
        pointsManager = pm;
    }

    IEnumerator TravelToTarget()
    {
        __elapsedTime = 0.0f;
        float peakTime = __travelTime * 0.5f;
        Vector3 startingPosition = transform.position;

        while (__elapsedTime < __travelTime)
        {
            //Aquí hay un bug. Si la bola spawnea a cierta altura, digamos a 1ud de altura. Con esta fórmula núnca llegará a bajar de 1ud cuando esté bajando por "gravedad"
            float height = Mathf.Lerp(0, __maxTravelHeight, 1 - Mathf.Pow((__elapsedTime - peakTime) / peakTime, 2));

            transform.position = new Vector3(
                Mathf.Lerp(startingPosition.x, __targetPosition.x, __elapsedTime / __travelTime),
                startingPosition.y + height,
				Mathf.Lerp(startingPosition.z, __targetPosition.z, __elapsedTime / __travelTime)

			);

            __elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = __targetPosition;
        pointsManager.dropBall(0);
    }

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(__targetPosition, 0.2f);
    }

    public bool isFalling() {
        return (__elapsedTime > __travelTime / 2) && !isOnFloor();
    }

    public bool isOnFloor()
    {
        return __elapsedTime >= __travelTime;
    }
}
