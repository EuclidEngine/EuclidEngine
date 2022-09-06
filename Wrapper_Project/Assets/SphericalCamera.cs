using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SphericalCamera : MonoBehaviour
{
    public Material shaderHolder;

    private Vector3 oldPos;
    private Vector4 position4;
    private float worldRadius;

    private IntPtr _gv;
    public IntPtr gyroVector { get => _gv; }

    // Start is called before the first frame update
    void Start()
    {
        worldRadius = FindObjectOfType<EuclidEngineSpace>().worldRadius;
        //sObj = GetComponent<SphericalObject>();
        Vector3 pos = transform.position; Quaternion rot = transform.rotation;
        _gv = SphericalObject.new_GyroVector(in pos, in rot);
        if (SphericalObject.GyroVector_toMatrix(_gv, out Matrix4x4 m))
            Shader.SetGlobalMatrix("_CGyrVec", m);
        else
            print("Failed to set global matrix");
        //oldPos = transform.position;
        //position4 = new Vector4(oldPos.x, oldPos.y, oldPos.z, 1f);
        //position4 = new Vector4(0,0,0,-1f) + (-2*Vector4.Dot(new Vector4(oldPos.x, oldPos.y, oldPos.z, 2f), new Vector4(0,0,0,-1f))/Vector4.Dot(new Vector4(oldPos.x, oldPos.y, oldPos.z, 2f),new Vector4(oldPos.x, oldPos.y, oldPos.z, 2f))) * new Vector4(oldPos.x, oldPos.y, oldPos.z, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = Vector3.zero;
        Vector3 q = Vector3.zero;
        
        if (Input.GetKey(KeyCode.UpArrow))
            v.z += 1;
        if (Input.GetKey(KeyCode.DownArrow))
            v.z -= 1;
        if (Input.GetKey(KeyCode.RightArrow))
            v.x += 1;
        if (Input.GetKey(KeyCode.LeftArrow))
            v.x -= 1;
        if (Input.GetKey(KeyCode.PageUp))
            v.y += 1;
        if (Input.GetKey(KeyCode.PageDown))
            v.y -= 1;
         
        if (Input.GetKey(KeyCode.Z))
            transform.LookAt(transform.position + transform.forward + Vector3.up*Time.deltaTime);
            //q.x -= 90;
        if (Input.GetKey(KeyCode.S))
            transform.LookAt(transform.position + transform.forward + Vector3.down*Time.deltaTime);
            //q.x += 90;
        if (Input.GetKey(KeyCode.Q))
            transform.LookAt(transform.position + transform.forward - transform.right*Time.deltaTime);
            //q.y += 90;
        if (Input.GetKey(KeyCode.D))
            transform.LookAt(transform.position + transform.forward + transform.right*Time.deltaTime);
            //q.y -= 90;

        if (v.sqrMagnitude > 0) {
            v = v.normalized / 2.0f / worldRadius;
            v *= Time.deltaTime;
            Vector3 tmp = v;
            //v *= Mathf.Tan(v.magnitude) / v.magnitude;
            SphericalObject.GyroVector_move(_gv, in v);
            //SphericalObject.GyroVector_alignUp(_gv);
            SphericalObject.GyroVector_toMatrix(_gv, out Matrix4x4 m);
            //print(m);
        }
        // if (q.sqrMagnitude > 0) {
        //     SphericalObject.GyroVector_rotate(_gv, in q2);
        // }

        //Vector3 delta = transform.position - oldPos;
        //oldPos = transform.position;
        //position4 = new Vector4(
        //    position4.x * Mathf.Cos(delta.x) + position4.w * Mathf.Sin(delta.x),
        //    position4.y * Mathf.Cos(delta.y) + position4.w * Mathf.Sin(delta.y),
        //    position4.z * Mathf.Cos(delta.z) + position4.w * Mathf.Sin(delta.z),
        //    position4.w * Mathf.Cos(delta.x) * Mathf.Cos(delta.y) * Mathf.Cos(delta.z)
        //        - position4.x * Mathf.Sin(delta.x) - position4.y * Mathf.Sin(delta.y) - position4.z * Mathf.Sin(delta.z)
        //);
        //shaderHolder.SetVector("_Camera", position4);
    }
}
