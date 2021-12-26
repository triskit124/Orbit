using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class CameraMovement : MonoBehaviour {

    [SerializeField]
    private float lookSpeedH = 4f;
    
    [SerializeField]
    private float lookSpeedV = 4f;

    [SerializeField]
    private float zoomSpeed = 5f;

    [SerializeField]
    private float dragSpeed = 500f;

    private float yaw = 0f;
    private float pitch = 0f;
    private float maxCameraPosition = 200f;

    private void Start() {
        // Initialize the correct initial rotation
        this.yaw = this.transform.eulerAngles.y;
        this.pitch = this.transform.eulerAngles.x;
    }

    private void Update() {

        Vector3 prevPosition = this.transform.position;

        //Look around with Right Mouse
        if (Input.GetMouseButton(1))
        {
            this.yaw += this.lookSpeedH * Input.GetAxis("Mouse X");
            this.pitch -= this.lookSpeedV * Input.GetAxis("Mouse Y");

            this.transform.eulerAngles = new Vector3(this.pitch, this.yaw, 0f);
        }

        //drag camera around with Middle Mouse
        if (Input.GetMouseButton(2))
        {
            transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
        }

        /*
        if (Input.GetMouseButton(1))
        {
            //Zoom in and out with Right Mouse
            this.transform.Translate(0, 0, Input.GetAxisRaw("Mouse X") * this.zoomSpeed * .07f, Space.Self);
        }
        */

        //Zoom in and out with Mouse Wheel
        this.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * this.zoomSpeed, Space.Self);

        // Constrain camera position to bounds
        if (this.transform.position.magnitude > maxCameraPosition) {
            this.transform.position = prevPosition;
        }
    }
}
