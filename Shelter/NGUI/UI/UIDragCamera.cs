using JetBrains.Annotations;
using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Camera"), ExecuteInEditMode]
// ReSharper disable once CheckNamespace
public class UIDragCamera : IgnoreTimeScale
{
    public UIDraggableCamera draggableCamera;
    [SerializeField, HideInInspector]
    private Component target;

    private void Awake()
    {
        if (this.target != null)
        {
            if (this.draggableCamera == null)
            {
                this.draggableCamera = this.target.GetComponent<UIDraggableCamera>();
                if (this.draggableCamera == null)
                {
                    this.draggableCamera = this.target.gameObject.AddComponent<UIDraggableCamera>();
                }
            }
            this.target = null;
        }
        else if (this.draggableCamera == null)
        {
            this.draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(gameObject);
        }
    }

    [UsedImplicitly]
    private void OnDrag(Vector2 delta)
    {
        if (enabled && NGUITools.GetActive(gameObject) && this.draggableCamera != null)
        {
            this.draggableCamera.Drag(delta);
        }
    }

    [UsedImplicitly]
    private void OnPress(bool isPressed)
    {
        if (enabled && NGUITools.GetActive(gameObject) && this.draggableCamera != null)
        {
            this.draggableCamera.Press(isPressed);
        }
    }

    [UsedImplicitly]
    private void OnScroll(float delta)
    {
        if (enabled && NGUITools.GetActive(gameObject) && this.draggableCamera != null)
        {
            this.draggableCamera.Scroll(delta);
        }
    }
}

