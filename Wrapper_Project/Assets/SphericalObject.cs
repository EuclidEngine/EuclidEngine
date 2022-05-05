using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalObject : MonoBehaviour
{
    public Material shaderHolder;
    public EuclidEngineSpace EESS;
    private SphericalCamera cam;

    private Vector3 oldPos;
    private Quaternion oldRot;

    private Vector4 hyperorigin;
    private Vector4 pos4;
    private Quaternion rot4;


    // Start is called before the first frame update
    void Start()
    {
        EESS = GameObject.FindObjectOfType<EuclidEngineSpace>();
        cam = GameObject.FindObjectOfType<SphericalCamera>();

        EESS.to4D(transform.position, transform.rotation, hyperorigin, ref pos4, ref rot4);
        EESS.to3D(pos4, rot4, cam, ref oldPos, ref oldRot);

        transform.position = oldPos;
        transform.rotation = oldRot;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation != oldRot)
            EESS.rotate(ref pos4, ref rot4, Quaternion.RotateTowards(oldRot, transform.rotation, 180));
        if (transform.position != oldPos)
            EESS.move(ref pos4, ref rot4, transform.position - oldPos);
        if (transform.rotation != oldRot || transform.position != oldPos) {
            //print(String.Format("While transposing [{0:G9},{1:G9},{2:G9},{3:G9}] => {4:G9}", oldPos.x, oldPos.y, oldPos.z, pos3.sqrMagnitude-1));
            EESS.to3D(pos4, rot4, cam, ref oldPos, ref oldRot);
            transform.position = oldPos;
            transform.rotation = oldRot;
        }
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
}
