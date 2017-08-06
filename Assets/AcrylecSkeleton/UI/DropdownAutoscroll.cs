using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AcrylecSkeleton.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class DropdownAutoscroll : MonoBehaviour
    {
        [SerializeField, Tooltip("Amount in percent to overshoot relavtive to item height.")] private float _scrollMargin;

        private ScrollRect _scrollRectComponent;
        private Dropdown _dropdown;

        private void Start()
        {
            _scrollRectComponent = GetComponent<ScrollRect>();
            _dropdown = transform.parent.gameObject.GetComponent<Dropdown>();
            OnUpdateSelected();
        }

        /// <summary>
        /// Check after navigation input and update scroll accordingly.
        /// </summary>
        private void Update()
        {
            if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0 || InputManager.ActiveDevice.DPadUp || InputManager.ActiveDevice.DPadDown ||
                InputManager.ActiveDevice.LeftStickUp || InputManager.ActiveDevice.LeftStickDown)
                OnUpdateSelected();
        }
    
        /// <summary>
        /// Takes current selection in dropdown and checks for bounds in scroll rect, then auto scrolls if it exceeds bounds. Can also add overshoot margin, check scroll margin.
        /// </summary>
        private void OnUpdateSelected()
        {
            if (_dropdown && _dropdown.options.Count <= 5)
                return;

            //Grab the currently selected item in the dropdown
            var selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

            if (!selectedElement)
                return;

            // helper vars
            float contentHeight = _scrollRectComponent.content.rect.height;
            float viewportHeight = _scrollRectComponent.viewport.rect.height;

            // what bounds must be visible?
            float centerLine = selectedElement.transform.localPosition.y; // selected item's center
            float upperBound = centerLine + (selectedElement.GetComponent<RectTransform>().rect.height / 2f); // selected item's upper bound
            float lowerBound = centerLine - (selectedElement.GetComponent<RectTransform>().rect.height / 2f); // selected item's lower bound

            // what are the bounds of the currently visible area?
            float lowerVisible = (contentHeight - viewportHeight) * _scrollRectComponent.normalizedPosition.y - contentHeight;
            float upperVisible = lowerVisible + viewportHeight;

            // is our item visible right now?
            float desiredLowerBound;
            if (upperBound > upperVisible)
            {
                // need to scroll up to upperBound
                desiredLowerBound = upperBound - viewportHeight + selectedElement.GetComponent<RectTransform>().rect.height * _scrollMargin;
            }
            else if (lowerBound < lowerVisible)
            {
                // need to scroll down to lowerBound
                desiredLowerBound = lowerBound - selectedElement.GetComponent<RectTransform>().rect.height * _scrollMargin;
            }
            else
            {
                // item already visible - all good
                return;
            }

            // normalize and set the desired viewport
            float normalizedDesired = (desiredLowerBound + contentHeight) / (contentHeight - viewportHeight);
            _scrollRectComponent.normalizedPosition = new Vector2(0f, Mathf.Clamp01(normalizedDesired));
        }
    }
}