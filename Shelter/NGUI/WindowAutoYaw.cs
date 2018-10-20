using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Window Auto-Yaw")]
// ReSharper disable once CheckNamespace
public class WindowAutoYaw : MonoBehaviour
{
    private Transform mTrans;
    public Camera uiCamera;
    public int updateOrder;
    public float yawAmount = 20f;

    private void CoroutineUpdate(float delta)
    {
        if (this.uiCamera != null)
        {
            Vector3 vector = this.uiCamera.WorldToViewportPoint(this.mTrans.position);
            this.mTrans.localRotation = Quaternion.Euler(0f, (vector.x * 2f - 1f) * this.yawAmount, 0f);
        }
    }

    private void OnDisable()
    {
        this.mTrans.localRotation = Quaternion.identity;
    }

    private void Start()
    {
        if (this.uiCamera == null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
        }
        this.mTrans = transform;
        UpdateManager.AddCoroutine(this, this.updateOrder, new UpdateManager.OnUpdate(this.CoroutineUpdate));
    }
}

