using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[DisallowMultipleComponent]
public class EuclidEngineSpace : MonoBehaviour
{
    public GameObject[] prefabs = new GameObject[6];
    public float[] prefRot = new float[6];
    public float worldRadius = 1f;
    public GameObject mainPlayer;

    private List<SphericalObject> objects = new List<SphericalObject>();

    private Vector3[] shunkPos = {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(0,0,1),
        new Vector3(-1,0,0),
        new Vector3(0,0,-1),
        new Vector3(1e18f,0,0)
    };

    void Awake()
    {
        if (!mainPlayer || worldRadius <= 0 || prefabs.Length != 6/* || Array.Exists(prefabs, o => !o)*/)
            throw new System.Exception();

        Collider[] objs;
        for (int i = 0; i < 6; ++i) {
            var shunk = Instantiate(prefabs[i] ? prefabs[i] : new GameObject(), transform);
            shunk.name = String.Format("Shunk {0}",i);
            if (i == 0) {
                mainPlayer.transform.parent = shunk.transform;
                Camera mainCamera = mainPlayer.GetComponentInChildren<Camera>();
                mainCamera.gameObject.AddComponent<SphericalCamera>();
                mainCamera.cullingMatrix = Matrix4x4.Ortho(-100.0f, 100.0f, -100.0f, 100.0f, -100.0f, 100.0f);
                objects.Add(mainPlayer.AddComponent<SphericalController>());
                objs = mainPlayer.GetComponentsInChildren<Collider>();
                foreach (Collider obj in objs) {
                    if (obj.gameObject.name == mainPlayer.name) continue;
                    objects.Add(obj.gameObject.AddComponent<SphericalObject>());
                }
            }
            shunk.AddComponent<SphericalShunk>().gyroPos = shunkPos[i];
            shunk.transform.localEulerAngles = new Vector3(0, prefRot[i], 0);
            objs = shunk.GetComponentsInChildren<Collider>();
            foreach (Collider obj in objs) {
                if (obj.gameObject.name == mainPlayer.name) continue;
                objects.Add(obj.gameObject.AddComponent<SphericalObject>());
            }
        }
        Shader.SetGlobalFloat("_EEWorldRadius", worldRadius);
    }

    void Update()
    {
        var cEnum = objects.GetEnumerator();
        while (cEnum.MoveNext()) {
            var enum2 = cEnum;
            if (!cEnum.Current) continue;
            while (enum2.MoveNext()) {
                if (enum2.Current && cEnum.Current.CheckCollision(enum2.Current)) {
                    cEnum.Current.OnTriggerEnter(enum2.Current.GetComponent<Collider>());
                    enum2.Current.OnTriggerEnter(cEnum.Current.GetComponent<Collider>());
                }
                //print((cEnum.Current, enum2.Current));
            }
        }
        cEnum.Dispose();
    }

    /*[DllImport(EuclidEngine.plugin)] private static extern IntPtr new_EESphericalSpace(float radius, Vector4 center);
    [DllImport(EuclidEngine.plugin)] private static extern void delete_EESphericalSpace(IntPtr space);
    [DllImport(EuclidEngine.plugin)] private static extern void EESphericalSpace_to4D(IntPtr space, ref Vector4 pos, ref Quaternion rot, ref Vector3 scale, Vector4 refPos);
    [DllImport(EuclidEngine.plugin)] public static extern void EESphericalSpace_to3D(IntPtr space, ref Vector4 pos, ref Quaternion rot, ref Vector3 scale, Vector4 refPos);
    [DllImport(EuclidEngine.plugin)] public static extern void EESphericalSpace_rotateObject(IntPtr space, ref Vector4 pos, ref Quaternion rot, ref Vector3 scale, Quaternion orient);
    [DllImport(EuclidEngine.plugin)] public static extern void EESphericalSpace_moveObject(IntPtr space, ref Vector4 pos, ref Quaternion rot, ref Vector3 scale, Vector3 dir);

    private IntPtr _space;
    public IntPtr space { get { return _space; } }
    public float radius = 1;
    public Vector4 center = new Vector4(0,0,0,0);

    void Awake()
    {
        _space = new_EESphericalSpace(radius, center);
    }    

    void OnDestroy()
    {
        delete_EESphericalSpace(_space);
    }

    public void to4D(Vector3 pos3, Quaternion rot3, Vector4 hyperOrigin, ref Vector4 pos4, ref Quaternion rot4)
    {
        Vector4 _p = pos3;
        Quaternion _r = rot3;
        Vector3 _s = Vector3.one;
        EESphericalSpace_to4D(_space, ref _p, ref _r, ref _s, hyperOrigin);
        pos4 = _p;
        rot4 = _r;
    }

    public void to3D(Vector4 pos4, Quaternion rot4, SphericalCamera viewPoint, ref Vector3 pos3, ref Quaternion rot3)
    {
        Vector4 _p = pos4;
        Quaternion _r = rot4;
        Vector3 _s = Vector3.one;
        //EESphericalSpace_to3D(_space, ref _p, ref _r, ref _s, viewPoint.pos4);
        print("Missing operation");
        pos3 = _p;
        rot3 = _r;
    }

    public void rotate(ref Vector4 pos4, ref Quaternion rot4, Quaternion rotation)
    {
        Vector3 _s = Vector3.one;
        EESphericalSpace_rotateObject(_space, ref pos4, ref rot4, ref _s, rotation);
    }

    public void move(ref Vector4 pos4, ref Quaternion rot4, Vector3 vec)
    {
        Vector3 _s = Vector3.one;
        EESphericalSpace_moveObject(_space, ref pos4, ref rot4, ref _s, vec);
    }*/
}