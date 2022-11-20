using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class EuclidEnginePlayTime : MonoBehaviour
{
    private double startingPoint = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        startingPoint = EditorApplication.timeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        EuclidEngineAPI.SendPlaytime(EuclidWindow.publicUserEmail, EditorApplication.timeSinceStartup - startingPoint);
    }
}
