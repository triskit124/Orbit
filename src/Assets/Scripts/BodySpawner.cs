using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BodySpawner : MonoBehaviour
{

    public float mass = 50f;
    public GameObject[] bodyPrefabs;
    public GameObject velcityVectorPrefab;
    public GameObject bodyPlaceholderPrefab;
    public LayerMask layerMask;
    public Slider massSlider;
    public Text massSliderText;

    bool mouseWasDownLastFrame;
    bool generatePlaceholder = true;
    float dragDistance;
    Vector3 origSpawnPos;
    Vector3 origMousePos;
    Vector3 spawnPos;
    Vector3 currentPos;
    Vector3 lookRotation;
    Vector3 lastValidPoint;
    GameObject newBody;
    GameObject newBodyPlaceholder;
    bool hasWaitedForLevelToLoad = false;

    // Update is called once per frame
    void Update() {



        // mouse clicked
        if (Input.GetMouseButton(0)) {
            if (hasWaitedForLevelToLoad) {
                // mouse hold
                if (mouseWasDownLastFrame) {

                    Destroy(newBodyPlaceholder); // get rid of sphere that follows mouse
                    newBody.GetComponent<Body>().velocityText.text = dragDistance.ToString("F1"); // "F1" specifies float with one decimal place

                    // get mouse click position on solar plane
                    currentPos = GeneratePointFromRayCast();

                    // get direction and magnitude of initial velocity
                    Vector3 mousePosToOrigPos = -(currentPos - spawnPos).normalized;
                    dragDistance = (currentPos - spawnPos).magnitude;
                    lookRotation = Quaternion.LookRotation(mousePosToOrigPos) * Vector3.forward;

                    // predict the path by integrating forwards in time with current initial velocity
                    newBody.GetComponent<Body>().PredictPath(2.5f * dragDistance * lookRotation);
                } else {         
                    if (!IsPointerOverUIElement()) {    
                        // first mouse click
                        mouseWasDownLastFrame = true;

                        // get mouse click position on solar plane and spawn a new body there
                        spawnPos = GeneratePointFromRayCast();
                        newBody = (GameObject)Instantiate(bodyPrefabs[Random.Range(0, bodyPrefabs.Length)], spawnPos, Quaternion.identity);
                    }
                }
            }

        } else {

            // mouse not clicked
            if (mouseWasDownLastFrame && hasWaitedForLevelToLoad) {
                // spawn the new body
                newBody.GetComponent<LineRenderer>().enabled = false;
                newBody.GetComponent<Body>().UpdateVelocity(2.5f * dragDistance * lookRotation);
                newBody.GetComponent<Body>().UpdateMass(mass);
                newBody.GetComponent<Body>().physicsActive = true;
                newBody.GetComponent<Body>().textMesh.GetComponent<MeshRenderer>().enabled = true; // TODO: there has to be a better way to do this...
                newBody.GetComponent<Body>().velocityText.GetComponent<MeshRenderer>().enabled = false;
                newBody.transform.parent = transform; // parent the new body to the spawner
                FindObjectOfType<LevelController>().PlanetPlaced();

                generatePlaceholder = true;
                mouseWasDownLastFrame = false;
            }

            if (generatePlaceholder) {
                newBodyPlaceholder = (GameObject)Instantiate(bodyPlaceholderPrefab, GeneratePointFromRayCast(), Quaternion.identity);
                generatePlaceholder = false;
            }

            hasWaitedForLevelToLoad = true;
        }
    }

    public void SetNewMassValue() {
        mass = massSlider.value;
        massSliderText.text = mass.ToString();
    }

    private Vector3 GeneratePointFromRayCast() {
        Vector3 point = Vector3.zero;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            point = hit.point;
            lastValidPoint = point;
        } else {
            point = lastValidPoint;
        }

        return point; 
    }

    // Machinery below here is to determine if a click if over any GUI elements
    // (TODO: there's gotta be a more elegant way to do this)
    private bool IsPointerOverUIElement() {
        List<RaycastResult> eventSystemRaysastResults = GetEventSystemRaycastResults();
    
        for(int index = 0;  index < eventSystemRaysastResults.Count; index ++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults [index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }

    ///Gets all event systen raycast results of current mouse or touch position.
    private List<RaycastResult> GetEventSystemRaycastResults() {   
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position =  Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll( eventData, raysastResults );
        return raysastResults;
    }

}
