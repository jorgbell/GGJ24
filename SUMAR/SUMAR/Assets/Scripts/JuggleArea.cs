using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleArea : MonoBehaviour
{
    [SerializeField] private float maxDiameter = 10.0f;
    [SerializeField] private float minDiameter = 5.0f;
    [SerializeField] private GameObject minDiameterIndicator = null;
    [SerializeField] private GameObject maxDiameterIndicator = null;

    void Start()
    {
        minDiameterIndicator.transform.localScale = new Vector3(minDiameter, 1, minDiameter);
        maxDiameterIndicator.transform.localScale = new Vector3(maxDiameter, 1, maxDiameter);

        Renderer minIndicatorRenderer = minDiameterIndicator.GetComponent<Renderer>();
        minIndicatorRenderer.material.color = new Color(1f, 1f, 1f, 0.1f);

        Renderer maxIndicatorRenderer = minDiameterIndicator.GetComponent<Renderer>();
        maxIndicatorRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    }

    public Vector3 SelectPoint()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI); // Random angle in radians
        float distance = Random.Range(minDiameter * 0.5f, maxDiameter * 0.5f);

        float x = distance * Mathf.Cos(angle);
        float z = distance * Mathf.Sin(angle);

        Vector3 randomPoint = new Vector3(x, 0f, z);

        return randomPoint;
    }
}
