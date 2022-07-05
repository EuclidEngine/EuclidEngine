using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[AddComponentMenu("Euclid Engine/Area")]
[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]

/// @brief The Non-Euclidean Area (NEA) object, deserved by Euclid Engine
///
/// @details A NEA is a small space inside another space. It may have different dimensions than the oiginal space.
///
/// This is defined with a different external and internal size. The external size is the size of the Area, defining the space it will occuped on the world. The internal size is the size of the space inside the Area, defining the distance to cross the Area from both end.
///
/// Every object inside a NEA are anchored relatively to their internal position and the center of the area. 
///
/// By default, an EuclidEngineArea as an external and internal size of 1, with no transition area.
///
/// @ingroup cs
public class EuclidEngineArea2 : MonoBehaviour
{
#region C++ functions
    /************************************************/
    /*                                              */
    /*           C++ functions prototype            */
    /*                                              */
    /************************************************/

    [DllImport(EuclidEngine.plugin)] private static extern IntPtr EEAreaCreate(double eX, double eY, double eZ, double iX, double iY, double iZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaDelete(IntPtr area);
        private delegate void PositionGetterFn(IntPtr go, ref double x, ref double y, ref double z);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetAreaPositionGetterCallback(IntPtr area, PositionGetterFn callback);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetPositionGetterCallback(IntPtr area, PositionGetterFn callback);
        private delegate void SizeGetterFn(IntPtr go, ref double minX, ref double minY, ref double minZ, ref double maxX, ref double maxY, ref double maxZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetSizeGetterCallback(IntPtr area, SizeGetterFn callback);
        private delegate void ScalerFn(IntPtr go, double x, double y, double z);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetScalerCallback(IntPtr area, ScalerFn callback);
    [DllImport(EuclidEngine.plugin)] protected static extern void EEAreaSetCameraPosition(IntPtr area, Vector3 position);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetSize(IntPtr area, double pExternX, double pExternY, double pExternZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetInternalSize(IntPtr area, double pInternX, double pInternY, double pInternZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetTransitAreaSize(IntPtr area, double sizeX, double sizeY, double sizeZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaAddObjectInside(IntPtr area, int objectId, IntPtr obj);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaRemoveObjectInside(IntPtr area, int objectId);
    [DllImport(EuclidEngine.plugin)] protected static extern void EEAreaGetTransformMatrix(IntPtr area, Vector3 dir, out Matrix4x4 ret);
    [DllImport(EuclidEngine.plugin)] protected static extern void EEAreaUpdate(IntPtr area);

#endregion
#region Variables
    /************************************************/
    /*                                              */
    /*                   Variables                  */
    /*                                              */
    /************************************************/

    // C++ Area object and callback
    protected IntPtr _area;
    private PositionGetterFn _areaPosGetter;
    private PositionGetterFn _posGetter;
    private SizeGetterFn _sizeGetter;
    private ScalerFn _scaler;

    // C# Area object
    protected BoxCollider _collider;
    protected Camera _camera;
    protected EEAreaPlane _planeRight;
    protected EEAreaPlane _planeLeft;
    protected EEAreaPlane _planeTop;
    protected EEAreaPlane _planeBottom;
    protected EEAreaPlane _planeFront;
    protected EEAreaPlane _planeBack;

    // Used later to sort all plane by they distance with the camera
    protected List<KeyValuePair<EEAreaPlane, Vector3>> _sortAreaPlane = new List<KeyValuePair<EEAreaPlane, Vector3>>();

    // Area variables
    [SerializeField] [Tooltip("Size")] protected private Vector3 _size = new Vector3(1, 1, 1);
    [SerializeField] [Tooltip("Internal size")] private Vector3 _internalSize = new Vector3(1, 1, 1);
    [SerializeField] [Tooltip("Transit size")] private Vector3 _transitSize = new Vector3(0, 0, 0);

#endregion
#region Unity events
    /************************************************/
    /*                                              */
    /*             Unity events handler             */
    /*                                              */
    /************************************************/

    //Called in editor, when creating object
    void Reset()
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
    protected virtual void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.size = _size + 2 * _transitSize;

        _camera = Instantiate(new GameObject(), transform).AddComponent<Camera>();
        _camera.gameObject.name = "Camera";
        _camera.cullingMask &= (int)(0xFFFFFFFF ^ (1 << LayerMask.NameToLayer("toto")));
        _camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 0);

        GameObject planePrefab = Resources.Load("Prefab/New Sprite") as GameObject;
        _planeBack = Instantiate(planePrefab, new Vector3(0, _size.y / 2, -_size.z / 2f), Quaternion.LookRotation(Vector3.forward, transform.up), transform)
                        .GetComponent<EEAreaPlane>();
        _planeFront = Instantiate(planePrefab, new Vector3(0, _size.y / 2, _size.z / 2f), Quaternion.LookRotation(Vector3.back, transform.up), transform)
                        .GetComponent<EEAreaPlane>();
        _planeRight = Instantiate(planePrefab, new Vector3(_size.x / 2f, _size.y / 2, 0), Quaternion.LookRotation(Vector3.left, transform.up), transform)
                        .GetComponent<EEAreaPlane>();
        _planeLeft = Instantiate(planePrefab, new Vector3(-_size.x / 2f, _size.y / 2, 0), Quaternion.LookRotation(Vector3.right, transform.up), transform)
                        .GetComponent<EEAreaPlane>();
        _planeTop = Instantiate(planePrefab, new Vector3(0, _size.y, 0), Quaternion.LookRotation(Vector3.down, transform.forward), transform)
                        .GetComponent<EEAreaPlane>();
        _planeBottom = Instantiate(planePrefab, new Vector3(0, 0, 0), Quaternion.LookRotation(Vector3.up, -transform.forward), transform)
                        .GetComponent<EEAreaPlane>();

        //set game object name for each plane
        _planeBack.gameObject.name = "Plane Back";
        _planeRight.gameObject.name = "Plane Right";
        _planeLeft.gameObject.name = "Plane Left";
        _planeTop.gameObject.name = "Plane Top";
        _planeBottom.gameObject.name = "Plane Bottom";
        _planeFront.gameObject.name = "Plane Front";

        //set camera for each plane
        _planeBack.camera = _camera;
        _planeRight.camera = _camera;
        _planeLeft.camera = _camera;
        _planeTop.camera = _camera;
        _planeBottom.camera = _camera;
        _planeFront.camera = _camera;

        //set sortAreaPlane
        _sortAreaPlane.Add(new KeyValuePair<EEAreaPlane, Vector3>(_planeBack, transform.forward));
        _sortAreaPlane.Add(new KeyValuePair<EEAreaPlane, Vector3>(_planeFront, -transform.forward));
        _sortAreaPlane.Add(new KeyValuePair<EEAreaPlane, Vector3>(_planeRight, -transform.right));
        _sortAreaPlane.Add(new KeyValuePair<EEAreaPlane, Vector3>(_planeLeft, transform.right));
        _sortAreaPlane.Add(new KeyValuePair<EEAreaPlane, Vector3>(_planeTop, -transform.up));
        _sortAreaPlane.Add(new KeyValuePair<EEAreaPlane, Vector3>(_planeBottom, transform.up));
    }

    //Called at end (of object or scene)
    protected virtual void OnDestroy()
    {
        EEAreaDelete(_area);
    }

    //Called on collision
    void OnTriggerEnter(Collider c)
    {
        int id = c.GetInstanceID();
        EEAreaAddObjectInside(_area, id, (IntPtr)id);
        c.gameObject.AddComponent<EuclidEngineObject>();
    }

    //Called at the end of collision
    void OnTriggerExit(Collider c)
    {
        EEAreaRemoveObjectInside(_area, c.GetInstanceID());
        Destroy(c.gameObject.GetComponent<EuclidEngineObject>());
    }

    //Called every frame
    virtual protected void Update()
    {
        EEAreaUpdate(_area);
        UpdatePlanes();

        EuclidEngineCamera eecam = Array.Find(Camera.main.GetComponents<EuclidEngineCamera>(), camera => camera.area == _area);
        if (_collider.bounds.Contains(Camera.main.transform.position)) {
            if (!eecam) {
                eecam = Camera.main.gameObject.AddComponent<EuclidEngineCamera>();
                eecam.area = _area;
            }
        } else if (eecam) {
            Destroy(eecam);
        }
    }

#endregion
#region Constructors
    /************************************************/
    /*                                              */
    /*                 Constructors                 */
    /*                                              */
    /************************************************/


    /// @brief Construct a new default EuclidEngineArea
    /// @param position World position of the new EuclidEngineArea
    /// @param rotation World rotation of the new EuclidEngineArea
    /// @return A new EuclidEngineArea attached on a new GameObject
    public static EuclidEngineArea Instantiate(Vector3 position, Quaternion rotation)
    { return EuclidEngineArea.Instantiate(new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, 0f), position, rotation); }
    /// @brief Construct a new default EuclidEngineArea
    /// @param position Local position of the new EuclidEngineArea
    /// @param rotation Local rotation of the new EuclidEngineArea
    /// @param parent The parent of the new EuclidEngineArea
    /// @return A new EuclidEngineArea attached on a new GameObject
    public static EuclidEngineArea Instantiate(Vector3 position, Quaternion rotation, Transform parent)
    { return EuclidEngineArea.Instantiate(new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, 0f), position, rotation, parent); }

    /// @brief Construct a new EuclidEngineArea
    /// @param areaSize The visual size of the new EuclidEngineArea
    /// @param internalSize The traversal size of the new EuclidEngineArea
    /// @param position World position of the new EuclidEngineArea
    /// @param rotation World rotation of the new EuclidEngineArea
    /// @return A new EuclidEngineArea attached on a new GameObject
    public static EuclidEngineArea Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 position, Quaternion rotation)
    { return EuclidEngineArea.Instantiate(areaSize, internalSize, new Vector3(0f, 0f, 0f), position, rotation); }
    /// @brief Construct a new default EuclidEngineArea
    /// @param position Local position of the new EuclidEngineArea
    /// @param rotation Local rotation of the new EuclidEngineArea
    /// @param parent The parent of the new EuclidEngineArea
    /// @return A new EuclidEngineArea attached on a new GameObject
    public static EuclidEngineArea Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 position, Quaternion rotation, Transform parent)
    { return EuclidEngineArea.Instantiate(areaSize, internalSize, new Vector3(0f, 0f, 0f), position, rotation, parent); }

    /// @brief Construct a new default EuclidEngineArea
    /// @param position World position of the new EuclidEngineArea
    /// @param rotation World rotation of the new EuclidEngineArea
    /// @return A new EuclidEngineArea attached on a new GameObject
    public static EuclidEngineArea Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 transitionSize, Vector3 position, Quaternion rotation)
    { return EuclidEngineArea.Instantiate(areaSize, internalSize, transitionSize, position, rotation, null); }

    /// @brief Construct a new default EuclidEngineArea
    /// @param position Local position of the new EuclidEngineArea
    /// @param rotation Local rotation of the new EuclidEngineArea
    /// @param parent The parent of the new EuclidEngineArea
    /// @return A new EuclidEngineArea attached on a new GameObject
    public static EuclidEngineArea2 Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 transitionSize, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject go = new GameObject(typeof(EuclidEngineArea).ToString());
        if (parent)
            go.transform.parent = parent;
        go.transform.localPosition = position;
        go.transform.localRotation = rotation;

        EuclidEngineArea2 area = go.AddComponent<EuclidEngineArea2>();
        area.Reset();
        area.SetSize(areaSize, internalSize, transitionSize);

        return area;
    }

