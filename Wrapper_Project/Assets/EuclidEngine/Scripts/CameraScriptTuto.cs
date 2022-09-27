using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScriptTuto : MonoBehaviour
{
    public float speedH = 0.2f;
    public float speedV = 0.2f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
