using UnityEngine;

[RequireComponent(typeof(UIWidget)), ExecuteInEditMode]
// ReSharper disable once CheckNamespace
public class AnimatedColor : MonoBehaviour
{
    public Color color = Color.white;
    private UIWidget mWidget;

    private void Awake()
    {
        this.mWidget = GetComponent<UIWidget>();
    }

    private void Update()
    {
        this.mWidget.color = this.color;
    }
}

