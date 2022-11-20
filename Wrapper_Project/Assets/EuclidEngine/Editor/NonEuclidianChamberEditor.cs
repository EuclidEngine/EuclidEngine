using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
namespace EuclidEngine
{
    [CustomEditor(typeof(ChamberArea)), CanEditMultipleObjects]
    [InitializeOnLoad]
    public class NonEuclidianChamberAreaEditor : Editor
    {
        /*Serialized Property*//*
        SerializedProperty prefab;
        SerializedProperty WallMaterial;
        SerializedProperty WallThickness;
        SerializedProperty Size;
        SerializedProperty InternalSize;
        SerializedProperty TransitSize;
        SerializedProperty NumberOfPieces;
        SerializedProperty BackSide;
        SerializedProperty FrontSide;
        SerializedProperty LeftSide;
        SerializedProperty RightSide;
        SerializedProperty TopSide;
        SerializedProperty BottomSide;

        int selected = 0;
        string[] options = new string[]
    {
            "Back Side", "Front Side", "Right Side", "Left Side", "Top Side", "Bottom Side"
    };

        static Texture2D logoTexture = null;

        /// @brief Editor Constructor
        void OnEnable()
        {
            if (logoTexture == null)
                logoTexture = Resources.Load("img/background") as Texture2D;
            prefab = serializedObject.FindProperty("myPrefab");
            WallMaterial = serializedObject.FindProperty("WallMaterial");
            WallThickness = serializedObject.FindProperty("WallThickness");
            Size = serializedObject.FindProperty("_size");
            InternalSize = serializedObject.FindProperty("_internalSize");
            TransitSize = serializedObject.FindProperty("_transitSize");
            NumberOfPieces = serializedObject.FindProperty("NumberOfPieces");
            BackSide = serializedObject.FindProperty("BackSide");
            FrontSide = serializedObject.FindProperty("FrontSide");
            LeftSide = serializedObject.FindProperty("LeftSide");
            RightSide = serializedObject.FindProperty("RightSide");
            TopSide = serializedObject.FindProperty("TopSide");
            BottomSide = serializedObject.FindProperty("BottomSide");
        }

        private void CheckWholeSide(int selected, bool check)
        {
            SerializedProperty[] side = { BackSide, FrontSide, RightSide, LeftSide, TopSide, BottomSide };


            for (int j = 0; j < NumberOfPieces.intValue; ++j)
            {
                SerializedProperty property = side[selected].GetArrayElementAtIndex(j);
                property.Next(true);
                for (int k = 0; k < NumberOfPieces.intValue; ++k)
                {
                    SerializedProperty result = property.GetArrayElementAtIndex(k);
                    result.boolValue = check;
                }
            }
        }

        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logoTexture, GUILayout.Height(75), GUILayout.Width(75));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            serializedObject.Update();
            EditorGUILayout.LabelField("Main Settings:");
            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.PropertyField(WallMaterial);
            EditorGUILayout.PropertyField(WallThickness);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Advanced Settings:");
            EditorGUILayout.PropertyField(Size);
            EditorGUILayout.PropertyField(InternalSize);
            EditorGUILayout.PropertyField(TransitSize);

            if (NumberOfPieces.intValue < 1)
                NumberOfPieces.intValue = 1;
            BackSide.arraySize = NumberOfPieces.intValue;
            FrontSide.arraySize = NumberOfPieces.intValue;
            RightSide.arraySize = NumberOfPieces.intValue;
            LeftSide.arraySize = NumberOfPieces.intValue;
            TopSide.arraySize = NumberOfPieces.intValue;
            BottomSide.arraySize = NumberOfPieces.intValue;
            for (int i = 0; i < NumberOfPieces.intValue; ++i)
            {

                SerializedProperty property = BackSide.GetArrayElementAtIndex(i);
                property.Next(true);
                property.arraySize = NumberOfPieces.intValue;

                property = FrontSide.GetArrayElementAtIndex(i);
                property.Next(true);
                property.arraySize = NumberOfPieces.intValue;

                property = LeftSide.GetArrayElementAtIndex(i);
                property.Next(true);
                property.arraySize = NumberOfPieces.intValue;

                property = RightSide.GetArrayElementAtIndex(i);
                property.Next(true);
                property.arraySize = NumberOfPieces.intValue;

                property = TopSide.GetArrayElementAtIndex(i);
                property.Next(true);
                property.arraySize = NumberOfPieces.intValue;

                property = BottomSide.GetArrayElementAtIndex(i);
                property.Next(true);
                property.arraySize = NumberOfPieces.intValue;
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Side Modification:");
            EditorGUILayout.PropertyField(NumberOfPieces);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check whole side"))
                CheckWholeSide(selected, true);
            if (GUILayout.Button("Uncheck whole side"))
                CheckWholeSide(selected, false);
            GUILayout.EndHorizontal();
            selected = EditorGUILayout.Popup("Side to edit", selected, options);
            switch (selected)
            {
                case 0:
                    EditorGUILayout.PropertyField(BackSide);
                    break;
                case 1:
                    EditorGUILayout.PropertyField(FrontSide);
                    break;
                case 2:
                    EditorGUILayout.PropertyField(RightSide);
                    break;
                case 3:
                    EditorGUILayout.PropertyField(LeftSide);
                    break;
                case 4:
                    EditorGUILayout.PropertyField(TopSide);
                    break;
                case 5:
                    EditorGUILayout.PropertyField(BottomSide);
                    break;
                default:
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
};
*/