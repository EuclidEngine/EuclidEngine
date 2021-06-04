﻿using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class EuclidEngineArea : MonoBehaviour
{
    /************************************************/
    /*                                              */
    /*           C++ functions prototype            */
    /*                                              */
    /************************************************/

    [DllImport(EuclidEngine.plugin)] private static extern IntPtr EEAreaCreate(double iX, double iY, double iZ, double eX, double eY, double eZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaDelete(IntPtr area);
        private delegate void PositionGetterFn(IntPtr go, ref double x, ref double y, ref double z);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetAreaPositionGetterCallback(IntPtr area, PositionGetterFn callback);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetPositionGetterCallback(IntPtr area, PositionGetterFn callback);
        private delegate void SizeGetterFn(IntPtr go, ref double minX, ref double minY, ref double minZ, ref double maxX, ref double maxY, ref double maxZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetSizeGetterCallback(IntPtr area, SizeGetterFn callback);
        private delegate void ScalerFn(IntPtr go, double x, double y, double z);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetScalerCallback(IntPtr area, ScalerFn callback);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetSize(IntPtr area, double pExternX, double pExternY, double pExternZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetInternalSize(IntPtr area, double pInternX, double pInternY, double pInternZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetTransitAreaSize(IntPtr area, double sizeX, double sizeY, double sizeZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaAddObjectInside(IntPtr area, int objectId, IntPtr obj);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaRemoveObjectInside(IntPtr area, int objectId);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaUpdate(IntPtr area);


    /************************************************/
    /*                                              */
    /*                   Variables                  */
    /*                                              */
    /************************************************/

    // C++ Area object and callback
    private IntPtr _area;
    private PositionGetterFn _areaPosGetter;
    private PositionGetterFn _posGetter;
    private SizeGetterFn _sizeGetter;
    private ScalerFn _scaler;

    // C# Area object
    private BoxCollider _collider;

    // Area variables
    private Vector3 _size = new Vector3(1, 1, 1);
    private Vector3 _internalSize = new Vector3(1, 1, 1);
    private Vector3 _transitSize = new Vector3(0, 0, 0);


    /************************************************/
    /*                                              */
    /*             Unity events handler             */
    /*                                              */
    /************************************************/

    //Called in editor, when creating object
    private void Reset()
    {
        var boxCollider = GetComponent<BoxCollider>();
        var rigidbody = GetComponent<Rigidbody>();

        boxCollider.isTrigger = true;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    //Called on launch
    void Awake()
    {
        _areaPosGetter = new EuclidEngine.PositionGetterFn(SetAreaPosition);
        EuclidEngine.EEAreaSetAreaPositionGetterCallback(_area, _areaPosGetter);
        _posGetter = new EuclidEngine.PositionGetterFn(SetObjectPosition);
        EuclidEngine.EEAreaSetPositionGetterCallback(_area, _posGetter);
        _sizeGetter = new EuclidEngine.SizeGetterFn(SetObjectSize);
        EuclidEngine.EEAreaSetSizeGetterCallback(_area, _sizeGetter);
        _scaler = new EuclidEngine.ScalerFn(ScaleObjetct);
        EuclidEngine.EEAreaSetScalerCallback(_area, _scaler);
    }

    //Called on launch, after Awake
    void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }

    //Called at end (of object or scene)
    void OnDestroy()
    {
        EuclidEngine.EEAreaDelete(_area);
    }

    //Called on collision
    void OnTriggerEnter(Collider c)
    {
        int id = c.GetInstanceID();
        EuclidEngine.EEAreaAddObjectInside(_area, id, (IntPtr)id);
    }

    //Called at the end of collision
    void OnTriggerExit(Collider c)
    {
        EuclidEngine.EEAreaRemoveObjectInside(_area, c.GetInstanceID());
    }

    //Called every frame
    void Update()
    {
        EuclidEngine.EEAreaUpdate(_area);
    }


    /************************************************/
    /*                                              */
    /*                 Constructors                 */
    /*                                              */
    /************************************************/

    public static EuclidEngineArea Instantiate(Vector3 position = Vector3.zero, Quaternion rotation = Quaternion.identity)
    { return EuclidEngineArea.Instantiate(Vector3.one, Vector3.one, Vector3.zero, position, rotation); }
    public static EuclidEngineArea Instantiate(Vector3 position, Quaternion rotation, Transform parent)
    { return EuclidEngineArea.Instantiate(Vector3.one, Vector3.one, Vector3.zero, position, rotation, parent); }

    public static EuclidEngineArea Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 position = Vector3.zero, Quaternion rotation = Vector3.zero)
    { return EuclidEngineArea.Instantiate(areaSize, internalSize, Vector3.zero, position, rotation); }
    public static EuclidEngineArea Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 position, Quaternion rotation, Transform parent)
    { return EuclidEngineArea.Instantiate(areaSize, internalSize, Vector3.zero, position, rotation, parent); }

    public static EuclidEngineArea Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 transitionSize, Vector3 position = Vector3.zero, Quaternion rotation = Vector3.zero)
    {
        return EuclidEngineArea.Instantiate(areaSize, internalSize, transitionSize, position, rotation, null);
    }

    public static EuclidEngineArea Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 transitionSize, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject go = new GameObject(typeof(EuclidEngineArea).ToString());
        if (parent)
            go.transform.parent = parent;
        go.transform.localPosition = position;
        go.transform.localRotation = rotation;

        EuclidEngineArea area = go.AddComponent<EuclidEngineArea>();
        area.SetSize(areaSize, internalSize, transitionSize);

        return area;
    }


    /************************************************/
    /*                                              */
    /*              C# public functions             */
    /*                                              */
    /************************************************/

    public Vector3 areaSize
    {
        get { return _size; }
        set {
            if (value.x <= 0 || value.y <= 0 || value.z <= 0)
                throw new ArgumentOutOfRangeException("size", "Size must be strictly positiv"));
            _size = value;
            EEAreaSetSize(_area, (double)_size.x, (double)_size.z, (double)_size.y);
            _collider.size = _size + 2 * _transitSize;
        }
    }

    public Vector3 internalSize
    {
        get { return _internalSize; }
        set {
            if (value.x <= 0 || value.y <= 0 || value.z <= 0)
                throw new ArgumentOutOfRangeException("internalSize", "Internal size must be strictly positiv"));
            _internalSize = value;
            EEAreaSetIntenalSize(_area, (double)_internalSize.x, (double)_internalSize.z, (double)_internalSize.y);
        }
    }

    public Vector3 transitionSize
    {
        get { return _transitSize; }
        set {
            if (value.x < 0 || value.y < 0 || value.z < 0)
                throw new ArgumentOutOfRangeException("transitSize", "Transition size cannot be negativ"));
            _transitSize = value;
            EEAreaSetTransitSize(_area, (double)_transitSize.x, (double)_transitSize.z, (double)_transitSize.y);
            _collider.size = _size + 2 * _transitSize;
        }
    }

    public Vector3 size
    {
        get { return _size + 2 * _transitSize; }
    }

    public void SetSize(Vector3 areaSize, Vector3 internalSize)
    { SetSize(areaSize, internalSize, _transitSize); }
    public void SetSize(Vector3 areaSize, Vector3 internalSize, Vector3 transitionSize)
    {
        if (areaSize.x <= 0 || areaSize.y <= 0 || areaSize.z <= 0)
            throw new ArgumentOutOfRangeException("areaSize", "Size must be strictly positiv"));
        if (internalSize.x <= 0 || internalSize.y <= 0 || internalSize.z <= 0)
            throw new ArgumentOutOfRangeException("internalSize", "Internal size must be strictly positiv"));
        if (transitionSize.x < 0 || transitionSize.y < 0 || transitionSize.z < 0)
            throw new ArgumentOutOfRangeException("transitionSize", "Transition size cannot be negativ"));
        _size = areaSize;
        _internalSize = internalSize;
        _transitSize = transitionSize;
        EEAreaSetSize(_area, (double)_size.x, (double)_size.z, (double)_size.y);
        EEAreaSetIntenalSize(_area, (double)_internalSize.x, (double)_internalSize.z, (double)_internalSize.y);
        EEAreaSetTransitSize(_area, (double)_transitSize.x, (double)_transitSize.z, (double)_transitSize.y);
        _collider.size = _size + 2 * _transitSize;
    }


    /************************************************/
    /*                                              */
    /*            C# private functions              */
    /*                                              */
    /************************************************/


    /************************************************/
    /*                                              */
    /*                 C++ callbacks                */
    /*                                              */
    /************************************************/

    private void SetAreaPosition(IntPtr _, ref double x, ref double y, ref double z)
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
    }

    private void SetObjectPosition(IntPtr go, ref double x, ref double y, ref double z)
    {
        Collider collider = EuclidEngine.FindObjectFromInstanceID((int)go) as Collider;
        x = collider.transform.position.x;
        y = collider.transform.position.y;
        z = collider.transform.position.z;
    }

    private void SetObjectSize(IntPtr go, ref double minx, ref double miny, ref double minz, ref double maxx, ref double maxy, ref double maxz)
    {
        Collider collider = EuclidEngine.FindObjectFromInstanceID((int)go) as Collider;
        minx = collider.bounds.min.x;
        miny = collider.bounds.min.y;
        minz = collider.bounds.min.z;
        maxx = collider.bounds.max.x;
        maxy = collider.bounds.max.y;
        maxz = collider.bounds.max.z;
    }

    private void ScaleObjetct(IntPtr go, double x, double y, double z)
    {
        //print("Scale");
        Collider collider = EuclidEngine.FindObjectFromInstanceID((int)go) as Collider;
        collider.transform.localScale = new Vector3((float)x, (float)y, (float)z);
    }
}