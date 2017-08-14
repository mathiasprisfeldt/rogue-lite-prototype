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
    public class Melee : Ability
    {
        [SerializeField]
        private CollisionCheck _collisionCheck;

        [SerializeField]
        private float _damage;

        [SerializeField,Range(0.01f,float.MaxValue)]
        private float _hitCooldown;

        private List<GameObject> _objectsTouched = new List<GameObject>();

        private float _cooldownTimer;
        private bool _active;

        public override bool VerticalActive
        {
            get
            {
                if (_actionController.App.C.PlayerActions.Attack.WasPressed && !_active && _cooldownTimer <= 0)
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
                        if (c.gameObject.name == "New Sprite")
                        {
                            
                        }
                        HealthController hc = c.gameObject.GetComponent<HealthController>();
                        if (hc != null)
                            hc.Damage(_damage);
                    }
                }
            }
        }

        public void ResetMelee()
        {
            _cooldownTimer = _hitCooldown;
            _objectsTouched.Clear();
            _active = false;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x,0);
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }
    }
}