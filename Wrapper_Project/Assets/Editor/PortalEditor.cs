using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Portal)), CanEditMultipleObjects]
[InitializeOnLoad]
public class PortalEditor : Editor
{
    SerializedProperty nearClipOffset;
    SerializedProperty nearClipLimit;
    SerializedProperty linkedPortal;
    SerializedProperty screen;
    static Texture2D logoTexture = null;
    SerializedProperty recursionLimit;

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
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(logoTexture, GUILayout.Height(75), GUILayout.Width(75));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        serializedObject.Update();
        EditorGUILayout.LabelField("Main Settings:");
        EditorGUILayout.PropertyField(linkedPortal);
        EditorGUILayout.PropertyField(screen);
        EditorGUILayout.PropertyField(recursionLimit);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Advanced Settings :");
        EditorGUILayout.PropertyField(nearClipOffset);
        EditorGUILayout.PropertyField(nearClipLimit);
        serializedObject.ApplyModifiedProperties();
    }
}
