using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EuclidEngineCamera : MonoBehaviour
{
    public IntPtr area;
    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaGetTransformMatrix(IntPtr area, Vector3 dir, out Matrix4x4 ret);
    void OnPreCull()
    {
        Matrix4x4 transformMatrix = Matrix4x4.identity;
        EEAreaGetTransformMatrix(area, Vector3.one, out transformMatrix);
        _camera.worldToCameraMatrix *= transformMatrix;
    }

    void OnPostRender()
    {
        _camera.ResetWorldToCameraMatrix();
    }
}
