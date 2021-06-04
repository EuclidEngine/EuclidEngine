using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// @brief The EnclidEngine default class, including variables and functions used in several of our classes. 
public class EuclidEngine
{
    /// @brief Plugin name of the C++ Area DLL/SO
#if UNITY_IOS
    public const string plugin = "__Internal";
#else
    public const string plugin = "EEPluginCpp";
#endif

    /// @brief Function used to retrieve the Object associated to an ID.
    /// This will eventually be removed.
    public static Func<int, UnityEngine.Object> FindObjectFromInstanceID = null;

    static EuclidEngine()
    {
        FindObjectFromInstanceID = (Func<int, UnityEngine.Object>)Delegate.CreateDelegate(
            typeof(Func<int, UnityEngine.Object>),
            typeof(UnityEngine.Object).GetMethod("FindObjectFromInstanceID",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
    }
}