using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPlaceholder : MonoBehaviour
{

    public LayerMask layerMask;

    // Update is called once per frame
    void Update()
    {
        transform.position = GeneratePointFromRayCast();
    }


    Vector3 GeneratePointFromRayCast() {

        Vector3 point = Vector3.zero;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            point = hit.point;
        }

        return point;
        
    }
}
