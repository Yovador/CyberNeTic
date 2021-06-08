using UnityEngine;
using UnityEngine.UI;

public abstract class SettingsField : MonoBehaviour
{
    private Slider slider;

    public delegate void Changed();
    public event Changed OnChanged;

    protected void Init()
    {
        slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;

        UpdateUI();
    }

    public virtual void OnValueChanged()
    {
        UpdateUI();

        if (OnChanged != null)
            OnChanged();
    }

    protected virtual void UpdateUI ()
    {

    }

    protected void SetSliderValue (float sliderValue)
    {
        slider.value = sliderValue;
    }

    protected float GetSliderValue()
    {
        return slider.value;
    }
}
