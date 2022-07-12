using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EuclidEngine
{
    /// @brief The Non-Euclidean Area (NEA) object, deserved by Euclid Engine
    ///
    /// @details A NEA is a small space inside another space. It may have different dimensions than the oiginal space.
    ///
    /// This is defined with a different external and internal size. The external size is the size of the Area, defining the space it will occuped on the world. The internal size is the size of the space inside the Area, defining the distance to cross the Area from both end.
    ///
    /// Every object inside a NEA are anchored relatively to their internal position and the center of the area. 
    ///
    /// By default, an EuclidEngineArea as an external and internal size of 1, with no transition area.
    ///
    /// @ingroup cs
    public class EEObject : MonoBehaviour
    {
        /************************************************/
        /*                                              */
        /*           C++ functions prototype            */
        /*                                              */
        /************************************************/

        //[DllImport(EuclidEngine.plugin)] private static extern IntPtr EEAreaCreate(double eX, double eY, double eZ, double iX, double iY, double iZ);


        /************************************************/
        /*                                              */
        /*                   Variables                  */
        /*                                              */
        /************************************************/

        /************************************************/
        /*                                              */
        /*             Unity events handler             */
        /*                                              */
        /************************************************/

        //Called in editor, when creating object
        void Reset()
        {
        }

        GameObject EEObjectContainer;
        //Called on launch
        void Awake()
        {
            EEObjectContainer = new GameObject("EEObjectContainer[" + gameObject.name + "]");
            EEObjectContainer.transform.SetParent(gameObject.transform.parent);
            gameObject.transform.SetParent(EEObjectContainer.transform);
        }

        //Called on launch, after Awake
        void Start()
        {

        }

        //Called at end (of object or scene)
        void OnDestroy()
        {
            gameObject.transform.SetParent(EEObjectContainer.transform.parent);
            Destroy(EEObjectContainer);
        }

        //Called on collision
        void OnTriggerEnter(Collider c)
        {
        }

        //Called at the end of collision
        void OnTriggerExit(Collider c)
        {
        }

        //Called every frame
        void Update()
        {
        }

        /************************************************/
        /*                                              */
        /*                 Constructors                 */
        /*                                              */
        /************************************************/

        /************************************************/
        /*                                              */
        /*              C# public functions             */
        /*                                              */
        /************************************************/

        /************************************************/
        /*                                              */
        /*            C# private functions              */
        /*                                              */
        /************************************************/


        /************************************************/
        /*                                              */
        /*                 C++ callbacks                */
        /*                                              */
        /************************************************/
    }
};