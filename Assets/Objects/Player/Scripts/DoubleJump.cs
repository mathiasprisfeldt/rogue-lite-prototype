using System;
using Archon.SwissArmyLib.Utils;
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

        public float Cooldown { get; set; }

        public override bool VerticalActive
        {
            get
            {
                if (!base.VerticalActive)
                    return false;

                if (_actionsController.App.C.PlayerActions.ProxyInputActions.Jump.WasPressed && _actionsController.State == CharacterState.InAir
                    && _jumpTimer <= 0 && !HasJumped && _actionsController.LastUsedVerticalMoveAbility == MoveAbility.None && Cooldown <= 0 && 
                  !_actionsController.AbilityReferences.WallSlide.VerticalActive)
                {
                    _jumpTimer = _jumpDuration;
                    HasJumped = true;
                }
                return _jumpTimer > 0;
            }
        }

        public bool HasJumped
        {
            get { return _hasJumped; }
            set { _hasJumped = value; }
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
            if (_jumpTimer > 0)
                _jumpTimer -= BetterTime.FixedDeltaTime;
            if ((_actionsController.OnGround || _actionsController.AbilityReferences.WallSlide.VerticalActive) && !_actionsController.AbilityReferences.WallJump.VerticalActive)
                HasJumped = false;
            if (Cooldown > 0)
                Cooldown -= Time.deltaTime;
        }

    }
}
