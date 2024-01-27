using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SpotLightManager : MonoBehaviour
{
    [SerializeField]
    Transform Position;
    [SerializeField]
    uint uNumOfLights;
    [SerializeField]
    float fNumOfLights;

    [SerializeField]
    List<Light> lights;

    [SerializeField]
    List<Transform> players;

    [SerializeField]
    [Range(-1, 1)]
    float fIntrest;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerSeparation = players[1].position - players[0].position;

        foreach (Light light in lights)
        {
            float[] fClosenessFactor = { ClosenessFactor(light, players[0].position), ClosenessFactor(light, players[1].position) };

            //Relative percentual distance
            float fRPD = (Mathf.Abs(fClosenessFactor[0] - fClosenessFactor[1]) / ((fClosenessFactor[0] + fClosenessFactor[1]) /2));
            Debug.Log(fRPD);
            //Alguien le esta pasando la mano al otro
            if(fRPD > 0.9)
            {
                Vector3 vTarget = new Vector3();
                if (fClosenessFactor[0] > fClosenessFactor[1])
                {
                    vTarget = Vector3.Lerp(players[1].position, players[0].position, fRPD);
                }
                else
                {
                    vTarget = Vector3.Lerp(players[0].position, players[1].position, fRPD);
                }

                light.transform.LookAt(vTarget);
            }
            else
            {
                if (fClosenessFactor[0] > fClosenessFactor[1])
                {
                    light.transform.LookAt(players[0].position);
                }
                else
                {
                    light.transform.LookAt(players[1].position);
                }
            }

        }

    }

    float ClosenessFactor(Light l, Vector3 pos)
    {
        float fDistance = (l.transform.position - pos).sqrMagnitude;

        return 1 / fDistance;
    }


}
