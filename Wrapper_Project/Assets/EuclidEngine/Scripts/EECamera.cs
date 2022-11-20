using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace EuclidEngine
{
    public class EECamera : MonoBehaviour
    {
        public IntPtr area;
        private UnityEngine.Camera _camera;

        void Start()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }

        [DllImport(EuclidEngineDll.plugin)] private static extern void EEAreaGetPlayerTransformMatrix(IntPtr area, out Matrix4x4 ret);
        void OnPreCull()
        {
            Matrix4x4 transformMatrix = Matrix4x4.identity;
            EEAreaGetPlayerTransformMatrix(area, out transformMatrix);

            transformMatrix.m03 = 0;
            transformMatrix.m13 = 0;
            transformMatrix.m23 = 0;

            Debug.Log(transformMatrix);

            _camera.worldToCameraMatrix *= transformMatrix;
        }

        void OnPostRender()
        {
            _camera.ResetWorldToCameraMatrix();
        }
    }
};