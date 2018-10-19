using UnityEngine;

[AddComponentMenu("NGUI/Examples/Drag and Drop Root")]
// ReSharper disable once CheckNamespace
public class DragDropRoot : MonoBehaviour
{
    public static Transform root;

    private void Awake()
    {
        root = transform;
    }
}

