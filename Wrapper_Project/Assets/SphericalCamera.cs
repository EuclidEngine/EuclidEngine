using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalCamera : MonoBehaviour
{
    public Material shaderHolder;

    private Vector3 oldPos;
    private Vector4 position4;

    // Start is called before the first frame update
    void Start()
    {
        oldPos = transform.position;
        position4 = new Vector4(oldPos.x, oldPos.y, oldPos.z, 1f);
        position4 = new Vector4(0,0,0,-1f) + (-2*Vector4.Dot(new Vector4(oldPos.x, oldPos.y, oldPos.z, 2f), new Vector4(0,0,0,-1f))/Vector4.Dot(new Vector4(oldPos.x, oldPos.y, oldPos.z, 2f),new Vector4(oldPos.x, oldPos.y, oldPos.z, 2f))) * new Vector4(oldPos.x, oldPos.y, oldPos.z, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = transform.position - oldPos;
        oldPos = transform.position;
        position4 = new Vector4(
            position4.x * Mathf.Cos(delta.x) + position4.w * Mathf.Sin(delta.x),
            position4.y * Mathf.Cos(delta.y) + position4.w * Mathf.Sin(delta.y),
            position4.z * Mathf.Cos(delta.z) + position4.w * Mathf.Sin(delta.z),
            position4.w * Mathf.Cos(delta.x) * Mathf.Cos(delta.y) * Mathf.Cos(delta.z)
                - position4.x * Mathf.Sin(delta.x) - position4.y * Mathf.Sin(delta.y) - position4.z * Mathf.Sin(delta.z)
        );
        shaderHolder.SetVector("_Camera", position4);
    }
}
