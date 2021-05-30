using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(EuclidEngine.Area))]
[CanEditMultipleObjects]
public partial class EuclidEngine
{
    public class AreaEditor : Editor
    {
        SerializedProperty lookAtPoint;

        void OnEnable()
        {
            lookAtPoint = serializedObject.FindProperty("lookAtPoint");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(lookAtPoint);
            serializedObject.ApplyModifiedProperties();
        }
    }
}