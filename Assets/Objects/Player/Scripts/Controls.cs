using System;
using System.Collections.Generic;
using Abilitys;
using UnityEngine;

namespace Controls
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class Controls : MonoBehaviour 
    {

        [SerializeField]
        private GameObject _special;

        [SerializeField]
        private GameObject _doubleJump;

        private AbilityHandler _abilityHandler;

        public void Start()
        {
            _abilityHandler = FindObjectOfType<AbilityHandler>();

            if (_abilityHandler != null)
            {
                _abilityHandler.OnAbilityChange.AddListener(OnAbilityChange);
                OnAbilityChange();
            }
        }

        private void OnAbilityChange()
        {
            _special.SetActive(_abilityHandler.GetAbility(HandledAbility.Throw).Active);
            _doubleJump.SetActive(_abilityHandler.GetAbility(HandledAbility.DoubleJump).Active);
        }

        public void OnDestroy()
        {
            if (_abilityHandler != null)
                _abilityHandler.OnAbilityChange.RemoveListener(OnAbilityChange);
        }
    }
}