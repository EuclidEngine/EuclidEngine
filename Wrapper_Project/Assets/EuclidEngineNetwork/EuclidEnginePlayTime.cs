using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
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

    public void AddTime(double time)
    {
        totalTime += time;
    }

    public void OnDestroy<T>()
    {
        if (totalTime == 0.0f) return;

        name = typeof(T).ToString();
        EuclidEngineAPI.SendPlaytime(EuclidWindow.publicUserEmail, name, (int)totalTime);
    }
}

#endif