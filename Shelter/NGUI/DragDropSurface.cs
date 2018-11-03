using JetBrains.Annotations;
using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Drag and Drop Surface")]
// ReSharper disable once CheckNamespace
public class DragDropSurface : MonoBehaviour
{
    public bool rotatePlacedObject;

    [UsedImplicitly]
    private void OnDrop(GameObject go)
    {
        DragDropItem component = go.GetComponent<DragDropItem>();
        if (component != null)
        {
            Transform tr = NGUITools.AddChild(gameObject, component.prefab).transform;
            tr.position = UICamera.lastHit.point;
            if (this.rotatePlacedObject)
            {
                tr.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
            }
            Destroy(go);
        }
    }
}

