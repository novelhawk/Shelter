using UnityEngine;

// ReSharper disable once CheckNamespace
public class RCRegionLabel : MonoBehaviour
{
    public GameObject myLabel;

    private void Update()
    {
        if (this.myLabel != null && this.myLabel.GetComponent<UILabel>().isVisible)
        {
            this.myLabel.transform.LookAt(2f * this.myLabel.transform.position - Camera.main.transform.position);
        }
    }
}

