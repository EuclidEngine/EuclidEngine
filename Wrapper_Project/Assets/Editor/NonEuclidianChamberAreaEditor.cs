using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System;


namespace EuclidEngine
{

    [CustomEditor(typeof(EuclidEngineChamberArea)), CanEditMultipleObjects]
    [InitializeOnLoad]
    public class NonEuclidianChamberAreaEditor : Editor
    {
        /*Serialized Property*/
        SerializedProperty prefab;
        SerializedProperty WallMaterial;
        SerializedProperty WallThickness;
        SerializedProperty Size;
        SerializedProperty InternalSize;
        SerializedProperty TransitSize;
        SerializedProperty NumberOfPieces;
        SerializedProperty Position;
        int NumberOfPiecesPerSide = 0;
        List<int[,]> toSave = new List<int[,]>();

        EuclidEngineChamberArea test;

        int selected = 0;
        string[] options = new string[]
        {
            "Back Side", "Front Side", "Right Side", "Left Side", "Top Side", "Bottom Side"
        };

        static Texture2D logoTexture = null;

        private void LoadArray()
        {
            if (!PlayerPrefs.HasKey("Length"))
            {
                return;
            }

            // For each face
            if (PlayerPrefs.GetInt("Length") != NumberOfPieces.intValue)
            {
                test.changeY(PlayerPrefs.GetInt("Length"));
                NumberOfPiecesPerSide = PlayerPrefs.GetInt("Length");
                NumberOfPieces.intValue = PlayerPrefs.GetInt("Length");
            }
            else
                test.changeY(NumberOfPieces.intValue);

            toSave.Clear();
            for (int i = 0; i < 6; ++i)
                toSave.Add(new int[NumberOfPieces.intValue, NumberOfPieces.intValue]);

            for (int i = 0; i < 6; ++i)
            {
                if (PlayerPrefs.HasKey(options[i]))
                {
                    for (int j = 0; j < NumberOfPieces.intValue; ++j)
                    {
                        string[] side = PlayerPrefs.GetString(options[i]).Split(char.Parse("\n"));
                        int[] nums = new int[NumberOfPieces.intValue];
                        for (int k = 0; k < NumberOfPieces.intValue; ++k)
                        {
                            Debug.Log(side[k]);
                            nums = Array.ConvertAll<string, int>(side[k].Split(','), int.Parse);
                            for (int l = 0; l < NumberOfPieces.intValue; ++l)
                            {
                                test.ArrayOfSides[i][k, l] = (nums[l] == 1) ? true : false;
                                //test.modifySide(i, test.ArrayOfSides[i][k, l], k, l);
                                toSave[i][l, k] = (test.ArrayOfSides[i][k, l]) ? 1 : 0;
                            }
                        }
                    }
                }
            }
            Debug.Log("Je suis à la fin de l'array");
        }

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
            Position = serializedObject.FindProperty("pos");


            test = target as EuclidEngineChamberArea;
            EditorUtility.SetDirty(test);
            test.changeY(NumberOfPieces.intValue);

            for (int i = 0; i < 6; ++i)
                toSave.Add(new int[NumberOfPieces.intValue, NumberOfPieces.intValue]);
            LoadArray();
            NumberOfPiecesPerSide = NumberOfPieces.intValue;
        }

        private void CheckWholeSide(int selected, bool check)
        {
            for (int y = 0; y < EuclidEngineChamberArea.Y; y++)
            {
                for (int x = 0; x < EuclidEngineChamberArea.Y; x++)
                {
                    test.ArrayOfSides[selected][x, y] = check;
                }
            }
        }

        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            test = target as EuclidEngineChamberArea;
            GUI.changed = false;
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
            EditorGUILayout.PropertyField(Position);


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Side Modification:");
            GUILayout.BeginHorizontal();
            NumberOfPiecesPerSide = EditorGUILayout.IntField(NumberOfPiecesPerSide);

            //EditorGUILayout.PropertyField(NumberOfPieces);

            if (NumberOfPieces.intValue < 1)
                NumberOfPieces.intValue = 1;
            if (NumberOfPiecesPerSide < 1)
                NumberOfPiecesPerSide = 1;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Validate numbers of pieces per side"))
            {
                NumberOfPieces.intValue = NumberOfPiecesPerSide;
                EuclidEngineChamberArea.Y = NumberOfPieces.intValue;
                test.changeY(EuclidEngineChamberArea.Y);
                toSave.Clear();
                for (int i = 0; i < 6; ++i)
                    toSave.Add(new int[NumberOfPieces.intValue, NumberOfPieces.intValue]);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check whole side"))
                CheckWholeSide(selected, true);
            if (GUILayout.Button("Uncheck whole side"))
                CheckWholeSide(selected, false);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            selected = EditorGUILayout.Popup("Side to edit", selected, options);

            // Print Beautiful and scalable array
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            for (int y = 0; y < EuclidEngineChamberArea.Y; y++)
            {
                EditorGUILayout.BeginVertical();
                for (int x = 0; x < EuclidEngineChamberArea.Y; x++)
                {
                    test.ArrayOfSides[selected][x, y] = EditorGUILayout.Toggle(test.ArrayOfSides[selected][x, y]);
                    //test.modifySide(selected, test.ArrayOfSides[selected][x, y], y, x);
                    Debug.Log(x + " " + y);
                    toSave[selected][x, y] = (test.ArrayOfSides[selected][x, y]) ? 1 : 0;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Save modification made to side"))
                SetKeySide();

            if (GUI.changed)
                EditorUtility.SetDirty(test);

            serializedObject.ApplyModifiedProperties();
        }

        private void SetKeySide()
        {

            if (NumberOfPiecesPerSide != 1 && test.Walls.Count != 0 && !test.check())
            {
                Debug.Log("Check failed");
                return;
            }

            NumberOfPieces.intValue = NumberOfPiecesPerSide;
            PlayerPrefs.SetInt("Length", NumberOfPieces.intValue);

            for (int num = 0; num < 6; ++num)
            {
                string saveSide = "";

                for (int i = 0; i < NumberOfPieces.intValue; ++i)
                {
                    if (i != 0)
                        saveSide += "\n";
                    for (int j = 0; j < NumberOfPieces.intValue; ++j)
                    {
                        saveSide += string.Join(",", (test.ArrayOfSides[num][i, j]) ? 1 : 0);
                        saveSide += (1 + j == NumberOfPieces.intValue) ? "" : ",";
                    }
                }

                PlayerPrefs.SetInt("Length", NumberOfPieces.intValue);
                PlayerPrefs.SetString(options[num], saveSide);
                Debug.Log(PlayerPrefs.GetString(options[num]));
            }
            PlayerPrefs.Save();
            Debug.Log("Has been saved: " + PlayerPrefs.GetInt("Length"));
            test.UpdateFaces();
        }
    }
}
