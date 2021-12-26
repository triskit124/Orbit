using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour
{

    public int numStarsRequested;
    public float radius;
    public GameObject starPrefab;

    bool needsUpdate = true;
    Transform holder;
    int currentNumberOfStars;

    float maxTheta = 2 * Mathf.PI;
    float minTheta = 0.0f;
    float maxPhi = 2 * Mathf.PI;
    float minPhi = 0.0f;



    void Update() {
        if (needsUpdate) {
            Generate();
        }
    }

    void Generate() {

        if (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
            currentNumberOfStars = 0;
        }
        holder = new GameObject("Holder").transform;
		holder.parent = transform;

        while (currentNumberOfStars < numStarsRequested) {
            float theta = Random.Range(minTheta, maxTheta);
            float phi = Random.Range(minPhi, maxPhi);
            Vector3 spawnPos = new Vector3(radius * Mathf.Cos(theta) * Mathf.Sin(phi), radius * Mathf.Sin(theta) * Mathf.Sin(phi), radius * Mathf.Cos(phi));

            // spawn the new star
            GameObject newStar = (GameObject)Instantiate(starPrefab, spawnPos, Quaternion.identity);
            newStar.transform.parent = holder;
            currentNumberOfStars++;
        }

        needsUpdate = false;

    }


    void OnValidate() {
        needsUpdate = true;
    }

}
