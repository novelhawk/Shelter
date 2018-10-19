using NGUI;
using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Color")]
// ReSharper disable once CheckNamespace
public class UIButtonColor : MonoBehaviour
{
    public float duration = 0.2f;
    public Color hover = new Color(0.6f, 1f, 0.2f, 1f);
    protected Color mColor;
    protected bool mHighlighted;
    protected bool mStarted;
    public Color pressed = Color.grey;
    public GameObject tweenTarget;

    protected void Init()
    {
        if (this.tweenTarget == null)
        {
            this.tweenTarget = gameObject;
        }
        UIWidget component = this.tweenTarget.GetComponent<UIWidget>();
        if (component != null)
        {
            this.mColor = component.color;
        }
        else
        {
            Renderer renderer = this.tweenTarget.renderer;
            if (renderer != null)
            {
                this.mColor = renderer.material.color;
            }
            else
            {
                Light light = this.tweenTarget.light;
                if (light != null)
                {
                    this.mColor = light.color;
                }
                else
                {
                    Debug.LogWarning(NGUITools.GetHierarchy(gameObject) + " has nothing for UIButtonColor to color", this);
                    enabled = false;
                }
            }
        }
        this.OnEnable();
    }

    private void OnDisable()
    {
        if (this.mStarted && this.tweenTarget != null)
        {
            TweenColor component = this.tweenTarget.GetComponent<TweenColor>();
            if (component != null)
            {
                component.color = this.mColor;
                component.enabled = false;
            }
        }
    }

    protected virtual void OnEnable()
    {
        if (this.mStarted && this.mHighlighted)
        {
            this.OnHover(UICamera.IsHighlighted(gameObject));
        }
    }

    public virtual void OnHover(bool isOver)
    {
        if (enabled)
        {
            if (!this.mStarted)
            {
                this.Start();
            }
            TweenColor.Begin(this.tweenTarget, this.duration, !isOver ? this.mColor : this.hover);
            this.mHighlighted = isOver;
        }
    }

    public virtual void OnPress(bool isPressed)
    {
        if (enabled)
        {
            if (!this.mStarted)
            {
                this.Start();
            }
            TweenColor.Begin(this.tweenTarget, this.duration, !isPressed ? (!UICamera.IsHighlighted(gameObject) ? this.mColor : this.hover) : this.pressed);
        }
    }

    private void Start()
    {
        if (!this.mStarted)
        {
            this.Init();
            this.mStarted = true;
        }
    }

    public Color defaultColor
    {
        get
        {
            if (!this.mStarted)
            {
                this.Init();
            }
            return this.mColor;
        }
        set
        {
            this.mColor = value;
        }
    }
}

