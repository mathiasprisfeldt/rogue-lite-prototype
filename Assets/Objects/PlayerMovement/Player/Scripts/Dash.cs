using System;
using UnityEngine;

namespace CharacterController
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    public class Dash : MovementAbility
    {
        [SerializeField]
        private float _dashCooldown;

        [SerializeField]
        private float _dashDuration;

        [SerializeField]
        private float _dashForce;

        [SerializeField]
        private AnimationCurve _dashCurve;

        private bool _dashing;
        private float _dashingTimer;
        private float _cooldownTimer;
        private Vector2 _oldVelocity;
        private float _direction;

        [ExecuteInEditMode]
        public override void Awake()
        {
            base.Awake();
            _playerMovement.Dash = this;
        }

        public override bool HorizontalActive
        {
            get
            {
                var input = _playerMovement.App.C.PlayerActions.Dash.WasPressed && _cooldownTimer <= 0 && !_dashing;
                if ((input || _dashing) && _cooldownTimer <= 0)
                {
                    if (input)
                    {
                        _direction = _playerMovement.Model.transform.localScale.x > 0 ? 1 : -1;
                        var leftInput = _playerMovement.App.C.PlayerActions.Left.IsPressed;
                        var rightInput = _playerMovement.App.C.PlayerActions.Right.IsPressed;

                        if (leftInput || rightInput)
                            _direction = leftInput ? -1 : 1;

                        if (_playerMovement.TriggerCheck.Left && _direction == -1)
                            return false;

                        if (_playerMovement.TriggerCheck.Right && _direction == 1)
                            return false;

                        _dashing = true;
                        _oldVelocity = _playerMovement.Rigidbody.velocity;
                        _dashingTimer = _dashDuration;
                    }
                    return true;
                }
                return false;
            }
        }

        public void Update()
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= Time.deltaTime;
            if (_dashingTimer > 0)
                _dashingTimer -= Time.deltaTime;
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            if (_dashingTimer <= 0)
            {
                _dashing = false;
                velocity = _oldVelocity;
                _cooldownTimer = _dashCooldown;
            }
            else
                velocity = new Vector2(_dashCurve.Evaluate((_dashDuration - Mathf.Abs(_dashingTimer)) / _dashDuration) * _dashForce * _direction,
                    _playerMovement.Rigidbody.gravityScale * Physics2D.gravity.y);
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            throw new NotImplementedException();
        }
    }
}