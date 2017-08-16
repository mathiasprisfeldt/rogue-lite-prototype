using System.Collections.Generic;
using BindingsExample;
using CharacterController;
using Health;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class Melee : MonoBehaviour
    {
        [SerializeField]
        private CollisionCheck _collisionCheck;

        [SerializeField]
        private float _damage;

        [SerializeField,Range(0f,float.MaxValue)]
        private float _hitCooldown;

        [SerializeField]
        private ActionsController _actionsController;

        private List<GameObject> _objectsTouched = new List<GameObject>();

        private float _cooldownTimer;
        private bool _active;

        public bool Active
        {
            get
            {
                if (_actionsController.App.C.PlayerActions.ProxyInputActions.Attack.WasPressed && !_active && _cooldownTimer <= 0 && 
                    _actionsController.LastUsedVerticalAbility != CharacterController.Ability.LedgeHanging)
                    _active = true;
                return _active;
            }
        }

        public void Update()
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= Time.deltaTime;
            if(_active)
            {
                foreach (var c in _collisionCheck.Sides.TargetColliders)
                {
                    if (!_objectsTouched.Contains(c.gameObject))
                    {
                        _objectsTouched.Add(c.gameObject);
                        CollisionCheck cc = c.gameObject.GetComponent<CollisionCheck>();
                        if (cc != null)
                            if (cc.Character.HealthController != null)
                            {
                                cc.Character.HealthController.Damage(_damage);
                            }
                                
                    }
                }
            }
        }

        public void ResetMelee()
        {
            _cooldownTimer = _hitCooldown;
            _objectsTouched.Clear();
            _active = false;
            _actionsController.Combat = false;
        }
    }
}