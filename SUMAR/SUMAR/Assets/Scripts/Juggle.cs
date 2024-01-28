using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JUGGLESTATE { ON_AIR, THROWN, AVAILABLE, ON_FLOOR };

public class Juggle : MonoBehaviour
{
    [SerializeField] float minTravelTime = 0.5f, maxTravelTime = 1f;
    [SerializeField] float shootSpeed = 10f;
    [SerializeField] float pickupThreshold = 0.7f; //At X% of fall time the player will be able to pick it up
    [SerializeField] float minHeight = 10f, maxHeight = 30f;
    [SerializeField] JugglePickupArea jugglePickupArea;
    [SerializeField] GameObject juggleBall;
    [SerializeField] SpriteRenderer sprite;

    private float __travelTime = 0f, __maxTravelHeight = 0f;
    private PlayerController __playerController;
    public int __playerID;
    private Vector3 __targetPosition;
    private float elapsedTime;
    private SpriteRenderer spriteRenderer;
    private Vector3 __direction;
    private Coroutine travelCoroutine;
    private Coroutine shootCoroutine;
    public JUGGLESTATE state = JUGGLESTATE.AVAILABLE;

    private PointsManager pointsManager;

    void Start()
    {
        sprite.enabled = false;
    }

    public void SetPlayer(PlayerController playerController)
    {
        __playerController = playerController;
        __playerID = playerController.playerID;
        jugglePickupArea.SetPlayerId(__playerID);
    }

    public void Shoot(Vector3 startingPosition, Vector3 direction)
    {
        transform.position = startingPosition;
        juggleBall.transform.localPosition = new Vector3(0, 0, 0);
        __direction = direction;
        sprite.enabled = true;
        state = JUGGLESTATE.THROWN;

        shootCoroutine = StartCoroutine(ShootCoroutine());
    }

    IEnumerator ShootCoroutine()
    {
        while (MapBorders.Instance.CheckPositionInBorders(juggleBall.transform.position) == true){
            juggleBall.transform.position += (__direction * shootSpeed * Time.deltaTime);

            yield return null;
        }

        state = JUGGLESTATE.ON_FLOOR;
        GameManager.Instance.cameraEffects.shakeDuration = 2;
    }

    public bool TryReceiveShot(int playerID)
    {
        if (playerID == __playerID || state != JUGGLESTATE.THROWN) return false;

        StopCoroutine(shootCoroutine);
        shootCoroutine = null;

        Vector3 newJugglePosition = __playerController.GetJugglePosition();

        setTargetPosition(newJugglePosition, juggleBall.transform.position);

        return true;
    }

    public void setTargetPosition(Vector3 targetPosition, Vector3 startingPosition, bool isFrontalThrow = false)
    {
       this.transform.position = startingPosition;

        // isFrontalThrow is meant to indicate whether or not it's launched to a player.
        __travelTime = Random.Range(minTravelTime, maxTravelTime);
        __maxTravelHeight = Random.Range(minHeight, maxHeight);
        __targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);

        jugglePickupArea.SetActiveInPosition(__targetPosition);

        travelCoroutine = StartCoroutine(TravelToTarget());
    }

    public void setPointsManager(PointsManager pm) {
        pointsManager = pm;
    }

    IEnumerator TravelToTarget()
    {
        elapsedTime = 0.0f;
        float peakTime = __travelTime * 0.5f;
        Vector3 startingPosition = transform.position;
        sprite.enabled = true;
        state = JUGGLESTATE.ON_AIR;

        while (elapsedTime < __travelTime)
        {
            //Aqu� hay un bug. Si la bola spawnea a cierta altura, digamos a 1ud de altura. Con esta f�rmula n�nca llegar� a bajar de 1ud cuando est� bajando por "gravedad"
            float height = Mathf.Lerp(0, __maxTravelHeight, 1 - Mathf.Pow((elapsedTime - peakTime) / peakTime, 2));

            juggleBall.transform.position = new Vector3(
                Mathf.Lerp(startingPosition.x, __targetPosition.x, elapsedTime / __travelTime),
                startingPosition.y + height,
				Mathf.Lerp(startingPosition.z, __targetPosition.z, elapsedTime / __travelTime)
			);

            elapsedTime += Time.deltaTime;

            if (elapsedTime > __travelTime * pickupThreshold && jugglePickupArea.state == JUGGLEPICKUPAREASTATE.IDLE) jugglePickupArea.SetPickable();

            yield return null;
        }
        
        state = JUGGLESTATE.ON_FLOOR;
        pointsManager.dropBall(__playerID);
        jugglePickupArea.OnJuggleDropped();
        juggleBall.transform.position = __targetPosition;
    }

    public void PickUpFromAir()
    {
        state = JUGGLESTATE.AVAILABLE;
        sprite.enabled = false;

        StopCoroutine(travelCoroutine);
        travelCoroutine = null;
    }

    public void TryPickupFromFloor(int playerID)
    {
        if (__playerID != playerID || state != JUGGLESTATE.ON_FLOOR) return;

        state = JUGGLESTATE.AVAILABLE;
        sprite.enabled = false;
    }
}
