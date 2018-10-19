using NGUI;
using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Activate")]
// ReSharper disable once CheckNamespace
public class UIButtonActivate : MonoBehaviour
{
    public bool state = true;
    public GameObject target;

    private void OnClick()
    {
        if (this.target != null)
        {
            NGUITools.SetActive(this.target, this.state);
        }
    }
}

