using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class SphericalCamera : MonoBehaviour
{
    public Material shaderHolder;
    public EuclidEngineSpace EESS;

    private Vector3 oldPos;
    private Quaternion oldRot;

    public Vector4 pos4;
    private Quaternion rot4;

    void Start()
    {
        EESS = GameObject.FindObjectOfType<EuclidEngineSpace>();
        print((EESS, EESS.space));
        oldPos = transform.position;
        oldRot = transform.rotation;
        pos4 = new Vector4(0, 0, 0, EESS.radius);
        rot4 = Quaternion.identity;
    }

    void Update()
    {
        Vector3 scale = transform.lossyScale;
        if (oldRot != transform.rotation) {
            EuclidEngineSpace.EESphericalSpace_rotateObject(EESS.space,
                ref pos4, ref rot4, ref scale,
                Quaternion.RotateTowards(oldRot, transform.rotation, 180));
            oldRot = transform.rotation;
        }
        if (oldPos != transform.position) {
            EuclidEngineSpace.EESphericalSpace_moveObject(EESS.space,
                ref pos4, ref rot4, ref scale,
                transform.position - oldPos);
            Vector4 pos3 = pos4;
            Quaternion rot3 = rot4;
            
            print(String.Format("While transposing [{0:G9},{1:G9},{2:G9},{3:G9}] => {4:G9}\n", pos3.x, pos3.y, pos3.z, pos3.w, pos3.sqrMagnitude-1));
            EuclidEngineSpace.EESphericalSpace_to3D(EESS.space,
                ref pos3, ref rot3, ref scale, new Vector4(0, 0, 0, EESS.radius));
            transform.position = oldPos = pos3;
            transform.rotation = oldRot = rot3;
            
            shaderHolder.SetVector("_Camera", pos4);
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    print(transform.localRotation);
    //    Vector3 delta = transform.position - oldPos;
    //    oldPos = transform.position;
    //    position4 = new Vector4(
    //        position4.x * Mathf.Cos(delta.x) + position4.w * Mathf.Sin(delta.x),
    //        position4.y * Mathf.Cos(delta.y) + position4.w * Mathf.Sin(delta.y),
    //        position4.z * Mathf.Cos(delta.z) + position4.w * Mathf.Sin(delta.z),
    //        position4.w * Mathf.Cos(delta.x) * Mathf.Cos(delta.y) * Mathf.Cos(delta.z)
    //            - position4.x * Mathf.Sin(delta.x) - position4.y * Mathf.Sin(delta.y) - position4.z * Mathf.Sin(delta.z)
    //    );
    //}
}
