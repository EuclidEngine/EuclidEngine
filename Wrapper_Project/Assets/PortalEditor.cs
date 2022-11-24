﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace EuclidEngine
{
    [CustomEditor(typeof(Portal)), CanEditMultipleObjects]
    [InitializeOnLoad]
    public class PortalEditor : UnityEditor.Editor
    {
        SerializedProperty nearClipOffset;
        SerializedProperty nearClipLimit;
        SerializedProperty linkedPortal;
        SerializedProperty screen;
        static Texture2D logoTexture = null;
        SerializedProperty recursionLimit;
        static EuclidEnginePlayTime pT = new EuclidEnginePlayTime();

        /// @brief Editor Constructor
        void OnEnable()
        {
            if (logoTexture == null)
                logoTexture = Resources.Load("img/background") as Texture2D;
            screen = serializedObject.FindProperty("screen");
            nearClipOffset = serializedObject.FindProperty("nearClipOffset");
            nearClipLimit = serializedObject.FindProperty("nearClipLimit");
            linkedPortal = serializedObject.FindProperty("linkedPortal");
            recursionLimit = serializedObject.FindProperty("recursionLimit");
        }

        private bool foldedAdvanced = false;
        private bool foldedMain = true;
        public override void OnInspectorGUI()
        {
            pT.AddTime();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logoTexture, GUILayout.Height(75), GUILayout.Width(75));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            serializedObject.Update();
            if (foldedMain = EditorGUILayout.Foldout(foldedMain, "Main Settings :"))
            {
                if (linkedPortal.objectReferenceValue == null)
                    EditorGUILayout.HelpBox("Don't forget to link it to another portal", MessageType.Error);
                EditorGUILayout.PropertyField(linkedPortal);
                EditorGUILayout.PropertyField(screen);
                EditorGUILayout.PropertyField(recursionLimit);
            }
            EditorGUILayout.Space();
            if (foldedAdvanced = EditorGUILayout.Foldout(foldedAdvanced, "Advanced Settings :"))
            {
                EditorGUILayout.PropertyField(nearClipOffset);
                EditorGUILayout.PropertyField(nearClipLimit);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            pT.OnDestroy<Portal>();
        }
    }
}
#endif