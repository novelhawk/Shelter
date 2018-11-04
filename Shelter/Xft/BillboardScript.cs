using UnityEngine;

// ReSharper disable once CheckNamespace
public class BillboardScript : MonoBehaviour
{
    public void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(Vector3.left * -90f);
    }
}

