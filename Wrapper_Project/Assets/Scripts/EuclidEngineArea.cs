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
public class EuclidEngineArea : MonoBehaviour
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
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetSize(IntPtr area, double pExternX, double pExternY, double pExternZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetInternalSize(IntPtr area, double pInternX, double pInternY, double pInternZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaSetTransitAreaSize(IntPtr area, double sizeX, double sizeY, double sizeZ);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaAddObjectInside(IntPtr area, int objectId, IntPtr obj);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaRemoveObjectInside(IntPtr area, int objectId);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaGetTransformMatrix(IntPtr area, Vector3 dir, out Matrix4x4 ret);
    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaUpdate(IntPtr area);

#endregion
#region Variables
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
    private Camera _camera;
    private EEAreaPlane _planeRight;
    private EEAreaPlane _planeLeft;
    private EEAreaPlane _planeTop;
    private EEAreaPlane _planeBottom;
    private EEAreaPlane _planeFront;
    private EEAreaPlane _planeBack;

    // Area variables
    [SerializeField] [Tooltip("Size")] private Vector3 _size = new Vector3(1, 1, 1);
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
    void Start()
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
    }

    //Called at end (of object or scene)
    void OnDestroy()
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
    void Update()
    {
        EEAreaUpdate(_area);
        UpdatePlanes();
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
    public static EuclidEngineArea Instantiate(Vector3 areaSize, Vector3 internalSize, Vector3 transitionSize, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject go = new GameObject(typeof(EuclidEngineArea).ToString());
        if (parent)
            go.transform.parent = parent;
        go.transform.localPosition = position;
        go.transform.localRotation = rotation;

        EuclidEngineArea area = go.AddComponent<EuclidEngineArea>();
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

    private void UpdatePlanes()
    {
        _camera.transform.position = Camera.main.transform.position;
        _camera.transform.rotation = Camera.main.transform.rotation;

        Vector4 tmp;
        Matrix4x4 spaceToScreen = GL.GetGPUProjectionMatrix(_camera.projectionMatrix, true) * _camera.worldToCameraMatrix;

        Vector3 vertex000 = transform.TransformPoint(new Vector3(-_size.x / 2, 0, -_size.z / 2));
                tmp = spaceToScreen * new Vector4(vertex000.x, vertex000.y, vertex000.z, 1);
                vertex000 = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 vertex001 = transform.TransformPoint(new Vector3(-_size.x / 2, 0, _size.z / 2));
                tmp = spaceToScreen * new Vector4(vertex001.x, vertex001.y, vertex001.z, 1);
                vertex001 = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 vertex010 = transform.TransformPoint(new Vector3(-_size.x / 2, _size.y, -_size.z / 2));
                tmp = spaceToScreen * new Vector4(vertex010.x, vertex010.y, vertex010.z, 1);
                vertex010 = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 vertex011 = transform.TransformPoint(new Vector3(-_size.x / 2, _size.y, _size.z / 2));
                tmp = spaceToScreen * new Vector4(vertex011.x, vertex011.y, vertex011.z, 1);
                vertex011 = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 vertex100 = transform.TransformPoint(new Vector3(_size.x / 2, 0, -_size.z / 2));
                tmp = spaceToScreen * new Vector4(vertex100.x, vertex100.y, vertex100.z, 1);
                vertex100 = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 vertex101 = transform.TransformPoint(new Vector3(_size.x / 2, 0, _size.z / 2));
                tmp = spaceToScreen * new Vector4(vertex101.x, vertex101.y, vertex101.z, 1);
                vertex101 = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 vertex110 = transform.TransformPoint(new Vector3(_size.x / 2, _size.y, -_size.z / 2));
                tmp = spaceToScreen * new Vector4(vertex110.x, vertex110.y, vertex110.z, 1);
                vertex110 = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 vertex111 = transform.TransformPoint(new Vector3(_size.x / 2, _size.y, _size.z / 2));
                tmp = spaceToScreen * new Vector4(vertex111.x, vertex111.y, vertex111.z, 1);
                vertex111 = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);

        //set plane vertex
        _planeFront.vertex00 =  vertex101;  _planeFront.vertex01 =  vertex111;  _planeFront.vertex10 =  vertex001;  _planeFront.vertex11 =  vertex011;
        _planeBack.vertex00 =   vertex000;  _planeBack.vertex01 =   vertex010;  _planeBack.vertex10 =   vertex100;  _planeBack.vertex11 =   vertex110;
        _planeRight.vertex00 =  vertex101;  _planeRight.vertex01 =  vertex111;  _planeRight.vertex10 =  vertex100;  _planeRight.vertex11 =  vertex110;
        _planeLeft.vertex00 =   vertex000;  _planeLeft.vertex01 =   vertex010;  _planeLeft.vertex10 =   vertex001;  _planeLeft.vertex11 =   vertex011;
        _planeTop.vertex00 =    vertex011;  _planeTop.vertex01 =    vertex010;  _planeTop.vertex10 =    vertex111;  _planeTop.vertex11 =    vertex110;
        _planeBottom.vertex00 = vertex000;  _planeBottom.vertex01 = vertex001;  _planeBottom.vertex10 = vertex100;  _planeBottom.vertex11 = vertex101;

        //set plane size
        _planeBack.size = new Vector2(_size.x, _size.y);
        _planeFront.size = new Vector2(_size.x, _size.y);
        _planeRight.size = new Vector2(_size.z, _size.y);
        _planeLeft.size = new Vector2(_size.z, _size.y);
        _planeTop.size = new Vector2(_size.x, _size.z);
        _planeBottom.size = new Vector2(_size.x, _size.z);


        //update each plan with new camera matrix
        Matrix4x4 worldToCam = _camera.worldToCameraMatrix;
        Matrix4x4 transformMatrix = Matrix4x4.identity;

        EEAreaGetTransformMatrix(_area, transform.forward, out transformMatrix);
        _camera.worldToCameraMatrix = worldToCam * transformMatrix;
        _planeBack.UpdatePlane();
        transformMatrix = Matrix4x4.identity;//EEAreaGetTransformMatrix(_area, transform.forward, out transformMatrix);
        _camera.worldToCameraMatrix = worldToCam * transformMatrix;
        _planeFront.UpdatePlane();
        transformMatrix = Matrix4x4.identity;//EEAreaGetTransformMatrix(_area, transform.forward, out transformMatrix);
        _camera.worldToCameraMatrix = worldToCam * transformMatrix;
        _planeRight.UpdatePlane();
        EEAreaGetTransformMatrix(_area, transform.right, out transformMatrix);
        _camera.worldToCameraMatrix = worldToCam * transformMatrix;
        _planeLeft.UpdatePlane();
        transformMatrix = Matrix4x4.identity;//EEAreaGetTransformMatrix(_area, transform.forward, out transformMatrix);
        _camera.worldToCameraMatrix = worldToCam * transformMatrix;
        _planeTop.UpdatePlane();
        EEAreaGetTransformMatrix(_area, transform.up, out transformMatrix);
        _camera.worldToCameraMatrix = worldToCam * transformMatrix;
        _planeBottom.UpdatePlane();

        //reset cam
        _camera.ResetWorldToCameraMatrix();
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
