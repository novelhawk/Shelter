using ExitGames.Client.Photon;
using System;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class Extensions
{
    public static bool AlmostEquals(this float target, float second, float floatDiff)
    {
        return Mathf.Abs(target - second) < floatDiff;
    }

    public static bool AlmostEquals(this Quaternion target, Quaternion second, float maxAngle)
    {
        return Quaternion.Angle(target, second) < maxAngle;
    }

    public static bool AlmostEquals(this Vector2 target, Vector2 second, float sqrMagnitudePrecision)
    {
        Vector2 vector = target - second;
        return vector.sqrMagnitude < sqrMagnitudePrecision;
    }

    public static bool AlmostEquals(this Vector3 target, Vector3 second, float sqrMagnitudePrecision)
    {
        Vector3 vector = target - second;
        return vector.sqrMagnitude < sqrMagnitudePrecision;
    }

    public static bool Contains(this int[] target, int nr)
    {
        if (target != null)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] == nr)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static PhotonView GetPhotonView(this GameObject go)
    {
        return go.GetComponent<PhotonView>();
    }

    public static PhotonView[] GetPhotonViewsInChildren(this GameObject go)
    {
        return go.GetComponentsInChildren<PhotonView>(true);
    }

    public static void Merge(this IDictionary target, IDictionary addHash)
    {
        if (addHash != null && !target.Equals(addHash))
        {
            foreach (object current in addHash.Keys)
            {
                target[current] = addHash[current];
            }
        }
    }

    public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
    {
        if (addHash != null && !target.Equals(addHash))
        {
            foreach (object current in addHash.Keys)
            {
                if (current is string)
                    target[current] = addHash[current];
            }
        }
    }

    public static void StripKeysWithNullValues(this IDictionary original)
    {
        var keys = new object[original.Keys.Count];
        original.Keys.CopyTo(keys, 0);
        for (int i = 0; i < original.Count; i++)
        {
            var key = keys[i];
            if (original[key] == null)
                original.Remove(key);
        }
    }

    public static Hashtable StripToStringKeys(this IDictionary original)
    {
        Hashtable hashtable = new Hashtable();
        foreach (DictionaryEntry current in original)
        {
            if (current.Key is string key)
                hashtable[key] = current.Value;
        }
        return hashtable;
    }

    public static string ToStringFull(this IDictionary origin)
    {
        return SupportClass.DictionaryToString(origin, false);
    }
}

