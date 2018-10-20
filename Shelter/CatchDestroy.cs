using UnityEngine;

public class CatchDestroy : MonoBehaviour
{
    public GameObject target;

    private void OnDestroy()
    {
        if (this.target != null)
            Destroy(this.target);
    }
}