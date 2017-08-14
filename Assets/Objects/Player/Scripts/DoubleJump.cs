using System;
using Controllers;
using UnityEngine;

/// <summary>
/// Purpose:
/// Creator:
/// </summary>

namespace CharacterController
{
    [RequireComponent(typeof (Action))]
    public class DoubleJump : global::Ability
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
                if (_action.App.C.PlayerActions.Jump.WasPressed && _action.State == CharacterState.InAir
                    && _jumpTimer <= 0 && !_hasJumped)
                {
                    _jumpTimer = _jumpDuration;
                    _hasJumped = true;
                    _action.Animator.SetTrigger("DoubleJump");
                }
                    
                return _jumpTimer > 0;
            }
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _action.Rigidbody.CalculateVerticalSpeed(_jumpForce / _jumpDuration));
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }

        public void FixedUpdate()
        {
            if(_jumpTimer > 0)
                _jumpTimer -= Time.fixedDeltaTime;
            if (_action.OnGround )
                _hasJumped = false;
        }

    }
}
