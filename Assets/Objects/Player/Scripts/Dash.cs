﻿using UnityEngine;

namespace CharacterController
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(Action))]
    public class Dash : global::Ability
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


        public override bool HorizontalActive
        {
            get
            {
                var input = _action.App.C.PlayerActions != null && _action.App.C.PlayerActions.Dash.WasPressed && _cooldownTimer <= 0 && !_dashing;
                if ((input || _dashing) && _cooldownTimer <= 0)
                {
                    if (input)
                    {
                        _direction = _action.Model.transform.localScale.x > 0 ? 1 : -1;
                        var leftInput = _action.App.C.PlayerActions.Left;
                        var rightInput = _action.App.C.PlayerActions.Right;

                        if (!leftInput && _action.TriggerCheck.Left && _direction == -1 && !_action.App.C.PlayerActions.Left
                            || !rightInput && _action.TriggerCheck.Left && _direction == -1 && !_action.App.C.PlayerActions.Left)
                            _direction = leftInput ? -1 : 1;

                        if (_direction > 0 && _action.TriggerCheck.Right || _direction < 0 && _action.TriggerCheck.Left)
                            return false;

                        _dashing = true;
                        _oldVelocity = _action.Rigidbody.velocity;
                        _dashingTimer = _dashDuration;
                    }
                    return true;
                }
                return false;
            }
        }

        public override bool VerticalActive
        {
            get { return HorizontalActive; }
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
            if (_dashing && _direction > 0 && _action.WallSlideCheck.Right
                || _dashing && _direction < 0 && _action.WallSlideCheck.Left)
            {
                _dashingTimer = 0;
                _dashing = false;

            }

            _action.Flip(_direction);
            if (_dashingTimer <= 0)
            {
                _dashing = false;
                velocity = _oldVelocity;
                _cooldownTimer = _dashCooldown;
            }
            else
                velocity = new Vector2(_dashCurve.Evaluate((_dashDuration - Mathf.Abs(_dashingTimer)) / _dashDuration) * _dashForce * _direction,
                    velocity.y);
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _action.Rigidbody.CounterGravity(0));
        }
    }
}