﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EuclidEngine
{
    [CustomEditor(typeof(EuclidEngineCurvatureController)), CanEditMultipleObjects]
    [InitializeOnLoad]
    public class CurvatureControllerEditor : Editor
    {
        SerializedProperty worldCurvature;
        static Texture2D logoTexture = null;
        EuclidEnginePlayTime pT = new EuclidEnginePlayTime();


        /// @brief Editor Constructor
        void OnEnable()
        {
            if (logoTexture == null)
                logoTexture = Resources.Load("img/background") as Texture2D;
            worldCurvature = serializedObject.FindProperty("worldCurvature");
        }
        public override void OnInspectorGUI()
        {
            pT.AddTime();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logoTexture, GUILayout.Height(75), GUILayout.Width(75));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("Don't forget to apply Curved Shaders to the objects under this parent", MessageType.Warning);

            serializedObject.Update();
            EditorGUILayout.LabelField("Main Settings:");
            EditorGUILayout.PropertyField(worldCurvature);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            pT.OnDestroy<EuclidEngineCurvatureController>();
        }
    }
};