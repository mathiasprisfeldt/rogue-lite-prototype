using System;
using UnityEngine;

/// <summary>
/// Purpose:
/// Creator:
/// </summary>

namespace CharacterController
{
    [RequireComponent(typeof (PlayerMovement)), ExecuteInEditMode]
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
                if (_playerMovement.App.C.PlayerActions.Jump.WasPressed && _playerMovement.State == CharacterState.InAir 
                    && _jumpTimer <= 0)
                    _jumpTimer = _jumpDuration;
                return _jumpTimer > 0;
            }
        }

        public override void Awake()
        {
            base.Awake();
            _playerMovement.DoubleJump = this;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _jumpForce / _jumpDuration + Physics2D.gravity.y * _playerMovement.Rigidbody.gravityScale);
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }

        public void FixedUpdate()
        {
            if(_jumpTimer > 0)
                _jumpTimer -= Time.fixedDeltaTime;
        }

        public void OnDisable()
        {
            _playerMovement.DoubleJump = null;
        }

    }
}
