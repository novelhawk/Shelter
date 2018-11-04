using UnityEngine;

// ReSharper disable once CheckNamespace
public class ParentFollow : MonoBehaviour // Used by skin (ex. mikasa_asset_uni)
{
    private Transform bTransform;
    public bool isActiveInScene;
    private Transform parent;

    private void Awake()
    {
        this.bTransform = transform;
        this.isActiveInScene = true;
    }

    public void RemoveParent()
    {
        this.parent = null;
    }

    public void SetParent(Transform transform)
    {
        this.parent = transform;
        this.bTransform.rotation = transform.rotation;
    }

    private void Update()
    {
        if (this.isActiveInScene && this.parent != null)
        {
            this.bTransform.position = this.parent.position;
        }
    }
}

