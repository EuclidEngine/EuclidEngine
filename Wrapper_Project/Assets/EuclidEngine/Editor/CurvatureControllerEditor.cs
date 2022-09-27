﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CurvatureController)), CanEditMultipleObjects]
[InitializeOnLoad]
public class CurvatureControllerEditor : Editor
{
    SerializedProperty curv;
    static Texture2D logoTexture = null;

    /// @brief Editor Constructor
    void OnEnable()
    {
        if (logoTexture == null)
            logoTexture = Resources.Load("img/background") as Texture2D;
        curv = serializedObject.FindProperty("curv");
    }
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(logoTexture, GUILayout.Height(75), GUILayout.Width(75));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        serializedObject.Update();
        EditorGUILayout.LabelField("Main Settings:");
        EditorGUILayout.PropertyField(curv);
        serializedObject.ApplyModifiedProperties();
    }
}