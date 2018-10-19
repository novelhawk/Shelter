using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
// ReSharper disable once CheckNamespace
public class UIButton : UIButtonColor
{
    public Color disabledColor = Color.grey;

    protected override void OnEnable()
    {
        if (this.isEnabled)
        {
            base.OnEnable();
        }
        else
        {
            this.UpdateColor(false, true);
        }
    }

    public override void OnHover(bool isOver)
    {
        if (this.isEnabled)
        {
            base.OnHover(isOver);
        }
    }

    public override void OnPress(bool isPressed)
    {
        if (this.isEnabled)
        {
            base.OnPress(isPressed);
        }
    }

    public void UpdateColor(bool shouldBeEnabled, bool immediate)
    {
        if (tweenTarget != null)
        {
            if (!mStarted)
            {
                mStarted = true;
                Init();
            }
            Color color = !shouldBeEnabled ? this.disabledColor : defaultColor;
            TweenColor color2 = TweenColor.Begin(tweenTarget, 0.15f, color);
            if (immediate)
            {
                color2.color = color;
                color2.enabled = false;
            }
        }
    }

    public bool isEnabled
    {
        get
        {
            Collider collider = this.collider;
            return collider != null && collider.enabled;
        }
        set
        {
            Collider collider = this.collider;
            if (collider != null && collider.enabled != value)
            {
                collider.enabled = value;
                this.UpdateColor(value, false);
            }
        }
    }
}

