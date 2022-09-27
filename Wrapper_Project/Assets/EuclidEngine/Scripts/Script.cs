using UnityEngine;
using UnityEditor;
using System;

public class Script : MonoBehaviour
{
    public static int Y;
    public bool[,] boolArray2D = new bool[Y, Y];

    public void changeY(int new_y)
    {
        Y = new_y;
        boolArray2D = new bool[new_y,new_y];
    }
}

[CustomEditor(typeof(Script))]
public class MyEditorofTest : Editor
{

    Script targetScript;
    int n = 0;
    int size = 0;

    void OnEnable()
    {
        targetScript = target as Script;
    }

    public override void OnInspectorGUI()
    {
        Script.Y = EditorGUILayout.IntField(Script.Y);
        if (Script.Y < 0)
            return;
        if (GUI.changed)
            targetScript.changeY(Script.Y);

        EditorGUILayout.BeginHorizontal();
        for (int y = 0; y < Script.Y; y++)
        {
            EditorGUILayout.BeginVertical();
            for (int x = 0; x < Script.Y; x++) {
                targetScript.boolArray2D[x, y] = EditorGUILayout.Toggle(targetScript.boolArray2D[x, y]);
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndHorizontal();

    }
}