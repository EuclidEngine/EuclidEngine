using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
#if UNITY_IOS
    private const string eePlugin = "__Internal";
#else
    private const string eePlugin = "EEPluginCpp";
#endif

    [DllImport(eePlugin)] private static extern IntPtr EEAreaCreate(double iX, double iY, double iZ, double eX, double eY, double eZ);
    [DllImport(eePlugin)] private static extern void EEAreaDelete(IntPtr area);
    [DllImport(eePlugin)] private static extern void EEAreaSetInitialObjectInside(IntPtr area, int objCount, IntPtr objs);
    [DllImport(eePlugin)] private static extern void EEAreaAddObjectInside(IntPtr area, int obj);
    [DllImport(eePlugin)] private static extern void EEAreaRemoveObjectInside(IntPtr area, int obj);

    [DllImport(eePlugin)]
    private static extern int EEAreaNbInternObject(IntPtr area);

    private IntPtr _area;
    private bool firstUpdate = true;
    private List<int> objectIds = new List<int>();

    void Awake()
    {
        print("Awake" + GetInstanceID().ToString());
        _area = EEAreaCreate(10, 10, 10, 20, 20, 20);
    }

    void LateUpdate()
    {
        if (firstUpdate) {
            firstUpdate = false;
            GCHandle buff = GCHandle.Alloc(objectIds.ToArray(), GCHandleType.Pinned);
            try { EEAreaSetInitialObjectInside(_area, objectIds.Count, buff.AddrOfPinnedObject()); }
            finally { buff.Free(); }
        }
        print(EEAreaNbInternObject(_area));
    }

    void OnTriggerEnter(Collider c)
    {
        if (firstUpdate)
            objectIds.Add(c.GetInstanceID());
        else
            EEAreaAddObjectInside(_area, c.GetInstanceID());
    }

    void OnTriggerExit(Collider c)
    {
        EEAreaRemoveObjectInside(_area, c.GetInstanceID());
    }

    void OnDestroy()
    {
        print("OnDestroy");
        EEAreaDelete(_area);
    }
}