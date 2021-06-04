using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class EuclidEngine
{
#if UNITY_IOS
    public const string plugin = "__Internal";
#else
    public const string plugin = "EEPluginCpp";
#endif

    public static Func<int, UnityEngine.Object> FindObjectFromInstanceID = null;

    static EuclidEngine()
    {
        FindObjectFromInstanceID = (Func<int, UnityEngine.Object>)Delegate.CreateDelegate(
            typeof(Func<int, UnityEngine.Object>),
            typeof(UnityEngine.Object).GetMethod("FindObjectFromInstanceID",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
    }
}