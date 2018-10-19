using NGUI;
using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Tween")]
// ReSharper disable once CheckNamespace
public class UIButtonTween : MonoBehaviour
{
    public string callWhenFinished;
    public DisableCondition disableWhenFinished;
    public GameObject eventReceiver;
    public EnableCondition ifDisabledOnPlay;
    public bool includeChildren;
    private bool mHighlighted;
    private bool mStarted;
    private UITweener[] mTweens;
    public UITweener.OnFinished onFinished;
    public Direction playDirection = Direction.Forward;
    public bool resetOnPlay;
    public Trigger trigger;
    public int tweenGroup;
    public GameObject tweenTarget;

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

    public void Play(bool forward)
    {
        GameObject go = this.tweenTarget != null ? this.tweenTarget : gameObject;
        if (!NGUITools.GetActive(go))
        {
            if (this.ifDisabledOnPlay != EnableCondition.EnableThenPlay)
            {
                return;
            }
            NGUITools.SetActive(go, true);
        }
        this.mTweens = !this.includeChildren ? go.GetComponents<UITweener>() : go.GetComponentsInChildren<UITweener>();
        if (this.mTweens.Length == 0)
        {
            if (this.disableWhenFinished != DisableCondition.DoNotDisable)
            {
                NGUITools.SetActive(this.tweenTarget, false);
            }
        }
        else
        {
            bool flag = false;
            if (this.playDirection == Direction.Reverse)
            {
                forward = !forward;
            }
            int index = 0;
            int length = this.mTweens.Length;
            while (index < length)
            {
                UITweener tweener = this.mTweens[index];
                if (tweener.tweenGroup == this.tweenGroup)
                {
                    if (!flag && !NGUITools.GetActive(go))
                    {
                        flag = true;
                        NGUITools.SetActive(go, true);
                    }
                    if (this.playDirection == Direction.Toggle)
                    {
                        tweener.Toggle();
                    }
                    else
                    {
                        tweener.Play(forward);
                    }
                    if (this.resetOnPlay)
                    {
                        tweener.Reset();
                    }
                    tweener.onFinished = this.onFinished;
                    if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
                    {
                        tweener.eventReceiver = this.eventReceiver;
                        tweener.callWhenFinished = this.callWhenFinished;
                    }
                }
                index++;
            }
        }
    }

    private void Start()
    {
        this.mStarted = true;
        if (this.tweenTarget == null)
        {
            this.tweenTarget = gameObject;
        }
    }

    private void Update()
    {
        if (this.disableWhenFinished != DisableCondition.DoNotDisable && this.mTweens != null)
        {
            bool flag = true;
            bool flag2 = true;
            int index = 0;
            int length = this.mTweens.Length;
            while (index < length)
            {
                UITweener tweener = this.mTweens[index];
                if (tweener.tweenGroup == this.tweenGroup)
                {
                    if (tweener.enabled)
                    {
                        flag = false;
                        break;
                    }
                    if (tweener.direction != (Direction) (int) this.disableWhenFinished)
                    {
                        flag2 = false;
                    }
                }
                index++;
            }
            if (flag)
            {
                if (flag2)
                {
                    NGUITools.SetActive(this.tweenTarget, false);
                }
                this.mTweens = null;
            }
        }
    }
}

