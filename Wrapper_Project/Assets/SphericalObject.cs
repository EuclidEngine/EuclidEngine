using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetHyperOrigin(Vector4 hyperorigin)
    {
        Renderer rend = GetComponent<Renderer>();
        MaterialPropertyBlock matProp = new MaterialPropertyBlock();

        rend.GetPropertyBlock(matProp);
        matProp.SetVector("_TileOrigin", hyperorigin);
        rend.SetPropertyBlock(matProp);



        /*const*/ Vector4 pos = transform.position; pos.w = 1;
        /*const*/ Vector4 sphereCenter = new Vector4(0,0,0,0);
        /*const*/ Vector4 planeNormal = hyperorigin - sphereCenter;
        /*const*/ float sphereRadius = 1;

        Vector4 projPoint = sphereCenter - planeNormal.normalized * sphereRadius;
        Vector4 QP = pos - projPoint;
        Vector4 SQ = projPoint - sphereCenter;//length() = radius
        float A = Vector4.Dot(QP, QP);
        float B = 2 * Vector4.Dot(QP, SQ);
        float k = -B / A;
        //print((sphereCenter, pos, k, "\n->", projPoint + k * QP));
    }
}
