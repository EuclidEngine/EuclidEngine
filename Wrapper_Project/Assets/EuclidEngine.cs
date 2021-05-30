using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class EuclidEngine
{
    public static Func<int, UnityEngine.Object> FindObjectFromInstanceID = null;
    static EuclidEngine()
    {
        FindObjectFromInstanceID = (Func<int, UnityEngine.Object>)Delegate.CreateDelegate(
            typeof(Func<int, UnityEngine.Object>),
            typeof(UnityEngine.Object).GetMethod("FindObjectFromInstanceID",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
    }

#if UNITY_IOS
    private const string eePlugin = "__Internal";
#else
    private const string eePlugin = "EEPluginCpp";
#endif

    /* 
    * @brief Create a new NEA, with given dimension
    * 
    * @param sizeX The size of the object on the X coordinate
    * @param sizeY The size of the object on the Y coordinate
    * @param sizeZ The size of the object on the Z coordinate
    * @param internalSizeX The size of the area inside the object on the X coordinate
    * @param internalSizeY The size of the area inside the object on the Y coordinate
    * @param internalSizeZ The size of the area inside the object on the Z coordinate
    */
    [DllImport(eePlugin)] public static extern IntPtr EEAreaCreate(double iX, double iY, double iZ, double eX, double eY, double eZ);

    /*
     * @brief Destroy the Area object
    */
    [DllImport(eePlugin)] public static extern void EEAreaDelete(IntPtr area);

    /*
    * @brief Set the size of the NEA
    * 
    * @details Adjusting the size of the area may lead to insertion of new object inside the NEA.
    * However, as object inside the NEA are anchored relatively to their internal position, they will stay at the border of the NEA, as this will only change the external size of the NEA.
    *
    * @param Area object
    * @param sizeX The new size of the object on the X coordinate
    * @param sizeY The new size of the object on the Y coordinate
    * @param sizeZ The new size of the object on the Z coordinate
    */
    [DllImport(eePlugin)] public static extern void EEAreaSetSize(IntPtr area, double pExternX, double pExternY, double pExternZ);

    /*
    * @brief Set the size of the area inside the NEA
    * 
    * @details Adjusting the internal size of the area may lead change of position for object and relaese of object inside the NEA.
    * 
    * @param Area object
    * @param sizeX The new size of the area inside the object on the X coordinate
    * @param sizeY The new size of the area inside the object on the Y coordinate
    * @param sizeZ The new size of the area inside the object on the Z coordinate
    */
    [DllImport(eePlugin)] public static extern void EEAreaSetInternalSize(IntPtr area, double pInternX, double pInternY, double pInternZ);

    /*
    * @brief Set the size of the transition area around the object
    * 
    * @details The size will be applied on both side of the object, relatively to the coordinates (right and left, top and bottom, front and back)
    * Behaviour of object inside the transition area is undefined in case of changement of its size.
    * 
    * @param sizeX The new size of the transit area of the object on the X coordinate
    * @param sizeY The new size of the transit area of the object on the Y coordinate
    * @param sizeZ The new size of the transit area of the object on the Z coordinate
    */
    [DllImport(eePlugin)] public static extern void EEAreaSetTransitAreaSize(IntPtr area, double sizeX, double sizeY, double sizeZ);

    /*
    * @brief Add a new object inside the NEA.
    * 
    * @details The object given as argument will be the one used whenever a callback need to be called.
    * 
    * @param Area object
    * @param id A unique identifier for the object
    * @param object The object which enter inside the NEA
    */
    [DllImport(eePlugin)] public static extern void EEAreaAddObjectInside(IntPtr area, int objectId, IntPtr obj);

    /*
    * @brief Remove an object from inside the NEA.
    * 
    * @param Area object
    * @param id The unique identifier identifying the object`
    */
    [DllImport(eePlugin)] public static extern void EEAreaRemoveObjectInside(IntPtr area, int objectId);

    /*
    * @brief Update the objects inside the NEA. Need to be called each frame.
    * 
    * @details Update objects inside the NEA relatively to changes occuring after the last frame.
    * Most callback registered to the NEA will be called on this function, and here only.
    */
    [DllImport(eePlugin)] public static extern void EEAreaUpdate(IntPtr area);

    public delegate void ScalerFn(IntPtr go, double x, double y, double z);
    public delegate void PositionGetterFn(IntPtr go, ref double x, ref double y, ref double z);
    public delegate void SizeGetterFn(IntPtr go, ref double minX, ref double minY, ref double minZ, ref double maxX, ref double maxY, ref double maxZ);
    [DllImport(eePlugin)] public static extern void EEAreaSetAreaPositionGetterCallback(IntPtr area, PositionGetterFn callback);
    [DllImport(eePlugin)] public static extern void EEAreaSetScalerCallback(IntPtr area, ScalerFn callback);
    [DllImport(eePlugin)] public static extern void EEAreaSetPositionGetterCallback(IntPtr area, PositionGetterFn callback);
    [DllImport(eePlugin)] public static extern void EEAreaSetSizeGetterCallback(IntPtr area, SizeGetterFn callback);

    public static EuclidEngineArea CreateArea(double interiorX, double interiorY, double interiorZ, double exteriorX, double exteriorY, double exteriorZ)
    {
        IntPtr intPtr = EEAreaCreate(interiorX, interiorY, interiorZ, exteriorX, exteriorY, exteriorZ);

        GameObject obj = new GameObject();

        EuclidEngineArea area = obj.AddComponent<EuclidEngineArea>();
        area._area = intPtr;
        return area;
    }

    public static void DeleteArea(EuclidEngineArea area)
    {
        EEAreaDelete(area._area);
    }
}