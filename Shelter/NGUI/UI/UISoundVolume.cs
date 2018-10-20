using JetBrains.Annotations;
using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Sound Volume"), RequireComponent(typeof(UISlider))]
// ReSharper disable once CheckNamespace
public class UISoundVolume : MonoBehaviour
{
    private UISlider mSlider;

    private void Awake()
    {
        this.mSlider = GetComponent<UISlider>();
        this.mSlider.sliderValue = NGUITools.soundVolume;
        this.mSlider.eventReceiver = gameObject;
    }

    [UsedImplicitly]
    private void OnSliderChange(float val)
    {
        NGUITools.soundVolume = val;
    }
}

