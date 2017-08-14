using System;
using UnityEngine;

/// <summary>
/// Purpose:
/// Creator:
/// </summary>

namespace CharacterController
{
    [RequireComponent(typeof (ActionController)), ExecuteInEditMode]
    public class DoubleJump : MovementAbility
    {
        [SerializeField]
        private float _jumpForce;

        [SerializeField]
        private float _jumpDuration;

        private bool _hasJumped;
        private float _jumpTimer;

        public override bool VerticalActive
        {
            get
            {
                if (_actionController.App.C.PlayerActions.Jump.WasPressed && _actionController.State == CharacterState.InAir
                    && _jumpTimer <= 0 && !_hasJumped)
                {
                    _jumpTimer = _jumpDuration;
                    _hasJumped = true;
                    _actionController.Animator.SetTrigger("DoubleJump");
                }
                    
                return _jumpTimer > 0;
            }
        }

        public override void Awake()
        {
            base.Awake();
            _actionController.DoubleJump = this;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _actionController.Rigidbody.CalculateVerticalSpeed(_jumpForce / _jumpDuration));
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }

        public void FixedUpdate()
        {
            if(_jumpTimer > 0)
                _jumpTimer -= Time.fixedDeltaTime;
            if (_actionController.OnGround )
                _hasJumped = false;
        }

        public void OnDisable()
        {
            _actionController.DoubleJump = null;
        }

    }
}
