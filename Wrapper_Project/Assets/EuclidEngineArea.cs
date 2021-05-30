using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class EuclidEngineArea : MonoBehaviour
{
    public IntPtr _area;
    private EuclidEngine.PositionGetterFn _areaPosGetter;
    private EuclidEngine.PositionGetterFn _posGetter;
    private EuclidEngine.SizeGetterFn _sizeGetter;
    private EuclidEngine.ScalerFn _scaler;

    void Awake()
    {
        print("Awake" + GetInstanceID().ToString());
        _area = EuclidEngine.EEAreaCreate(18, 18, 18, 20, 10, 5);
        EuclidEngine.EEAreaSetTransitAreaSize(_area, 0, 0, 0);

        _areaPosGetter = new EuclidEngine.PositionGetterFn(SetAreaPosition);
        EuclidEngine.EEAreaSetAreaPositionGetterCallback(_area, _areaPosGetter);
        _posGetter = new EuclidEngine.PositionGetterFn(SetObjectPosition);
        EuclidEngine.EEAreaSetPositionGetterCallback(_area, _posGetter);
        _sizeGetter = new EuclidEngine.SizeGetterFn(SetObjectSize);
        EuclidEngine.EEAreaSetSizeGetterCallback(_area, _sizeGetter);
        _scaler = new EuclidEngine.ScalerFn(ScaleObjetct);
        EuclidEngine.EEAreaSetScalerCallback(_area, _scaler);
    }

    void OnDestroy()
    {
        EuclidEngine.EEAreaDelete(_area);
    }

    void OnTriggerEnter(Collider c)
    {
        int id = c.GetInstanceID();
        EuclidEngine.EEAreaAddObjectInside(_area, id, (IntPtr)id);
    }

    void OnTriggerExit(Collider c)
    {
        EuclidEngine.EEAreaRemoveObjectInside(_area, c.GetInstanceID());
    }

    void Update()
    {
        EuclidEngine.EEAreaUpdate(_area);
    }

    void SetAreaPosition(IntPtr _, ref double x, ref double y, ref double z)
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
    }

    void SetObjectPosition(IntPtr go, ref double x, ref double y, ref double z)
    {
        Collider collider = EuclidEngine.FindObjectFromInstanceID((int)go) as Collider;
        x = collider.transform.position.x;
        y = collider.transform.position.y;
        z = collider.transform.position.z;
    }

    void SetObjectSize(IntPtr go, ref double minx, ref double miny, ref double minz, ref double maxx, ref double maxy, ref double maxz)
    {
        Collider collider = EuclidEngine.FindObjectFromInstanceID((int)go) as Collider;
        minx = collider.bounds.min.x;
        miny = collider.bounds.min.y;
        minz = collider.bounds.min.z;
        maxx = collider.bounds.max.x;
        maxy = collider.bounds.max.y;
        maxz = collider.bounds.max.z;
    }

    void ScaleObjetct(IntPtr go, double x, double y, double z)
    {
        //print("Scale");
        Collider collider = EuclidEngine.FindObjectFromInstanceID((int)go) as Collider;
        collider.transform.localScale = new Vector3((float)x, (float)y, (float)z);
    }

    public void SetTransitAreaSize(double sizeX, double sizeY, double sizeZ)
    {
        EuclidEngine.EEAreaSetTransitAreaSize(_area, sizeX, sizeY, sizeZ);
    }

    public void AddObjectInside(int objectId, EuclidEngineArea area)
    {
        EuclidEngine.EEAreaAddObjectInside(_area, objectId, area._area);
    }

    public void RemoveObjectInside(int objectId)
    {
        EuclidEngine.EEAreaRemoveObjectInside(_area, objectId);
    }

    public void AreaUpdate()
    {
        EuclidEngine.EEAreaUpdate(_area);
    }
}