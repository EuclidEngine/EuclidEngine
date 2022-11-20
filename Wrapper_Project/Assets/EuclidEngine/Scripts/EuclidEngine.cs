using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EuclidEngine
{

    #region Main page documentation
    /// @file EuclidEngine.cs

    /// @mainpage notitle
    /// @image html LogoEE.png width=200px
    /// @section eeSect Euclid Engine (EE)
    /// - Website: <a href="/">Site</a>
    /// - Github: <a href="https://github.com/EuclidEngine">EuclidEngine</a>
    /// - <a href="modules.html">API documentation</a>
    /// - Contact us: <a href="mailto:euclid.engine@gmail.com">euclid.engine@gmail.com</a>

    /// @addtogroup ee Euclid Engine (EE)
    /// @{
    /// @defgroup cpp C++ Objects types
    ///
    /// @brief Objects and functions defined in C++
    ///
    /// The C++ part of Euclid Engine is not accessible to public.
    /// If you still manage to got some access, know that this part may not
    /// be of any use, except if you may want to try to understand how does it work.
    ///
    /// For the public part of Euclid Engine, you should refer to <a href="group__cs.html">the C# part</a>.
#if DEBUG
    /// The C++ part is reachable from <a href="../../cpp/doxygen/group__cpp.html">here</a>.
#endif
    ///
    /// @defgroup cs C# Objects type - Unity
    ///
    /// @brief Objects and functions defined in C#, for the used with <a href="https://unity.com">Unity</a>
    ///
    /// This C# part of Euclid Engine is freely accessible on a demonstration version
    /// or as a complete version on our <a href="">website</a>.
    ///
    /// This documentation is made for the complete version. Therefore, some objects
    /// and functions may not be available on your version.
    /// @}
    #endregion

    /// @brief The EnclidEngine default class, including variables and functions used in several of our classes. 
    /// @ingroup cs
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
}