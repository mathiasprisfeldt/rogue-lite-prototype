using System;
using UnityEngine;

/// <summary>
/// Purpose:
/// Creator:
/// </summary>

namespace CharacterController
{
    [RequireComponent(typeof (PlayerActions)), ExecuteInEditMode]
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
                if (_playerActions.App.C.PlayerActions.Jump.WasPressed && _playerActions.State == CharacterState.InAir
                    && _jumpTimer <= 0 && !_hasJumped)
                {
                    _jumpTimer = _jumpDuration;
                    _hasJumped = true;
                    _playerActions.Animator.SetTrigger("DoubleJump");
                }
                    
                return _jumpTimer > 0;
            }
        }

        public override void Awake()
        {
            base.Awake();
            _playerActions.DoubleJump = this;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _playerActions.Rigidbody.CalculateVerticalSpeed(_jumpForce / _jumpDuration));
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }

        public void FixedUpdate()
        {
            if(_jumpTimer > 0)
                _jumpTimer -= Time.fixedDeltaTime;
            if (_playerActions.OnGround )
                _hasJumped = false;
        }

        public void OnDisable()
        {
            _playerActions.DoubleJump = null;
        }

    }
}
