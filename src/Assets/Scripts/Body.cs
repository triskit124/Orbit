using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Body : MonoBehaviour
{

    public enum BodyType {Sun, Planet};
    public BodyType bodyType;
    public float mass;
    public Vector3 InitialVelocity;
    public Vector3 velocity;
    public bool physicsActive = false;
    public TextMeshPro textMesh;
    public TextMeshPro velocityText;
    public Transform predictStepHolder;
    
    int numPredictSteps = 100;
    string bodyName;
    string descriptionText;
    GameObject spawner;
    LineRenderer lineRenderer;
    Vector3 acceleration;
    Rigidbody rb;


    void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>(); // get reference to its rigidbody
        rb.mass = mass; // set rigidbody mass to match mass setting
        spawner = GameObject.Find("BodySpawner"); // find the spawner gameObject
        velocity = InitialVelocity;

        // assign a sicc name
        if (bodyType == BodyType.Sun) {
            bodyName = Universe.starNames[UnityEngine.Random.Range(0, Universe.starNames.Length - 1)];
        } else if (bodyType == BodyType.Planet) {
            bodyName = Universe.planetNames[UnityEngine.Random.Range(0, Universe.planetNames.Length - 1)];
        }

        UpdateDescriptionText();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (physicsActive && !LevelController.levelPaused) {
            
            // loop through each body and add an acceleration due to it            
            foreach(GameObject body in GameObject.FindGameObjectsWithTag("Body")) {
                float otherMass = body.GetComponent<Rigidbody>().mass;
                Vector3 dir = (body.transform.position - transform.position).normalized; // direction from current body to other body
                float r = (body.transform.position - transform.position).magnitude;

                if (r > 0.5) {
                    acceleration = dir * Universe.G * otherMass / (r * r);
                    velocity += acceleration * Time.fixedDeltaTime;
                }
            }
            
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    void OnTriggerExit(Collider triggerCollider) {
        if(triggerCollider.tag == "OutOfBounds") {
            //print(bodyType);
            if (bodyType == BodyType.Sun) {
                FindObjectOfType<LevelController>().SunLost();
            } else {
                FindObjectOfType<LevelController>().PlanetLost();
            }
            Destroy(gameObject);
        }
    }

    public void PredictPath(Vector3 currentVelocity) {
        Vector3 position = transform.position;
        Vector3[] positions = new Vector3[numPredictSteps];
        positions[0] = position;

        for (int j = 0; j < numPredictSteps - 1; j++) {
            foreach(GameObject body in GameObject.FindGameObjectsWithTag("Body")) {
                float otherMass = body.GetComponent<Rigidbody>().mass;
                Vector3 dir = (body.transform.position - positions[j]).normalized; // direction from current body to other body
                float r = (body.transform.position - positions[j]).magnitude;

                if (r > 0.5) {
                    Vector3 acceleration = dir * Universe.G * otherMass / (r * r);          
                    currentVelocity += acceleration * Time.fixedDeltaTime;
                }
            }
            
            position += currentVelocity * Time.fixedDeltaTime;
            positions[j+1] = position;

            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
            lineRenderer.enabled = true;
        }
        
    }

    private Vector3 TrapezoidalIntegration(Vector3[] x) {
        Vector3 sum = Vector3.zero;
        float coeff;
        for (int i = 0; i < x.Length; i++) {
            if (i == 0 || i == x.Length - 1) {
                coeff = 0.5f;
            } else {
                coeff = 1f;
            }
            sum += coeff * x[i] * Time.fixedDeltaTime;
        }
        return sum;
    }

    public void UpdateMass(float massInput) {
        mass = massInput;
        rb.mass = massInput;
        UpdateDescriptionText();
    }

    public void UpdateVelocity(Vector3 velocityInput) {
        velocity = velocityInput;
    }

    public void UpdateDescriptionText() {
        descriptionText = bodyName + "\n" + "Mass: " + mass.ToString();
        textMesh.text = descriptionText;
    }

}
