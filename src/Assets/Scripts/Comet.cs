using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Comet : MonoBehaviour
{


    string bodyName;

    //public static event System.Action OnBodyHasBeenLost;

    public enum BodyType {Sun, Planet};
    public BodyType bodyType;
    public float mass;
    public Vector3 InitialVelocity;
    public Vector3 velocity;
    public bool physicsActive = true;
    public TextMeshPro textMesh;

    

    Vector3 acceleration;
    Rigidbody rb;

    string descriptionText;
    float lifetime = CometSpawner.cometLifetime;




    void Awake() {
        rb = GetComponent<Rigidbody>(); // get reference to its rigidbody
        rb.mass = mass; // set rigidbody mass to match mass setting
        velocity = InitialVelocity;


        // assign a sicc name
        if (bodyType == BodyType.Sun) {
            bodyName = Universe.starNames[Random.Range(0, Universe.starNames.Length - 1)];
        } else if (bodyType == BodyType.Planet) {
            bodyName = Universe.planetNames[Random.Range(0, Universe.planetNames.Length - 1)];
        }

        UpdateDescriptionText();
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0) {
            Destroy(gameObject);
        }


        if (physicsActive) {
            // loop through each body and add an acceleration due to it
            //acceleration = Vector3.zero;
            
            foreach(GameObject body in GameObject.FindGameObjectsWithTag("Body")) {
                float otherMass = body.GetComponent<Rigidbody>().mass;
                Vector3 dir = (body.transform.position - transform.position).normalized; // direction from current body to other body
                float r = (body.transform.position - transform.position).magnitude;

                if (r >= 0.1) {
                    acceleration = dir * Universe.G * otherMass / (r * r);
                    velocity += acceleration * Time.fixedDeltaTime;
                }
            }
            
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }


    /*
    void OnTriggerExit(Collider triggerCollider) {
        if(triggerCollider.tag == "OutOfBounds") {
            levelController.PlanetLost();
            Destroy(gameObject);
        }
    }
    */

    /*
    void OnTriggerEnter(Collider triggerCollider) {

        // if two bodies collide, destroy both
        if(triggerCollider.tag == "Body") {
            Destroy(gameObject);
        }
    }
    */


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
