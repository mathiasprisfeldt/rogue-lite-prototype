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

        private bool _hasJumped;

        public override bool VerticalActive
        {
            get
            {
                return !_hasJumped && _playerMovement.App.C.PlayerActions.Jump.WasPressed &&
                       _playerMovement.State == CharacterState.InAir;
            }
        }

        public override void Awake()
        {
            base.Awake();
            _playerMovement.DoubleJump = this;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _jumpForce);
            _hasJumped = true;
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }

        public void Update()
        {
            if (_hasJumped && _playerMovement.State != CharacterState.InAir)
                _hasJumped = false;
        }

        public void OnDisable()
        {
            _playerMovement.DoubleJump = null;
        }

    }
}
