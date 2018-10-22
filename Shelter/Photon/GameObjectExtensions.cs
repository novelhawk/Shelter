// ----------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Provides some helpful methods and extensions for Hashtables, etc.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;

namespace Photon
{
    /// <summary>Small number of extension methods that make it easier for PUN to work cross-Unity-versions.</summary>
    public static class GameObjectExtensions
    {
        /// <summary>Unity-version-independent replacement for active GO property.</summary>
        /// <returns>Unity 3.5: active. Any newer Unity: activeInHierarchy.</returns>
        public static bool GetActive(this GameObject target)
        {
#if UNITY_3_5
            return target.active;
#else
            return target.activeInHierarchy;
#endif
        }

#if UNITY_3_5
        /// <summary>Unity-version-independent setter for active and SetActive().</summary>
        public static void SetActive(this GameObject target, bool value)
        {
            target.active = value;
        }
#endif
    }
}