#endregion
#region Public properties
    /************************************************/
    /*                                              */
    /*              C# public properties            */
    /*                                              */
    /************************************************/

    /// @brief The external size of the EuclidEngineArea, without the transition area.
    public Vector3 areaSize
    {
        get { return _size; }
        set {
            if (value.x <= 0 || value.y <= 0 || value.z <= 0)
                throw new ArgumentOutOfRangeException("size", "Size must be strictly positiv");
            _size = value;
            EEAreaSetSize(_area, (double)_size.x, (double)_size.z, (double)_size.y);
            _collider.size = _size + 2 * _transitSize;
        }
    }

    /// The internal size of the EuclidEngineArea
    public Vector3 internalSize
    {
        get { return _internalSize; }
        set {
            if (value.x <= 0 || value.y <= 0 || value.z <= 0)
                throw new ArgumentOutOfRangeException("internalSize", "Internal size must be strictly positiv");
            _internalSize = value;
            EEAreaSetInternalSize(_area, (double)_internalSize.x, (double)_internalSize.z, (double)_internalSize.y);
        }
    }

    /// @brief The size of the transition area around the EuclidEngineArea. This size is applied as is to both side the the Area.
    public Vector3 transitionSize
    {
        get { return _transitSize; }
        set {
            if (value.x < 0 || value.y < 0 || value.z < 0)
                throw new ArgumentOutOfRangeException("transitSize", "Transition size cannot be negativ");
            _transitSize = value;
            EEAreaSetTransitAreaSize(_area, (double)_transitSize.x, (double)_transitSize.z, (double)_transitSize.y);
            _collider.size = _size + 2 * _transitSize;
        }
    }

    /// @brief The total size of the EuclidEngineArea, including the transition area.
    public Vector3 size
    {
        get { return _size + 2 * _transitSize; }
    }

