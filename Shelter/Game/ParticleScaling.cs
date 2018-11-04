using UnityEngine;

// ReSharper disable once CheckNamespace
public class ParticleScaling : MonoBehaviour // Used for every particle
{
    public void OnWillRenderObject()
    {
        var material = GetComponent<ParticleSystem>().renderer.material;
        
        material.SetVector("_Center", transform.position);
        material.SetVector("_Scaling", transform.lossyScale);
        material.SetMatrix("_Camera", Camera.current.worldToCameraMatrix);
        material.SetMatrix("_CameraInv", Camera.current.worldToCameraMatrix.inverse);
    }
}

