using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public float moveSpeedPerPixel = 0.05f;
    public float rotationSpeedPerPixel = 0.05f;
    public float zoomSpeedPerWheel = 0.75f;
    public Vector3 distance;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 45.0f;

    private float dz = 1.0f; 

    void Start()
    {
        transform.position = distance;

        //TODO ALL ile baslasin
    }


    void LateUpdate()
    {
        // For moving on camera with left click
        if (Input.GetMouseButton(0))
        {
            Vector3 dz = Vector3.zero;
            Vector3 dy = transform.forward * Input.GetAxis("Mouse Y") * moveSpeedPerPixel * distance.y * 2;
            //Debug.Log("dy:" + dy);
            dz = new Vector3(0.0f, dy.y, 0.0f);           
            Vector3 dx = transform.right * Input.GetAxis("Mouse X") * moveSpeedPerPixel * distance.y;
            //Debug.Log("dx:" + dx);
            transform.position -= dx + dy - dz;
            
        }
        // For rotating the camera with right click
        if (Input.GetMouseButton(1))
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        // For zooming in and out
        distance.y *= (1 - Input.GetAxis("Mouse ScrollWheel") * zoomSpeedPerWheel);
        distance.y = Mathf.Clamp(distance.y, 50, 200);
        transform.position = new Vector3(transform.position.x, distance.y, transform.position.z);
    }
}
