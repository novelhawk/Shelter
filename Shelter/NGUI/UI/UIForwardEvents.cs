using JetBrains.Annotations;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Forward Events")]
// ReSharper disable once CheckNamespace
public class UIForwardEvents : MonoBehaviour
{
    public bool onClick;
    public bool onDoubleClick;
    public bool onDrag;
    public bool onDrop;
    public bool onHover;
    public bool onInput;
    public bool onPress;
    public bool onScroll;
    public bool onSelect;
    public bool onSubmit;
    public GameObject target;

    [UsedImplicitly]
    private void OnClick()
    {
        if (this.onClick && this.target != null)
        {
            this.target.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
        }
    }

    [UsedImplicitly]
    private void OnDoubleClick()
    {
        if (this.onDoubleClick && this.target != null)
        {
            this.target.SendMessage("OnDoubleClick", SendMessageOptions.DontRequireReceiver);
        }
    }

    [UsedImplicitly]
    private void OnDrag(Vector2 delta)
    {
        if (this.onDrag && this.target != null)
        {
            this.target.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
        }
    }

    [UsedImplicitly]
    private void OnDrop(GameObject go)
    {
        if (this.onDrop && this.target != null)
        {
            this.target.SendMessage("OnDrop", go, SendMessageOptions.DontRequireReceiver);
        }
    }

    [UsedImplicitly]
    private void OnHover(bool isOver)
    {
        if (this.onHover && this.target != null)
        {
            this.target.SendMessage("OnHover", isOver, SendMessageOptions.DontRequireReceiver);
        }
    }

    [UsedImplicitly]
    private void OnInput(string text)
    {
        if (this.onInput && this.target != null)
        {
            this.target.SendMessage("OnInput", text, SendMessageOptions.DontRequireReceiver);
        }
    }

    [UsedImplicitly]
    private void OnPress(bool pressed)
    {
        if (this.onPress && this.target != null)
        {
            this.target.SendMessage("OnPress", pressed, SendMessageOptions.DontRequireReceiver);
        }
    }

    [UsedImplicitly]
    private void OnScroll(float delta)
    {
        if (this.onScroll && this.target != null)
        {
            this.target.SendMessage("OnScroll", delta, SendMessageOptions.DontRequireReceiver);
        }
    }

    [UsedImplicitly]
    private void OnSelect(bool selected)
    {
        if (this.onSelect && this.target != null)
        {
            this.target.SendMessage("OnSelect", selected, SendMessageOptions.DontRequireReceiver);
        }
    }

    [UsedImplicitly]
    private void OnSubmit()
    {
        if (this.onSubmit && this.target != null)
        {
            this.target.SendMessage("OnSubmit", SendMessageOptions.DontRequireReceiver);
        }
    }
}

