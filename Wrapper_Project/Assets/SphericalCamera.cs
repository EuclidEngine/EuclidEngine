using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalCamera : MonoBehaviour
{
    public Material shaderHolder;

    // Start is called before the first frame update
    void Start()
    {
        /*const*/ Vector4 pos = transform.position; pos.w = 1;
        /*const*/ Vector4 hyperorigin = pos;
        /*const*/ Vector4 sphereCenter = new Vector4(0,0,0,0);
        /*const*/ Vector4 planeNormal = hyperorigin - sphereCenter;
        /*const*/ float sphereRadius = 1;

        Vector4 projPoint = sphereCenter - planeNormal.normalized * sphereRadius;
        Vector4 QP = pos - projPoint;
        Vector4 SQ = projPoint - sphereCenter;
        float A = Vector4.Dot(QP, QP);
        float B = 2 * Vector4.Dot(QP, SQ);
        float k = - B / A;
        //print(("Camera: ", pos, projPoint, k, "->", projPoint + k * QP));
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector4 pos = transform.position; pos.w = 1;
        //
        shaderHolder.SetVector("_Camera", pos);
    }
}
