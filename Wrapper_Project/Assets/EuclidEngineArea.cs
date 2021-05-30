using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class EuclidEngine
{
    public class Area : MonoBehaviour
    {
        public IntPtr _area;
        private PositionGetterFn _areaPosGetter;
        private PositionGetterFn _posGetter;
        private SizeGetterFn _sizeGetter;
        private ScalerFn _scaler;

        void Awake()
        {
            print("Awake" + GetInstanceID().ToString());
            _area = EEAreaCreate(18, 18, 18, 20, 10, 5);
            EEAreaSetTransitAreaSize(_area, 0, 0, 0);

            _areaPosGetter = new PositionGetterFn(SetAreaPosition);
            EEAreaSetAreaPositionGetterCallback(_area, _areaPosGetter);
            _posGetter = new PositionGetterFn(SetObjectPosition);
            EEAreaSetPositionGetterCallback(_area, _posGetter);
            _sizeGetter = new SizeGetterFn(SetObjectSize);
            EEAreaSetSizeGetterCallback(_area, _sizeGetter);
            _scaler = new ScalerFn(ScaleObjetct);
            EEAreaSetScalerCallback(_area, _scaler);
        }

        void OnDestroy()
        {
            EEAreaDelete(_area);
        }

        void OnTriggerEnter(Collider c)
        {
            int id = c.GetInstanceID();
            EEAreaAddObjectInside(_area, id, (IntPtr)id);
        }

        void OnTriggerExit(Collider c)
        {
            EEAreaRemoveObjectInside(_area, c.GetInstanceID());
        }

        void Update()
        {
            EEAreaUpdate(_area);
        }

        void SetAreaPosition(IntPtr _, ref double x, ref double y, ref double z)
        {
            x = transform.position.x;
            y = transform.position.y;
            z = transform.position.z;
        }

        void SetObjectPosition(IntPtr go, ref double x, ref double y, ref double z)
        {
            Collider collider = FindObjectFromInstanceID((int)go) as Collider;
            x = collider.transform.position.x;
            y = collider.transform.position.y;
            z = collider.transform.position.z;
        }

        void SetObjectSize(IntPtr go, ref double minx, ref double miny, ref double minz, ref double maxx, ref double maxy, ref double maxz)
        {
            Collider collider = FindObjectFromInstanceID((int)go) as Collider;
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
            Collider collider = FindObjectFromInstanceID((int)go) as Collider;
            collider.transform.localScale = new Vector3((float)x, (float)y, (float)z);
        }

        public void SetTransitAreaSize(double sizeX, double sizeY, double sizeZ)
        {
            EEAreaSetTransitAreaSize(_area, sizeX, sizeY, sizeZ);
        }

        public void AddObjectInside(int objectId, Area area)
        {
            EEAreaAddObjectInside(_area, objectId, area._area);
        }

        public void RemoveObjectInside(int objectId)
        {
            EEAreaRemoveObjectInside(_area, objectId);
        }

        public void AreaUpdate()
        {
            EEAreaUpdate(_area);
        }
    }
}