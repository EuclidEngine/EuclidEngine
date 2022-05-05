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

    [DllImport(EuclidEngine.plugin)] private static extern void EEAreaGetPlayerTransformMatrix(IntPtr area, out Matrix4x4 ret);
    void OnPreCull()
    {
        Matrix4x4 transformMatrix = Matrix4x4.identity;
        EEAreaGetPlayerTransformMatrix(area, out transformMatrix);
        _camera.worldToCameraMatrix *= transformMatrix;
    }

    void OnPostRender()
    {
        _camera.ResetWorldToCameraMatrix();
    }
}
