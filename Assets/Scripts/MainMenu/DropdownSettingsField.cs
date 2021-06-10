using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownSettingsField : MonoBehaviour
{
    public delegate void Changed();
    public event Changed OnChanged;

    private int value = 0;
    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponentInChildren<TMP_Dropdown>();
    }

    public void SetValue()
    {
        value = dropdown.value;

        if (OnChanged != null)
            OnChanged();
    }

    public void SetValue (int newValue)
    {
        value = newValue;
        UpdateUI();

        if (OnChanged != null)
            OnChanged();
    }

    public int GetValue()
    {
        return value;
    }

    private void UpdateUI()
    {
        dropdown.value = value;
    }
}
