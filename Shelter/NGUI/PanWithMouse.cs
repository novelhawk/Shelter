using UnityEngine;

[AddComponentMenu("NGUI/Examples/Pan With Mouse")]
// ReSharper disable once CheckNamespace
public class PanWithMouse : IgnoreTimeScale
{
    public Vector2 degrees = new Vector2(5f, 3f);
    private Vector2 mRot = Vector2.zero;
    private Quaternion mStart;
    private Transform mTrans;
    public float range = 1f;

    private void Start()
    {
        this.mTrans = transform;
        this.mStart = this.mTrans.localRotation;
    }

    private void Update()
    {
        float num = UpdateRealTimeDelta();
        Vector3 mousePosition = Input.mousePosition;
        float num2 = Screen.width * 0.5f;
        float num3 = Screen.height * 0.5f;
        if (this.range < 0.1f)
        {
            this.range = 0.1f;
        }
        float x = Mathf.Clamp((mousePosition.x - num2) / num2 / this.range, -1f, 1f);
        float y = Mathf.Clamp((mousePosition.y - num3) / num3 / this.range, -1f, 1f);
        this.mRot = Vector2.Lerp(this.mRot, new Vector2(x, y), num * 5f);
        this.mTrans.localRotation = this.mStart * Quaternion.Euler(-this.mRot.y * this.degrees.y, this.mRot.x * this.degrees.x, 0f);
    }
}

