using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Vector4 hyperorigin;
    // Update is called once per frame
    void Update()
    {

    }

    public void SetHyperOrigin(Vector4 _hyperorigin)
    {
        hyperorigin = _hyperorigin;
        Renderer rend = GetComponent<Renderer>();
        MaterialPropertyBlock matProp = new MaterialPropertyBlock();

        rend.GetPropertyBlock(matProp);
        matProp.SetVector("_TileOrigin", hyperorigin);
        rend.SetPropertyBlock(matProp);
    }

    // Old version, slightly off. Not used anymore
    private Vector4 StereographicProjection()
    {
        /*const*/ Vector4 pos = transform.position; pos.w = 1;
        /*const*/ Vector4 sphereCenter = new Vector4(0,0,0,0);
        /*const*/ Vector4 planeNormal = hyperorigin - sphereCenter;
        /*const*/ float sphereRadius = 5;

        /*const*/ Vector4 calcSphereCenter = new Vector4(0,0,0,sphereRadius);

        Vector4 Q = calcSphereCenter + new Vector4(0,0,0,1) * sphereRadius;
        Vector4 QP = pos - Q;
        Vector4 SQ = Q - calcSphereCenter;
        float A = Vector4.Dot(QP, QP);
        float B = 2 * Vector4.Dot(QP, SQ);
        float k = -B / A;

        Vector4 projPoint = sphereCenter - (planeNormal.normalized * sphereRadius - sphereCenter);
        return projPoint + k * QP;
    }

    private Vector4 ReverseStereographicProjection()
    {
        /*const*/ Vector4 pos = transform.position; pos.w = 1;
        /*const*/ Vector4 sphereCenter = new Vector4(0,0,0,0);
        /*const*/ Vector4 planeNormal = hyperorigin - sphereCenter;
        /*const*/ float sphereRadius = 5;

        /*const*/ Vector4 calcSphereCenter = new Vector4(0,0,0,sphereRadius);

        Vector4 Q = calcSphereCenter + new Vector4(0,0,0,1) * sphereRadius;
        Vector4 QP = pos - Q;
        Vector4 SQ = Q - calcSphereCenter;
        float A = Vector4.Dot(QP, QP);
        float B = 2 * Vector4.Dot(QP, SQ);
        float k = -B / A;

        Vector4 projPoint = sphereCenter - (planeNormal.normalized * sphereRadius - sphereCenter);
        Vector4 rotVec1 = SQ;
        Vector4 rotVec2 = projPoint - sphereCenter;
        Matrix4x4 rotMat = Matrix4x4.identity;

        if ((rotVec1.x == 0 && rotVec1.y == 0 && rotVec2.x == 0 && rotVec2.y == 0) ||
            ((rotVec1.x != 0 || rotVec1.y != 0) && (rotVec2.x != 0 || rotVec2.y != 0))) {
                float angle1 = Vector2.SignedAngle(new Vector2(rotVec1.x, rotVec1.y), new Vector2(rotVec2.x, rotVec2.y)) * Mathf.Deg2Rad;
                float angle2 = Vector2.SignedAngle(new Vector2(rotVec1.z, rotVec1.w), new Vector2(rotVec2.z, rotVec2.w)) * Mathf.Deg2Rad;
                rotMat[0,0] = Mathf.Cos(angle1); rotMat[1,1] = Mathf.Cos(angle1);
                rotMat[0,1] =-Mathf.Sin(angle1); rotMat[1,0] = Mathf.Sin(angle1);
                rotMat[2,2] = Mathf.Cos(angle2); rotMat[3,3] = Mathf.Cos(angle2);
                rotMat[2,3] =-Mathf.Sin(angle2); rotMat[3,2] = Mathf.Sin(angle2);
        } else if ((rotVec1.x == 0 && rotVec1.z == 0 && rotVec2.x == 0 && rotVec2.z == 0) ||
            ((rotVec1.x != 0 || rotVec1.z != 0) && (rotVec2.x != 0 || rotVec2.z != 0))) {
                float angle1 = Vector2.SignedAngle(new Vector2(rotVec1.x, rotVec1.z), new Vector2(rotVec2.x, rotVec2.z)) * Mathf.Deg2Rad;
                float angle2 = Vector2.SignedAngle(new Vector2(rotVec1.y, rotVec1.w), new Vector2(rotVec2.y, rotVec2.w)) * Mathf.Deg2Rad;
                rotMat[0,0] = Mathf.Cos(angle1); rotMat[2,2] = Mathf.Cos(angle1);
                rotMat[0,2] =-Mathf.Sin(angle1); rotMat[2,0] = Mathf.Sin(angle1);
                rotMat[1,1] = Mathf.Cos(angle2); rotMat[3,3] = Mathf.Cos(angle2);
                rotMat[1,3] =-Mathf.Sin(angle2); rotMat[3,1] = Mathf.Sin(angle2);
        } else if ((rotVec1.x == 0 && rotVec1.w == 0 && rotVec2.x == 0 && rotVec2.w == 0) ||
            ((rotVec1.x != 0 || rotVec1.w != 0) && (rotVec2.x != 0 || rotVec2.w != 0))) {
                float angle1 = Vector2.SignedAngle(new Vector2(rotVec1.x, rotVec1.w), new Vector2(rotVec2.x, rotVec2.w)) * Mathf.Deg2Rad;
                float angle2 = Vector2.SignedAngle(new Vector2(rotVec1.y, rotVec1.z), new Vector2(rotVec2.y, rotVec2.z)) * Mathf.Deg2Rad;
                rotMat[0,0] = Mathf.Cos(angle1); rotMat[3,3] = Mathf.Cos(angle1);
                rotMat[0,3] =-Mathf.Sin(angle1); rotMat[3,0] = Mathf.Sin(angle1);
                rotMat[1,1] = Mathf.Cos(angle2); rotMat[2,2] = Mathf.Cos(angle2);
                rotMat[1,2] =-Mathf.Sin(angle2); rotMat[2,1] = Mathf.Sin(angle2);
        }
        return rotMat * (SQ + k * QP) + sphereCenter;
    }
}
