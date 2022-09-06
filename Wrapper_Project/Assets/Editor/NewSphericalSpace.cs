using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;

public class NewSphericalSpace : EditorWindow
{
    /// @brief Callback used to display the main EuclidEngine window
    [MenuItem("File/New Spherical Space", false, 2)]
    public static void ShowWindow()
    {
        var window = GetWindow<NewSphericalSpace>("Euclid Engine");

        window.titleContent = new GUIContent("Euclid Engine");
    }
    
    /// @brief Called on enablement of the window.
    private void OnEnable()
    {
    }

    bool shunksOpen = true;
    float worldSize = 1;
    GameObject[] gos = new GameObject[6];
    float[] rots = new float[6];

    /// @brief Contains buttons that apply Euclid Engine features
    private void OnGUI()
    {
        worldSize = Mathf.Max(EditorGUILayout.FloatField("World size", worldSize), 1e-5f);
        shunksOpen = EditorGUILayout.BeginFoldoutHeaderGroup(shunksOpen, "Shunks");
        if (shunksOpen) {
            for (int i = 0; i < 6; ++i) {
                EditorGUILayout.BeginHorizontal();
                gos[i] = (GameObject)EditorGUILayout.ObjectField(gos[i], typeof(GameObject), false);
                rots[i] = (float)EditorGUILayout.IntPopup("Rotation", (int)rots[i], new string[]{"None","Clockwise","Anti-Clockwise","Half-Turn"}, new int[]{0,90,-90,180});
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        bool done = GUILayout.Button("Create space");

        if (done && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            Close();
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            var space = new GameObject().AddComponent<EuclidEngineSpace>();
            space.gameObject.name = "SphericalSpace";
            space.prefabs = gos;
            space.prefRot = rots;
            space.worldRadius = worldSize;
            space.mainCamera = Object.FindObjectOfType<Camera>();
        }
    }
}
