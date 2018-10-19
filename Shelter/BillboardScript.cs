using UnityEngine;

public class BillboardScript : MonoBehaviour // Probably from Xffect
{
    public void Main()
    {
    }

    public void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(Vector3.left * -90f);
    }
}

