using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Play Animation")]
public class UIButtonPlayAnimation : MonoBehaviour
{
    public string callWhenFinished;
    public bool clearSelection;
    public string clipName;
    public DisableCondition disableWhenFinished;
    public GameObject eventReceiver;
    public EnableCondition ifDisabledOnPlay;
    private bool mHighlighted;
    private bool mStarted;
    public ActiveAnimation.OnFinished onFinished;
    public Direction playDirection = Direction.Forward;
    public bool resetOnPlay;
    public Animation target;
    public Trigger trigger;

    private void OnActivate(bool isActive)
    {
        if (enabled && (this.trigger == Trigger.OnActivate || this.trigger == Trigger.OnActivateTrue && isActive || this.trigger == Trigger.OnActivateFalse && !isActive))
        {
            this.Play(isActive);
        }
    }

    private void OnClick()
    {
        if (enabled && this.trigger == Trigger.OnClick)
        {
            this.Play(true);
        }
    }

    private void OnDoubleClick()
    {
        if (enabled && this.trigger == Trigger.OnDoubleClick)
        {
            this.Play(true);
        }
    }

    private void OnEnable()
    {
        if (this.mStarted && this.mHighlighted)
        {
            this.OnHover(UICamera.IsHighlighted(gameObject));
        }
    }

    private void OnHover(bool isOver)
    {
        if (enabled)
        {
            if (this.trigger == Trigger.OnHover || this.trigger == Trigger.OnHoverTrue && isOver || this.trigger == Trigger.OnHoverFalse && !isOver)
            {
                this.Play(isOver);
            }
            this.mHighlighted = isOver;
        }
    }

    private void OnPress(bool isPressed)
    {
        if (enabled && (this.trigger == Trigger.OnPress || this.trigger == Trigger.OnPressTrue && isPressed || this.trigger == Trigger.OnPressFalse && !isPressed))
        {
            this.Play(isPressed);
        }
    }

    private void OnSelect(bool isSelected)
    {
        if (enabled && (this.trigger == Trigger.OnSelect || this.trigger == Trigger.OnSelectTrue && isSelected || this.trigger == Trigger.OnSelectFalse && !isSelected))
        {
            this.Play(true);
        }
    }

    private void Play(bool forward)
    {
        if (this.target == null)
        {
            this.target = GetComponentInChildren<Animation>();
        }
        if (this.target != null)
        {
            if (this.clearSelection && UICamera.selectedObject == gameObject)
            {
                UICamera.selectedObject = null;
            }
            int num = -(int)this.playDirection;
            Direction playDirection = !forward ? (Direction) num : this.playDirection;
            ActiveAnimation animation = ActiveAnimation.Play(this.target, this.clipName, playDirection, this.ifDisabledOnPlay, this.disableWhenFinished);
            if (animation != null)
            {
                if (this.resetOnPlay)
                {
                    animation.Reset();
                }
                animation.onFinished = this.onFinished;
                if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
                {
                    animation.eventReceiver = this.eventReceiver;
                    animation.callWhenFinished = this.callWhenFinished;
                }
                else
                {
                    animation.eventReceiver = null;
                }
            }
        }
    }

    private void Start()
    {
        this.mStarted = true;
    }
}

