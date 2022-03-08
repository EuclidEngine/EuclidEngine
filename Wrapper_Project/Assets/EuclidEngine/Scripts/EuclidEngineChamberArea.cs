using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;

[AddComponentMenu("Euclid Engine/Non Euclidian Chamber")]
[RequireComponent(typeof(BoxCollider), typeof(Rigidbody), typeof(Material))]

public class EuclidEngineChamberArea : EuclidEngineArea
{
    public GameObject myPrefab;

    public Material WallMaterial;
    public float WallThickness;

    private GameObject FrontWall;
    private GameObject BackWall;
    private GameObject LeftWall;
    private GameObject RightWall;
    private GameObject TopWall;
    private GameObject BotWall;

    // Start is called before the first frame update
    protected new void Start()
    {
        //Call Area Start function
        base.Start();

        //init Wall with pos and rotation
        FrontWall = Instantiate(myPrefab, new Vector3(0, _size.y / 2, _size.z / 2f), Quaternion.LookRotation(Vector3.back, transform.up), transform);
        BackWall = Instantiate(myPrefab, new Vector3(0, _size.y / 2, -_size.z / 2f), Quaternion.LookRotation(Vector3.forward, transform.up), transform);
        LeftWall = Instantiate(myPrefab, new Vector3(-_size.x / 2f, _size.y / 2, 0), Quaternion.LookRotation(Vector3.right, transform.up), transform);
        RightWall = Instantiate(myPrefab, new Vector3(_size.x / 2f, _size.y / 2, 0), Quaternion.LookRotation(Vector3.left, transform.up), transform);
        TopWall = Instantiate(myPrefab, new Vector3(0, _size.y, 0), Quaternion.LookRotation(Vector3.down, transform.forward), transform);
        BotWall = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.LookRotation(Vector3.up, -transform.forward), transform);

        //set name of object for debug purpose
        FrontWall.name = "FrontWall";
        BackWall.name = "BackWall";
        LeftWall.name = "LeftWall";
        RightWall.name = "RightWall";
        TopWall.name = "TopWall";
        BotWall.name = "BotWall";

        //set Wall size
        FrontWall.transform.localScale = new Vector3(_size.x / FrontWall.transform.localScale.x + WallThickness, _size.y / FrontWall.transform.localScale.y, WallThickness);
        BackWall.transform.localScale = new Vector3(_size.x / BackWall.transform.localScale.x + WallThickness, _size.y / BackWall.transform.localScale.y, WallThickness);
        LeftWall.transform.localScale = new Vector3(_size.x / LeftWall.transform.localScale.x + WallThickness, _size.y / LeftWall.transform.localScale.y, WallThickness);
        RightWall.transform.localScale = new Vector3(_size.x / RightWall.transform.localScale.x + WallThickness, _size.y / RightWall.transform.localScale.y, WallThickness);
        TopWall.transform.localScale = new Vector3(_size.x / TopWall.transform.localScale.x + WallThickness, _size.x + WallThickness, WallThickness);
        BotWall.transform.localScale = new Vector3(_size.x / BotWall.transform.localScale.x + WallThickness, _size.x + WallThickness, WallThickness);

        //set Wall material
        FrontWall.GetComponent<Renderer>().material = WallMaterial;
        BackWall.GetComponent<Renderer>().material = WallMaterial;
        LeftWall.GetComponent<Renderer>().material = WallMaterial;
        RightWall.GetComponent<Renderer>().material = WallMaterial;
        TopWall.GetComponent<Renderer>().material = WallMaterial;
        BotWall.GetComponent<Renderer>().material = WallMaterial;
    }

    protected new void Update()
    {
        EEAreaUpdate(_area);
        ChamberUpdatePlanes();

        EuclidEngineCamera eecam = Array.Find(Camera.main.GetComponents<EuclidEngineCamera>(), camera => camera.area == _area);
        if (_collider.bounds.Contains(Camera.main.transform.position))
        {
            if (!eecam)
            {
                eecam = Camera.main.gameObject.AddComponent<EuclidEngineCamera>();
                eecam.area = _area;
            }
        }
        else if (eecam)
        {
            Destroy(eecam);
        }
    }


    private void ChamberUpdatePlanes()
    {
        _camera.transform.position = Camera.main.transform.position;
        _camera.transform.rotation = Camera.main.transform.rotation;
        EEAreaSetCameraPosition(_area, _camera.transform.position);

        //set plane size
        _planeBack.size = new Vector2(_size.x, _size.y);
        _planeFront.size = new Vector2(_size.x, _size.y);
        _planeRight.size = new Vector2(_size.z, _size.y);
        _planeLeft.size = new Vector2(_size.z, _size.y);
        _planeTop.size = new Vector2(_size.x, _size.z);
        _planeBottom.size = new Vector2(_size.x, _size.z);

        //update each plan with new camera matrix
        Matrix4x4 transformMatrix;
        List<KeyValuePair<KeyValuePair<EEAreaPlane, Vector3>, float>> distances = new List<KeyValuePair<KeyValuePair<EEAreaPlane, Vector3>, float>>();

        for (short i = 0; i < 6; ++i)
        {
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
            EEAreaGetTransformMatrix(_area, new Vector3(1, 1, 1), out transformMatrix);
            item.Key.Key.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
            item.Key.Key.gameObject.layer ^= LayerMask.NameToLayer("toto");
        }
    }

    /*// Update is called once per frame
    void Update()
    {
        
    }*/
}
