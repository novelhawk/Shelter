using UnityEngine;

// ReSharper disable once CheckNamespace
public class CameraFacingBillboard : MonoBehaviour
{
    public Axis axis;
    private Camera referenceCamera;
    public bool reverseFace;

    private void Awake()
    {
        if (this.referenceCamera == null)
            this.referenceCamera = Camera.main;
    }

    private static Vector3 GetAxisVector(Axis axis)
    {
        switch (axis)
        {
            case Axis.Down:
                return Vector3.down;

            case Axis.Left:
                return Vector3.left;

            case Axis.Right:
                return Vector3.right;

            case Axis.Forward:
                return Vector3.forward;

            case Axis.Back:
                return Vector3.back;
        }
        return Vector3.up;
    }

    private void Update()
    {
        Vector3 worldPosition = transform.position + this.referenceCamera.transform.rotation * (!this.reverseFace ? Vector3.back : Vector3.forward);
        Vector3 worldUp = this.referenceCamera.transform.rotation * GetAxisVector(this.axis);
        transform.LookAt(worldPosition, worldUp);
    }

    public enum Axis
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back
    }
}

