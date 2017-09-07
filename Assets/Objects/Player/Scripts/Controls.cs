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
        private GameObject _dash;

        [SerializeField]
        private GameObject _special;

        private AbilityHandler _abilityHandler = new AbilityHandler();


        public void Start()
        {
            _abilityHandler = GameObject.FindObjectOfType<AbilityHandler>();
            if (_abilityHandler != null)
            {
                _abilityHandler.OnAbilityChange.AddListener(OnAbilityChange);
                OnAbilityChange();
            }
        }

        private void OnAbilityChange()
        {
            _dash.SetActive(_abilityHandler.GetAbility(HandledAbility.Dash).Active);
            _special.SetActive(_abilityHandler.GetAbility(HandledAbility.Throw).Active);
        }

        public void OnDestroy()
        {
            if (_abilityHandler != null)
                _abilityHandler.OnAbilityChange.RemoveListener(OnAbilityChange);
        }
    }
}