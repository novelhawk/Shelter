using Mod;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Debug")]
// ReSharper disable once CheckNamespace
public class NGUIDebug : MonoBehaviour
{
    private void Awake()
    {
        Shelter.Log("{0} cannot be removed.", nameof(NGUIDebug));
    }
}

