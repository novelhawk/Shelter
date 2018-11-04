using UnityEngine;

// ReSharper disable once CheckNamespace
public class CatchDestroy : MonoBehaviour
{
    public GameObject target;

    private void OnDestroy()
    {
        if (this.target != null)
            Destroy(this.target);
    }
}