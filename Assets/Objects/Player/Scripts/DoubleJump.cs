using System;
using Controllers;
using UnityEngine;

/// <summary>
/// Purpose:
/// Creator:
/// </summary>

namespace CharacterController
{
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
                if (!base.VerticalActive)
                    return false;

                if (_actionsController.App.C.PlayerActions.ProxyInputActions.Jump.WasPressed && _actionsController.State == CharacterState.InAir
                    && _jumpTimer <= 0 && !_hasJumped && _actionsController.LastUsedVerticalMoveAbility == MoveAbility.None)
                {
                    _jumpTimer = _jumpDuration;
                    _hasJumped = true;
                }
                    
                return _jumpTimer > 0;
            }
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _actionsController.Rigidbody.CalculateVerticalSpeed(_jumpForce / _jumpDuration));
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }

        public void FixedUpdate()
        {
            if(_jumpTimer > 0)
                _jumpTimer -= Time.fixedDeltaTime;
            if (_hasJumped)
            {
                if (_actionsController.OnGround || _actionsController.AbilityReferences.WallSlide.VerticalActive 
                    || _actionsController.AbilityReferences.LedgeHanging.VerticalActive)
                    _hasJumped = false;
            }
            if (!_hasJumped && _actionsController.AbilityReferences.WallJump.VerticalActive)
                _hasJumped = true;
        }

    }
}
