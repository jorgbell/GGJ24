using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] jugglePrefabs;
    [SerializeField] private Transform juggleParent;
    [SerializeField] private float baseThrust;

    [Range(0,1)]
    [SerializeField] private float thrustVariation;

    void Start()
    {
        
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
			Vector3 randomDirection = new Vector3(Random.Range(-thrustVariation, thrustVariation), 1f, Random.Range(-thrustVariation, thrustVariation)).normalized;

			Rigidbody rigidbody = Instantiate(
                jugglePrefabs[Random.Range(0, jugglePrefabs.Length)], this.transform.position, this.transform.rotation, juggleParent)
                .GetComponent<Rigidbody>();

            rigidbody.AddForce(randomDirection * baseThrust);
        }
    }
}
