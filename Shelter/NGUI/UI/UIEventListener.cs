using JetBrains.Annotations;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Event Listener")]
// ReSharper disable once CheckNamespace
public class UIEventListener : MonoBehaviour
{
    public VoidDelegate onClick;
    public VoidDelegate onDoubleClick;
    public VectorDelegate onDrag;
    public ObjectDelegate onDrop;
    public BoolDelegate onHover;
    public StringDelegate onInput;
    public KeyCodeDelegate onKey;
    public BoolDelegate onPress;
    public FloatDelegate onScroll;
    public BoolDelegate onSelect;
    public VoidDelegate onSubmit;
    public object parameter;

    public static UIEventListener Get(GameObject go)
    {
        UIEventListener component = go.GetComponent<UIEventListener>();
        if (component == null)
        {
            component = go.AddComponent<UIEventListener>();
        }
        return component;
    }

    [UsedImplicitly]
    private void OnClick()
    {
        if (this.onClick != null)
        {
            this.onClick(gameObject);
        }
    }

    [UsedImplicitly]
    private void OnDoubleClick()
    {
        if (this.onDoubleClick != null)
        {
            this.onDoubleClick(gameObject);
        }
    }

    [UsedImplicitly]
    private void OnDrag(Vector2 delta)
    {
        if (this.onDrag != null)
        {
            this.onDrag(gameObject, delta);
        }
    }

    [UsedImplicitly]
    private void OnDrop(GameObject go)
    {
        if (this.onDrop != null)
        {
            this.onDrop(gameObject, go);
        }
    }

    [UsedImplicitly]
    private void OnHover(bool isOver)
    {
        if (this.onHover != null)
        {
            this.onHover(gameObject, isOver);
        }
    }

    [UsedImplicitly]
    private void OnInput(string text)
    {
        if (this.onInput != null)
        {
            this.onInput(gameObject, text);
        }
    }

    [UsedImplicitly]
    private void OnKey(KeyCode key)
    {
        if (this.onKey != null)
        {
            this.onKey(gameObject, key);
        }
    }

    [UsedImplicitly]
    private void OnPress(bool isPressed)
    {
        if (this.onPress != null)
        {
            this.onPress(gameObject, isPressed);
        }
    }

    [UsedImplicitly]
    private void OnScroll(float delta)
    {
        if (this.onScroll != null)
        {
            this.onScroll(gameObject, delta);
        }
    }

    [UsedImplicitly]
    private void OnSelect(bool selected)
    {
        if (this.onSelect != null)
        {
            this.onSelect(gameObject, selected);
        }
    }

    [UsedImplicitly]
    private void OnSubmit()
    {
        if (this.onSubmit != null)
        {
            this.onSubmit(gameObject);
        }
    }

    public delegate void BoolDelegate(GameObject go, bool state);

    public delegate void FloatDelegate(GameObject go, float delta);

    public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

    public delegate void ObjectDelegate(GameObject go, GameObject draggedObject);

    public delegate void StringDelegate(GameObject go, string text);

    public delegate void VectorDelegate(GameObject go, Vector2 delta);

    public delegate void VoidDelegate(GameObject go);
}

