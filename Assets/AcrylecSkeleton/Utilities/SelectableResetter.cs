using UnityEngine;
using UnityEngine.UI;

namespace AcrylecSkeleton.Utilities
{
    public class SelectableResetter : MonoBehaviour
    {
        private Dropdown _dropdown;
        private Toggle _toggle;
        private Slider _slider;

        private object _defaultValue;

        // Use this for initialization
        void Awake ()
        {
            _dropdown = GetComponent<Dropdown>();
            if (_dropdown)
                _defaultValue = _dropdown.value;

            _toggle = GetComponent<Toggle>();
            if (_toggle)
                _defaultValue = _toggle.isOn;

            _slider = GetComponent<Slider>();
            if (_slider)
                _defaultValue = _slider.value;
        }
    
        public void SetToDefault()
        {
            if (_dropdown)
            {
                _dropdown.value = (int) _defaultValue;
                _dropdown.onValueChanged.Invoke(_dropdown.value);
            }

            if (_toggle)
            {
                _toggle.isOn = (bool) _defaultValue;
                _toggle.onValueChanged.Invoke(_toggle.isOn);
            }

            if (_slider)
            {
                _slider.value = (float) _defaultValue;
                _slider.onValueChanged.Invoke(_slider.value);
            }
        }
    }
}