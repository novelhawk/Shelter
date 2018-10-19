using UnityEngine;

// ReSharper disable once CheckNamespace
public class OnStartDelete : MonoBehaviour
{
    private void Start()
    {
        DestroyObject(gameObject);
    }
}

