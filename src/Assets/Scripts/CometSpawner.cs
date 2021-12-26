using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometSpawner : MonoBehaviour
{

    public int spawnTimeRangeMin = 30;
    public int spawnTimeRangeMax = 100;
    public GameObject cometPrefab;
    public float velocityMagnitude = 50f;
    public float radius = 300f;
    public float directionOffset = 60;
    public static float cometLifetime = 30f;
    public float cometMass = 50f;

    float maxTheta = 2 * Mathf.PI;
    float minTheta = 0.0f;
    float maxPhi = 2 * Mathf.PI;
    float minPhi = 0.0f;
    float timeUntilSpawn;


    void Start()
    {
        timeUntilSpawn = Random.Range(spawnTimeRangeMin, spawnTimeRangeMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeUntilSpawn <= 0) {
            float theta = Random.Range(minTheta, maxTheta);
            float phi = Random.Range(minPhi, maxPhi);
            Vector3 spawnPos = new Vector3(radius * Mathf.Cos(theta) * Mathf.Sin(phi), radius * Mathf.Sin(theta) * Mathf.Sin(phi), radius * Mathf.Cos(phi));
            Vector3 spawnVelocity = (-spawnPos + new Vector3(Random.Range(0, directionOffset), Random.Range(0, directionOffset), Random.Range(0, directionOffset))).normalized * velocityMagnitude;

            // spawn the new comet
            GameObject newComet = (GameObject)Instantiate(cometPrefab, spawnPos, Quaternion.identity);
            newComet.GetComponent<Comet>().UpdateVelocity(spawnVelocity);
            newComet.GetComponent<Comet>().UpdateMass(cometMass);
            newComet.transform.parent = transform;

            timeUntilSpawn = Random.Range(spawnTimeRangeMin, spawnTimeRangeMax);
        } else {
            timeUntilSpawn -= Time.deltaTime;
        }

    }
}
