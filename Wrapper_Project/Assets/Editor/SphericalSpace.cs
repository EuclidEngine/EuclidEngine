using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

[CustomEditor(typeof(EuclidEngineSpace))]
public class SphericalSpace : Editor
{
    SerializedProperty rad, cam;
    SerializedProperty[] gos = new SerializedProperty[6], rots = new SerializedProperty[6];
    static bool shunksOpen = false;

    public void OnEnable()
    {
        rad = serializedObject.FindProperty("worldRadius");
        cam = serializedObject.FindProperty("mainCamera");
        
        var _gos = serializedObject.FindProperty("prefabs");
        var _rots = serializedObject.FindProperty("prefRot");
        for (int i = 0; i < 6; ++i) {
            if (_gos.arraySize > i)
                gos[i] = _gos.GetArrayElementAtIndex(i);
            if (_rots.arraySize > i)
                rots[i] = _rots.GetArrayElementAtIndex(i);
        }
    }

    private string[] ShunkNames = {"Below","Front","Right","Back","Left","Above"};
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(rad, new GUIContent("World size"));
        EditorGUILayout.PropertyField(cam, new GUIContent("Player camera"));
        EditorGUILayout.Space();
        shunksOpen = EditorGUILayout.BeginFoldoutHeaderGroup(shunksOpen, "Shunks");
        if (shunksOpen) {
            for (int i = 0; i < 6; ++i) {
                EditorGUILayout.BeginHorizontal();
                //GUILayout.Label(ShunkNames[i]);
                GUILayout.FlexibleSpace();
                gos[i].objectReferenceValue = EditorGUILayout.ObjectField(gos[i].objectReferenceValue, typeof(GameObject), false);
                rots[i].floatValue = (float)EditorGUILayout.IntPopup("Rotation", (int)rots[i].floatValue, new string[]{"None","Clockwise","Anti-Clockwise","Half-Turn"}, new int[]{0,90,-90,180}, new GUILayoutOption[]{GUILayout.Width(175)});
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}