// ----------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Provides some helpful methods and extensions for Hashtables, etc.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System.Collections;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon
{
    public static class Extensions
    {
        /// <summary>Compares two floats and returns true of their difference is less than <see cref="precision"/></summary>
        public static bool AlmostEquals(this float current, float other, float precision)
        {
            return Mathf.Abs(current - other) < precision;
        }

        /// <summary>Compares the angle between current and second to given float value</summary>
        public static bool AlmostEquals(this Quaternion current, Quaternion other, float maxAngle)
        {
            return Quaternion.Angle(current, other) < maxAngle;
        }

        /// <summary>Compares the squared magnitude of current - second to given float value</summary>
        public static bool AlmostEquals(this Vector2 current, Vector2 other, float sqrMagnitudePrecision)
        {
            Vector2 vector = current - other;
            return vector.sqrMagnitude < sqrMagnitudePrecision;
        }

        /// <summary>Compares the squared magnitude of current - second to given float value</summary>
        public static bool AlmostEquals(this Vector3 current, Vector3 other, float sqrMagnitudePrecision)
        {
            Vector3 vector = current - other;
            return vector.sqrMagnitude < sqrMagnitudePrecision;
        }

        /// <summary>
        /// Checks if a particular integer value is in an int-array.
        /// </summary>
        /// <remarks>This might be useful to look up if a particular actorNumber is in the list of players of a room.</remarks>
        /// <param name="array">The array of ints to check.</param>
        /// <param name="entry">The number to lookup in array.</param>
        /// <returns>True if <see cref="entry"/> was found in array.</returns>
        public static bool Contains(this int[] array, int entry)
        {
            if (array == null)
                return false;

            return array.Any(x => x == entry);
        }

        public static PhotonView GetPhotonView(this GameObject go)
        {
            return go.GetComponent<PhotonView>();
        }

        public static PhotonView[] GetPhotonViewsInChildren(this GameObject go)
        {
            return go.GetComponentsInChildren<PhotonView>(true);
        }

        /// <summary>
        /// Merges all keys from addHash into the array. Adds new keys and updates the values of existing keys in array.
        /// </summary>
        /// <param name="target">The IDictionary to update.</param>
        /// <param name="addHash">The IDictionary containing data to merge into array.</param>
        public static void Merge(this IDictionary target, IDictionary addHash)
        {
            if (addHash == null || target.Equals(addHash))
                return;

            foreach (object key in addHash.Keys)
                target[key] = addHash[key];
        }

        /// <summary>
        /// Merges keys of type string to array Hashtable.
        /// </summary>
        /// <remarks>
        /// Does not remove keys from array (so non-string keys CAN be in array if they were before).
        /// </remarks>
        /// <param name="target">The array IDicitionary passed in plus all string-typed keys from the addHash.</param>
        /// <param name="addHash">A IDictionary that should be merged partly into array to update it.</param>
        public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
        {
            if (addHash == null || target.Equals(addHash))
                return;

            foreach (object key in addHash.Keys)
            {
                // only merge keys of type string
                if (key is string)
                    target[key] = addHash[key];
            }
        }
        
        /// <summary>
        /// This removes all key-value pairs that have a null-reference as value.
        /// Photon properties are removed by setting their value to null.
        /// Changes the original passed IDictionary!
        /// </summary>
        /// <param name="original">The IDictionary to strip of keys with null-values.</param>
        public static void StripKeysWithNullValues(this IDictionary original)
        {
            object[] keys = new object[original.Count];
            original.Keys.CopyTo(keys, 0); // Incompatible with some platforms
//            int i = 0;
//            foreach (object k in original.Keys)
//                keys[i++] = k;

            foreach (var key in keys)
            {
                if (original[key] == null)
                    original.Remove(key);
            }
        }

        /// <summary>
        /// This method copies all string-typed keys of the original into a new Hashtable.
        /// </summary>
        /// <remarks>
        /// Does not recurse (!) into hashes that might be values in the root-hash.
        /// This does not modify the original.
        /// </remarks>
        /// <param name="original">The original IDictonary to get string-typed keys from.</param>
        /// <returns>New Hashtable containing only string-typed keys of the original.</returns>
        public static Hashtable StripToStringKeys(this IDictionary original)
        {
            Hashtable target = new Hashtable();
            if (original != null)
            {
                foreach (object key in original.Keys)
                {
                    if (key is string)
                    {
                        target[key] = original[key];
                    }
                }
            }

            return target;
        }

        /// <summary>Helper method for debugging of IDictionary content, inlcuding type-information. Using this is not performant.</summary>
        /// <remarks>Should only be used for debugging as necessary.</remarks>
        /// <param name="origin">Some Dictionary or Hashtable.</param>
        /// <returns>String of the content of the IDictionary.</returns>
        public static string ToStringFull(this IDictionary origin)
        {
            return SupportClass.DictionaryToString(origin, false);
        }
        
        /// <summary>Helper method for debugging of object[] content. Using this is not performant.</summary>
        /// <remarks>Should only be used for debugging as necessary.</remarks>
        /// <param name="data">Any object[].</param>
        /// <returns>A comma-separated string containing each value's ToString().</returns>
        public static string ToStringFull(this object[] data)
        {
            if (data == null) return "null";

            string[] sb = new string[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                object o = data[i];
                sb[i] = (o != null) ? o.ToString() : "null";
            }

            return string.Join(", ", sb);
        }
    }
}

