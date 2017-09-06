using System.Collections.Generic;
using CharacterController;
using UnityEngine;

namespace Meele
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class DownMeele : MonoBehaviour
    {
        [SerializeField]
        private float _knockbackForce;

        [SerializeField]
        private float _knockbackDuration;

        [SerializeField]
        private CollisionCheck _collisionCheck;

        [SerializeField, Range(0f, float.MaxValue)]
        private float _hitCooldown;

        [SerializeField]
        private ActionsController _actionsController;

        private List<GameObject> _objectsTouched = new List<GameObject>();

        private float _cooldownTimer;
        private bool _active;
        private bool _knockbackAdded;

        public bool Active
        {
            get
            {
                if ((_actionsController.App.C.PlayerActions.ProxyInputActions.Attack.WasPressed && !_actionsController.OnGround
                    && _actionsController.App.C.PlayerActions.Down) && !_active &&
                    _cooldownTimer <= 0 && _actionsController.LastUsedVerticalMoveAbility != MoveAbility.LedgeHanging
                    && _actionsController.LastUsedCombatAbility == CombatAbility.None)
                {
                    _active = true;
                    _actionsController.StartDownMeele.Value = true;
                    _knockbackAdded = false;
                }

                return _active;
            }
        }

        public void Update()
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= Time.deltaTime;
            if (_active)
            {
                foreach (var c in _collisionCheck.Sides.TargetColliders)
                {
                    if (!_objectsTouched.Contains(c.gameObject))
                    {
                        _objectsTouched.Add(c.gameObject);

                        CollisionCheck cc = c.gameObject.GetComponent<CollisionCheck>();
                        if (cc && cc.Character.HealthController != null && !cc.Character.HealthController.IsDead)
                        {
                            cc.Character.HealthController.Damage(_actionsController.Damage, from: _actionsController);
                            _knockbackAdded = true;
                            _actionsController.App.C.Character.KnockbackHandler.AddForce(new Vector2(0,_knockbackForce), _knockbackDuration,true,false);
                            ResetDownMelee();
                            break;
                        }
                            
                    }
                }

                if (_active && _actionsController.OnGround)
                    ResetDownMelee();
                    
            }
        }

        public void ResetDownMelee()
        {
            _cooldownTimer = _hitCooldown;
            _objectsTouched.Clear();
            _active = false;
            _actionsController.Combat = false;
        }
    }
}