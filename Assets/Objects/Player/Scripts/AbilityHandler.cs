using System;
using UnityEngine;
using UnityEngine.Events;

namespace Abilitys
{
    public enum HandledAbility
    {
        None, DoubleJump, WallSlide, WallJump, Dash, LedgeHanging, Throw
    }

    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class AbilityHandler : MonoBehaviour
    {
        [SerializeField]
        private AbilityReferences _abilityReferences;

        [SerializeField]
        private bool _doubleJump;

        [SerializeField]
        private bool _wallSlide;

        [SerializeField]
        private bool _wallJump;

        [SerializeField]
        private bool _dash;

        [SerializeField]
        private bool _ledgeHanging;

        [SerializeField]
        private bool _throw;

        public UnityEvent OnAbilityChange = new UnityEvent();

        public void Awake()
        {
            UpdateAbilities();  
        }

        public void Update()
        {
            
        }

        private void UpdateAbilities()
        {
            _abilityReferences.DoubleJump.Active = _doubleJump;
            _abilityReferences.WallSlide.Active = _wallSlide;
            _abilityReferences.WallJump.Active = _wallJump;
            _abilityReferences.Dash.Active = _dash;
            _abilityReferences.LedgeHanging.Active = _ledgeHanging;
            _abilityReferences.Throw.Active = _throw;
        }

        public void UnlockAbility(HandledAbility ab, bool unlock = true)
        {
            switch (ab)
            {
                case HandledAbility.DoubleJump:
                    _doubleJump = unlock;
                    break;
                case HandledAbility.WallSlide:
                    _wallSlide = unlock;
                    break;
                case HandledAbility.WallJump:
                    _wallJump = unlock;
                    break;
                case HandledAbility.Dash:
                    _dash = unlock;
                    break;
                case HandledAbility.LedgeHanging:
                    _ledgeHanging = unlock;
                    break;
                case HandledAbility.Throw:
                    _throw = unlock;
                    break;
            }
            UpdateAbilities();
            OnAbilityChange.Invoke();
        }

        public Ability GetAbility(HandledAbility ab)
        {
            switch (ab)
            {
                case HandledAbility.DoubleJump:
                    return _abilityReferences.DoubleJump;
                case HandledAbility.WallSlide:
                    return _abilityReferences.WallSlide;
                case HandledAbility.WallJump:
                    return _abilityReferences.WallJump;
                case HandledAbility.Dash:
                    return _abilityReferences.Dash;
                case HandledAbility.LedgeHanging:
                    return _abilityReferences.LedgeHanging;
                case HandledAbility.Throw:
                    return _abilityReferences.Throw;
            }
            return null;
        }
    }
}