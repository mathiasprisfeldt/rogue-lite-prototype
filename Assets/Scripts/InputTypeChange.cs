using System;
using InControl;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace InputTypeChange
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class InputTypeChange : MonoBehaviour 
    {
        [Serializable]
        public class TypeChange : UnityEvent<BindingSourceType>{}

        public TypeChange OnKeyboardChange;

        public TypeChange OnControllerChange;

        public void Start()
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.InputTypeChange.AddListener(OnInputChange);
                if(GameManager.Instance.Player)
                    OnInputChange(GameManager.Instance.Player.C.PlayerActions.LastInputType);
            }
                

        }

        private void OnInputChange(BindingSourceType arg0)
        {
            switch (arg0)
            {
                case BindingSourceType.DeviceBindingSource:
                    OnControllerChange.Invoke(arg0);
                    break;
                case BindingSourceType.KeyBindingSource:
                case BindingSourceType.MouseBindingSource:
                    OnKeyboardChange.Invoke(arg0);
                    break;               
            }
        }
    }
}