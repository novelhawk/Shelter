using JetBrains.Annotations;
using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Drag and Drop Item")]
// ReSharper disable once CheckNamespace
public class DragDropItem : MonoBehaviour
{
    private bool mIsDragging;
    private Transform mParent;
    private bool mSticky;
    private Transform mTrans;
    public GameObject prefab;

    private void Awake()
    {
        this.mTrans = transform;
    }

    private void Drop()
    {
        Collider c = UICamera.lastHit.collider;
        DragDropContainer container = c == null ? null : c.gameObject.GetComponent<DragDropContainer>();
        if (container != null)
        {
            this.mTrans.parent = container.transform;
            Vector3 localPosition = this.mTrans.localPosition;
            localPosition.z = 0f;
            this.mTrans.localPosition = localPosition;
        }
        else
        {
            this.mTrans.parent = this.mParent;
        }
        this.UpdateTable();
        NGUITools.MarkParentAsChanged(gameObject);
    }

    [UsedImplicitly]
    private void OnDrag(Vector2 delta)
    {
        if (enabled && UICamera.currentTouchID > -2)
        {
            if (!this.mIsDragging)
            {
                this.mIsDragging = true;
                this.mParent = this.mTrans.parent;
                this.mTrans.parent = DragDropRoot.root;
                Vector3 localPosition = this.mTrans.localPosition;
                localPosition.z = 0f;
                this.mTrans.localPosition = localPosition;
                NGUITools.MarkParentAsChanged(gameObject);
            }
            else
            {
                this.mTrans.localPosition += (Vector3)delta;
            }
        }
    }

    [UsedImplicitly]
    private void OnPress(bool isPressed)
    {
        if (enabled)
        {
            if (isPressed)
            {
                if (!UICamera.current.stickyPress)
                {
                    this.mSticky = true;
                    UICamera.current.stickyPress = true;
                }
            }
            else if (this.mSticky)
            {
                this.mSticky = false;
                UICamera.current.stickyPress = false;
            }
            this.mIsDragging = false;
            Collider c = this.collider;
            if (c != null)
                c.enabled = !isPressed;
            if (!isPressed)
                this.Drop();
        }
    }

    private void UpdateTable()
    {
        UITable table = NGUITools.FindInParents<UITable>(gameObject);
        if (table != null)
        {
            table.repositionNow = true;
        }
    }
}

