using UnityEditor;
using UnityEngine;
using System;

#if UNITY_EDITOR
namespace EuclidEngine
{
    /// @brief Contains any code related to EuclidEngineArea Editor UI.
    [CustomEditor(typeof(Area))]
    [CanEditMultipleObjects]
    [InitializeOnLoad]
    public class AreaEditor : Editor
    {
        /// @brief Function containing the creation of a basic area
        /// @param areaSize External 3D dimensions
        /// @param internalSize Internal 3D dimensions
        /// @param position
        /// @param rotation 3D Rotation Matrix
        public static void CreateMenuCreateArea(Vector3 areaSize, Vector3 internalSize, Vector3 position, Quaternion rotation)
        {
            var obj = Area.Instantiate(areaSize, internalSize, position, rotation);
            obj.gameObject.name = "New EuclidEngineArea";
        }

        /// @brief Overload of CreateMenuCreateArea for context menu
        [MenuItem("GameObject/Euclid Engine/Create Area", false, -1)]
        public static void CreateMenuCreateArea()
        {
            var obj = Area.Instantiate(Vector3.one, Vector3.one, Vector3.one, Quaternion.identity);
            obj.gameObject.name = "New EuclidEngineArea";
        }

        SerializedProperty size;
        SerializedProperty internalSize;
        SerializedProperty transitSize;
        /// @brief Editor Constructor
        void OnEnable()
        {
            if (logoTexture == null)
                logoTexture = Resources.Load("Icons/background") as Texture2D;

            size = serializedObject.FindProperty("_size");
            internalSize = serializedObject.FindProperty("_internalSize");
            transitSize = serializedObject.FindProperty("_transitSize");
        }

        /// @brief Displays the centered Euclid Engine label
        static Texture2D logoTexture = null;
        private void DisplayCenteredLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logoTexture, GUILayout.Height(75), GUILayout.Width(75));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            DisplayCenteredLogo();

            EditorGUILayout.LabelField("Area Attributes :");

            size = serializedObject.FindProperty("_size");
            internalSize = serializedObject.FindProperty("_internalSize");
            transitSize = serializedObject.FindProperty("_transitSize");

            serializedObject.Update();
            EditorGUILayout.PropertyField(size);
            EditorGUILayout.PropertyField(internalSize);
            EditorGUILayout.PropertyField(transitSize);
            serializedObject.ApplyModifiedProperties();
        }
    }
};
#endif