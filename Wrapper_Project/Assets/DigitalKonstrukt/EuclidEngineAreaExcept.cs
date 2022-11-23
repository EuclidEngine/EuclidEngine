using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EuclidEngine
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class EuclidEngineAreaExcept : MonoBehaviour
    {
        /************************************************/
        /*                                              */
        /*           C++ functions prototype            */
        /*                                              */
        /************************************************/

        [DllImport(EuclidEngineDll.plugin)] private static extern IntPtr EEAreaCreate(double eX, double eY, double eZ, double iX, double iY, double iZ);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaDelete(IntPtr area);
        private delegate void PositionGetterFn(IntPtr go, ref double x, ref double y, ref double z);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaSetAreaPositionGetterCallback(IntPtr area, PositionGetterFn callback);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaSetPositionGetterCallback(IntPtr area, PositionGetterFn callback);
        private delegate void SizeGetterFn(IntPtr go, ref double minX, ref double minY, ref double minZ, ref double maxX, ref double maxY, ref double maxZ);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaSetSizeGetterCallback(IntPtr area, SizeGetterFn callback);
        private delegate void ScalerFn(IntPtr go, double x, double y, double z);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaSetScalerCallback(IntPtr area, ScalerFn callback);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaSetSize(IntPtr area, double pExternX, double pExternY, double pExternZ);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaSetInternalSize(IntPtr area, double pInternX, double pInternY, double pInternZ);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaSetTransitAreaSize(IntPtr area, double sizeX, double sizeY, double sizeZ);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaAddObjectInside(IntPtr area, int objectId, IntPtr obj);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaRemoveObjectInside(IntPtr area, int objectId);
        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaUpdate(IntPtr area);


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
        [SerializeField] private Vector3 _size = new Vector3(3, 3, 3);
        [SerializeField] private Vector3 _internalSize = new Vector3(6, 6, 6);
        [SerializeField] private Vector3 _transitSize = new Vector3(0, 0, 0);


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
            _area = EEAreaCreate(_size.x, _size.z, _size.y, _internalSize.x, _internalSize.z, _internalSize.y);
            EEAreaSetTransitAreaSize(_area, _transitSize.x, _transitSize.z, _transitSize.y);

            _areaPosGetter = new PositionGetterFn(SetAreaPosition);
            EEAreaSetAreaPositionGetterCallback(_area, _areaPosGetter);
            _posGetter = new PositionGetterFn(SetObjectPosition);
            EEAreaSetPositionGetterCallback(_area, _posGetter);
            _sizeGetter = new SizeGetterFn(SetObjectSize);
            EEAreaSetSizeGetterCallback(_area, _sizeGetter);
            _scaler = new ScalerFn(ScaleObjetct);
            EEAreaSetScalerCallback(_area, _scaler);
        }

        //Called on launch, after Awake
        void Start()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.size = _size + 2 * _transitSize;
        }

        //Called at end (of object or scene)
        void OnDestroy()
        {
            EEAreaDelete(_area);
        }

        //Called on collision
        void OnTriggerEnter(Collider c)
        {
            Debug.Log(c.transform.root.tag);
            Debug.Log(transform.tag);

            if (c.transform.root.CompareTag("NotEuclid") || (c.transform.root.CompareTag("Player") && !transform.CompareTag("NotEuclid")))
            {
                Debug.Log("je suis censé passé par ici");
                return;
            }


            if (c.transform.root.CompareTag("Player"))
            {
                Debug.Log("Jet suis dans le bon script");
                c.GetComponent<FPSController>().changeSpeed(1f, 1.7f);
                c.GetComponent<FPSController>().changeJumpForce(1.5f);
            }
            int id = c.GetInstanceID();
            EEAreaAddObjectInside(_area, id, (IntPtr)id);
        }

        //Called at the end of collision
        void OnTriggerExit(Collider c)
        {
            if (c.transform.root.CompareTag("NotEuclid") || (c.transform.root.CompareTag("Player") && !transform.CompareTag("NotEuclid")))
            {
                return;
            }

            Debug.Log(name + " U exited");
            if (c.transform.root.CompareTag("Player")) {
                c.GetComponent<FPSController>().changeSpeed(4, 8);
                c.GetComponent<FPSController>().changeJumpForce(8);
            }
            EEAreaRemoveObjectInside(_area, c.GetInstanceID());
        }

        //Called every frame
        void Update()
        {
            EEAreaUpdate(_area);
        }


        /************************************************/
        /*                                              */
        /*                 Constructors                 */
        /*                                              */
        /************************************************/

        public static Area Instantiate(Vector3 position, Quaternion rotation)
        { return Area.Instantiate(new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, 0f), position, rotation); }
        public static Area Instantiate(Vector3 position, Quaternion rotation, Transform parent)
        { return Area.Instantiate(new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, 0f), position, rotation, parent); }

        public static Area Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 position, Quaternion rotation)
        { return Area.Instantiate(areaSize, internalSize, new Vector3(0f, 0f, 0f), position, rotation); }
        public static Area Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 position, Quaternion rotation, Transform parent)
        { return Area.Instantiate(areaSize, internalSize, new Vector3(0f, 0f, 0f), position, rotation, parent); }

        public static Area Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 transitionSize, Vector3 position, Quaternion rotation)
        { return Area.Instantiate(areaSize, internalSize, transitionSize, position, rotation, null); }

        public static Area Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 transitionSize, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject go = new GameObject(typeof(Area).ToString());
            if (parent)
                go.transform.parent = parent;
            go.transform.localPosition = position;
            go.transform.localRotation = rotation;

            Area area = go.AddComponent<Area>();
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
            set
            {
                if (value.x <= 0 || value.y <= 0 || value.z <= 0)
                    throw new ArgumentOutOfRangeException("size", "Size must be strictly positiv");
                _size = value;
                EEAreaSetSize(_area, (double)_size.x, (double)_size.z, (double)_size.y);
                _collider.size = _size + 2 * _transitSize;
            }
        }

        public Vector3 internalSize
        {
            get { return _internalSize; }
            set
            {
                if (value.x <= 0 || value.y <= 0 || value.z <= 0)
                    throw new ArgumentOutOfRangeException("internalSize", "Internal size must be strictly positiv");
                _internalSize = value;
                EEAreaSetInternalSize(_area, (double)_internalSize.x, (double)_internalSize.z, (double)_internalSize.y);
            }
        }

        public Vector3 transitionSize
        {
            get { return _transitSize; }
            set
            {
                if (value.x < 0 || value.y < 0 || value.z < 0)
                    throw new ArgumentOutOfRangeException("transitSize", "Transition size cannot be negativ");
                _transitSize = value;
                EEAreaSetTransitAreaSize(_area, (double)_transitSize.x, (double)_transitSize.z, (double)_transitSize.y);
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
                throw new ArgumentOutOfRangeException("areaSize", "Size must be strictly positiv");
            if (internalSize.x <= 0 || internalSize.y <= 0 || internalSize.z <= 0)
                throw new ArgumentOutOfRangeException("internalSize", "Internal size must be strictly positiv");
            if (transitionSize.x < 0 || transitionSize.y < 0 || transitionSize.z < 0)
                throw new ArgumentOutOfRangeException("transitionSize", "Transition size cannot be negativ");
            _size = areaSize;
            _internalSize = internalSize;
            _transitSize = transitionSize;
            EEAreaSetSize(_area, (double)_size.x, (double)_size.z, (double)_size.y);
            EEAreaSetInternalSize(_area, (double)_internalSize.x, (double)_internalSize.z, (double)_internalSize.y);
            EEAreaSetTransitAreaSize(_area, (double)_transitSize.x, (double)_transitSize.z, (double)_transitSize.y);
            if (!_collider) _collider = GetComponent<BoxCollider>();
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
            y = transform.position.z;
            z = transform.position.y;
        }

        private void SetObjectPosition(IntPtr go, ref double x, ref double y, ref double z)
        {
            Collider collider = EuclidEngineDll.FindObjectFromInstanceID((int)go) as Collider;
            x = collider.transform.position.x;
            y = collider.transform.position.z;
            z = collider.transform.position.y;
        }

        private void SetObjectSize(IntPtr go, ref double minx, ref double miny, ref double minz, ref double maxx, ref double maxy, ref double maxz)
        {
            Collider collider = EuclidEngineDll.FindObjectFromInstanceID((int)go) as Collider;
            minx = collider.bounds.min.x;
            miny = collider.bounds.min.z;
            minz = collider.bounds.min.y;
            maxx = collider.bounds.max.x;
            maxy = collider.bounds.max.z;
            maxz = collider.bounds.max.y;
        }

        private void ScaleObjetct(IntPtr go, double x, double y, double z)
        {
            Collider collider = EuclidEngineDll.FindObjectFromInstanceID((int)go) as Collider;
            collider.transform.localScale = new Vector3((float)x, (float)z, (float)y);
            //print("x:" + x + " y:" + y + " z:" + z);
        }
    }
}