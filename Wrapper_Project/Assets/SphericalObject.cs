using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[DisallowMultipleComponent]
public class SphericalObject : MonoBehaviour
{
    public const string plugin = "mobius5";

    [DllImport("mobius3")] private static extern bool Mobius_addVec(in Vector3 pos, in Quaternion rot, in Vector3 vec, out Vector3 retPos, out Quaternion retRot);
    [DllImport("mobius3")] private static extern bool Mobius_addQuat(in Vector3 pos, in Quaternion rot, in Quaternion vec, out Vector3 retPos, out Quaternion retRot);

    [DllImport(plugin)] public static extern IntPtr new_GyroVector(in Vector3 pos, in Quaternion rot);
    [DllImport(plugin)] public static extern void delete_GyroVector(IntPtr gv);
    [DllImport(plugin)] public static extern bool GyroVector_toMatrix(IntPtr gv, out Matrix4x4 mat);
    [DllImport(plugin)] public static extern bool GyroVector_move(IntPtr gv, in Vector3 vec);
    [DllImport(plugin)] public static extern bool GyroVector_rotate(IntPtr gv, in Quaternion rot);
    [DllImport(plugin)] public static extern bool GyroVector_alignUp(IntPtr gv);
    [DllImport(plugin)] public static extern IntPtr GyroVector_combine(IntPtr gv, IntPtr other); // Gyrovector addition

    private Vector3 pos = new Vector3();
    private Quaternion rot = new Quaternion(0,0,0,1);
    public Material shaderHolder;
    private IntPtr _gv;
    SphericalCamera c;
    private float worldRadius;
    
    new private SphericalCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        worldRadius = GetComponentInParent<EuclidEngineSpace>().worldRadius;
        c = FindObjectOfType<SphericalCamera>();
        collider = gameObject.AddComponent<SphericalCollider>();

        if (c.gameObject == gameObject) {
            _gv = c.gyroVector;
        } else {
            SphericalShunk shunk = GetComponentInParent<SphericalShunk>();
            Quaternion shunkRot = Quaternion.identity;//shunk.transform.rotation;

            pos = transform.position / worldRadius;// / 30.0f;
            IntPtr shunkGv = new_GyroVector(in shunk.gyroPos, in shunkRot);
            IntPtr localGv = new_GyroVector(in pos, in rot);
            _gv = GyroVector_combine(shunkGv, localGv);
            delete_GyroVector(shunkGv);
            delete_GyroVector(localGv);
        }
        pos = transform.position;
        if (GyroVector_toMatrix(_gv, out Matrix4x4 m)) {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            List<Renderer> rs = new List<Renderer>(GetComponents<Renderer>());
            rs.AddRange(GetComponentsInChildren<Renderer>());
            foreach (Renderer r in rs) {
                r.GetPropertyBlock(propBlock);
                propBlock.SetMatrix("_GyrVec", m);
                r.SetPropertyBlock(propBlock);
            }
        } else
            print("Error on matrix conversion");
    }

    void OnDestroy()
    {
        if (c.gameObject != gameObject)
            delete_GyroVector(_gv);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation != rot) {
            Quaternion d = transform.rotation * Quaternion.Inverse(rot);
            if (!GyroVector_rotate(_gv, in d))
                print("Error on rot");
            rot = transform.rotation;
        }
        if (transform.position != pos) {
            Vector3 d = (transform.position - pos) / worldRadius;
            if (!GyroVector_move(_gv, in d))
                print("Error on move");
            pos = transform.position;
        }

        IntPtr gv2 = GyroVector_combine(_gv, c.gyroVector);
        if (GyroVector_toMatrix(gv2, out Matrix4x4 m)) {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            Renderer[] rs = GetComponents<Renderer>();
            Renderer[] r2 = GetComponentsInChildren<Renderer>();
            Array.Resize(ref rs, r2.Length);
            Array.Copy(r2, 0, rs, rs.Length - r2.Length, r2.Length);
            foreach (Renderer r in rs) {
                r.GetPropertyBlock(propBlock);
                propBlock.SetMatrix("_GyrVec", m);
                r.SetPropertyBlock(propBlock);
            }
        } else
            print("Error on matrix conversion");
        // if (GyroVector_toMatrix(c.gyroVector, out m))
        //     print(m);
        // print(c.GetComponent<Camera>().worldToCameraMatrix);
        delete_GyroVector(gv2);
        /*if (transform.rotation != rot) {
            Quaternion d = transform.rotation * Quaternion.Inverse(rot);
            if (!Mobius_addQuat(in pos, in rot, in d, out Vector3 v, out Quaternion q))
                print("Error on rot");
            pos = v;
            rot = q;
        }
        if (transform.position != pos) {
            Vector3 d = transform.position - pos;
            Mobius_addVec(in pos, in rot, in d, out Vector3 v, out Quaternion q);
            pos = v;
            rot = q;
        }

        if (transform.position != pos || transform.rotation != rot) {
            transform.position = pos;
            transform.rotation = rot;
            pos = transform.position;
            rot = transform.rotation;
            if (shaderHolder)
                shaderHolder.SetMatrix("_GyroVectorMat", Matrix4x4.TRS(pos, rot, Vector3.one));
        }*/
    }

    public bool CheckCollision(SphericalObject other)
    {
        return collider.CheckCollision(_gv, other._gv, other.collider);
    }

    public void OnTriggerEnter(Collider obj)
    {
        print(("Collision", name, obj.name));
    }
}