#endregion
#region Public methods
    /************************************************/
    /*                                              */
    /*              C# public functions             */
    /*                                              */
    /************************************************/

    /// @brief Set the external and internal size of the EuclidEngineArea
    /// @param areaSize The external size, without the transition area
    /// @param internalSize The internal size, fitting in the area
    public void SetSize(Vector3 areaSize, Vector3 internalSize)
    { SetSize(areaSize, internalSize, _transitSize); }
    /// @brief Set the external, internal size and the transition area of the EuclidEngineArea
    /// @param areaSize The external size, without the transition area
    /// @param internalSize The internal size, fitting in the area
    /// @param transitionSize The size of the transition area, which is apply on both side
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

        if (_collider == null)
            _collider = GetComponent<BoxCollider>();

        _collider.size = _size + 2 * _transitSize;
    }

    #endregion
    #region Private methods
    /************************************************/
    /*                                              */
    /*            C# private functions              */
    /*                                              */
    /************************************************/
    virtual protected void UpdatePlanes()
    {
        _camera.transform.position = Camera.main.transform.position;
        _camera.transform.rotation = Camera.main.transform.rotation;
        EEAreaSetCameraPosition(_area, _camera.transform.position);

        //set plane size
        _planeBack.size =   new Vector2(_size.x, _size.y);
        _planeFront.size =  new Vector2(_size.x, _size.y);
        _planeRight.size =  new Vector2(_size.z, _size.y);
        _planeLeft.size =   new Vector2(_size.z, _size.y);
        _planeTop.size =    new Vector2(_size.x, _size.z);
        _planeBottom.size = new Vector2(_size.x, _size.z);

        //update each plan with new camera matrix
        Matrix4x4 transformMatrix;
        List<KeyValuePair<KeyValuePair<EEAreaPlane, Vector3>, float>> distances = new List<KeyValuePair<KeyValuePair<EEAreaPlane, Vector3>, float>>();

        for (short i = 0; i < 6; ++i) {
            distances.Add(new KeyValuePair<KeyValuePair<EEAreaPlane, Vector3>, float>(_sortAreaPlane[i],
                Mathf.Sqrt(Mathf.Pow(_sortAreaPlane[i].Key.transform.position.x - _camera.transform.position.x, (float)2.0) +
                Mathf.Pow(_sortAreaPlane[i].Key.transform.position.y - _camera.transform.position.y, (float)2.0) +
                Mathf.Pow(_sortAreaPlane[i].Key.transform.position.z - _camera.transform.position.z, (float)2.0))));
            _sortAreaPlane[i].Key.gameObject.layer |= LayerMask.NameToLayer("toto");
        }

        //sort distances
        distances.Sort(delegate (KeyValuePair<KeyValuePair<EEAreaPlane, Vector3>, float> a, KeyValuePair<KeyValuePair<EEAreaPlane, Vector3>, float> b)
        {
            return a.Value.CompareTo(b.Value);
        });

        //Reverse array so it is descending
        distances.Reverse();

        foreach (var item in distances)
        {
            EEAreaGetTransformMatrix(_area, item.Key.Value, out transformMatrix);
            item.Key.Key.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
            item.Key.Key.gameObject.layer ^= LayerMask.NameToLayer("toto");
        }

        /*
        EEAreaGetTransformMatrix(_area, transform.forward, out transformMatrix);
        _planeBack.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
        EEAreaGetTransformMatrix(_area, -transform.forward, out transformMatrix);
        _planeFront.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
        EEAreaGetTransformMatrix(_area, -transform.right, out transformMatrix);
        _planeRight.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
        EEAreaGetTransformMatrix(_area, transform.right, out transformMatrix);
        _planeLeft.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
        EEAreaGetTransformMatrix(_area, -transform.up, out transformMatrix);
        _planeTop.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
        EEAreaGetTransformMatrix(_area, transform.up, out transformMatrix);
        _planeBottom.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
        */
    }

#endregion
#region C++ callbacks
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
        Collider collider = EuclidEngine.FindObjectFromInstanceID((int)go) as Collider;
        x = collider.transform.position.x;
        y = collider.transform.position.z;
        z = collider.transform.position.y;
    }

    private void SetObjectSize(IntPtr go, ref double minx, ref double miny, ref double minz, ref double maxx, ref double maxy, ref double maxz)
    {
        Collider collider = EuclidEngine.FindObjectFromInstanceID((int)go) as Collider;
        minx = collider.bounds.min.x;
        miny = collider.bounds.min.z;
        minz = collider.bounds.min.y;
        maxx = collider.bounds.max.x;
        maxy = collider.bounds.max.z;
        maxz = collider.bounds.max.y;
    }

    private void ScaleObjetct(IntPtr go, double x, double y, double z)
    {
        Collider collider = EuclidEngine.FindObjectFromInstanceID((int)go) as Collider;
        collider.transform.localScale = new Vector3((float)x, (float)z, (float)y);
    }
#endregion
}
