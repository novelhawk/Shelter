using NGUI;
using NGUI.Internal;
using UnityEngine;

[RequireComponent(typeof(Animation)), AddComponentMenu("NGUI/Internal/Active Animation")]
// ReSharper disable once CheckNamespace
public class ActiveAnimation : IgnoreTimeScale
{
    public string callWhenFinished;
    public GameObject eventReceiver;
    private Animation mAnim;
    private Direction mDisableDirection;
    private Direction mLastDirection;
    private bool mNotify;
    public OnFinished onFinished;

    private void Play(string clipName, Direction playDirection)
    {
        if (this.mAnim != null)
        {
            enabled = true;
            this.mAnim.enabled = false;
            if (playDirection == Direction.Toggle)
            {
                playDirection = this.mLastDirection == Direction.Forward ? Direction.Reverse : Direction.Forward;
            }
            if (string.IsNullOrEmpty(clipName))
            {
                if (!this.mAnim.isPlaying)
                {
                    this.mAnim.Play();
                }
            }
            else if (!this.mAnim.IsPlaying(clipName))
            {
                this.mAnim.Play(clipName);
            }

            foreach (AnimationState current in mAnim)
            {
                if (string.IsNullOrEmpty(clipName) || current.name == clipName)
                {
                    current.speed = Mathf.Abs(current.speed) * (float) playDirection;
                    switch (playDirection)
                    {
                        case Direction.Reverse when current.time == 0f:
                            current.time = current.length;
                            break;
                        case Direction.Forward when current.time == current.length:
                            current.time = 0f;
                            break;
                    }
                }
            }
            this.mLastDirection = playDirection;
            this.mNotify = true;
            this.mAnim.Sample();
        }
    }

    public static ActiveAnimation Play(Animation anim, Direction playDirection)
    {
        return Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
    }

    public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection)
    {
        return Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
    }

    public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
    {
        if (!NGUITools.GetActive(anim.gameObject))
        {
            if (enableBeforePlay != EnableCondition.EnableThenPlay)
            {
                return null;
            }
            NGUITools.SetActive(anim.gameObject, true);
            UIPanel[] componentsInChildren = anim.gameObject.GetComponentsInChildren<UIPanel>();
            int index = 0;
            int length = componentsInChildren.Length;
            while (index < length)
            {
                componentsInChildren[index].Refresh();
                index++;
            }
        }
        ActiveAnimation component = anim.GetComponent<ActiveAnimation>();
        if (component == null)
        {
            component = anim.gameObject.AddComponent<ActiveAnimation>();
        }
        component.mAnim = anim;
        component.mDisableDirection = (Direction) disableCondition;
        component.eventReceiver = null;
        component.callWhenFinished = null;
        component.onFinished = null;
        component.Play(clipName, playDirection);
        return component;
    }

    public void Reset()
    {
        if (this.mAnim != null)
        {
            foreach (AnimationState current in mAnim)
            {
                switch (this.mLastDirection)
                {
                    case Direction.Reverse:
                        current.time = current.length;
                        break;
                    case Direction.Forward:
                        current.time = 0f;
                        break;
                }
            }
        }
    }

    private void Update()
    {
        float num = UpdateRealTimeDelta();
        if (num != 0f)
        {
            if (this.mAnim != null)
            {
                bool flag = false;
                foreach (AnimationState state in mAnim)
                {
                    if (this.mAnim.IsPlaying(state.name))
                    {
                        float num2 = state.speed * num;
                        state.time += num2;
                        if (num2 < 0f)
                        {
                            if (state.time > 0f)
                                flag = true;
                            else
                                state.time = 0f;
                        }
                        else if (state.time < state.length)
                        {
                            flag = true;
                        }
                        else
                        {
                            state.time = state.length;
                        }
                    }
                }
                this.mAnim.Sample();
                if (!flag)
                {
                    enabled = false;
                    if (this.mNotify)
                    {
                        this.mNotify = false;
                        onFinished?.Invoke(this);
                        if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
                        {
                            this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
                        }
                        if (this.mDisableDirection != Direction.Toggle && this.mLastDirection == this.mDisableDirection)
                        {
                            NGUITools.SetActive(gameObject, false);
                        }
                    }
                }
            }
            else
            {
                enabled = false;
            }
        }
    }

    public bool isPlaying
    {
        get
        {
            if (this.mAnim != null)
            {
                foreach (AnimationState state in mAnim)
                {
                    if (this.mAnim.IsPlaying(state.name))
                    {
                        if (this.mLastDirection == Direction.Forward)
                        {
                            if (state.time < state.length)
                                return true;
                        }
                        else
                        {
                            if (this.mLastDirection != Direction.Reverse)
                                return true;
                            if (state.time > 0f)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    public delegate void OnFinished(ActiveAnimation anim);
}

