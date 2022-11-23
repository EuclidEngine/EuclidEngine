using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EuclidEnginePlayTime
{
    private double totalTime = 0.0f;
    private string name;

    public void SetName<T>()
    {
        name = typeof(T).ToString();
    }

    public void AddTime()
    {
        totalTime += Time.deltaTime;
    }

    public void OnDestroy<T>()
    {
        name = typeof(T).ToString();
        Debug.Log("oprite tg: " + name);
        EuclidEngineAPI.SendPlaytime(EuclidWindow.publicUserEmail, name, (int)totalTime / 1000);
    }
}
