using Archon.SwissArmyLib.Utils;
using Controllers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterController
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
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

        [SerializeField]
        private float _damage;

        public bool InitialDash { get; set; }

        private bool _dashing;
        private float _dashingTimer;
        private float _cooldownTimer;
        private Vector2 _oldVelocity;
        private float _direction;
        private bool _canDash;
        List<Character> _dirtyColls = new List<Character>();

        public float LeftCooldown { get; set; }
        public float RightCooldown { get; set; }

        public override bool HorizontalActive
        {
            get
            {
                var input = _actionsController.App.C.PlayerActions != null && _actionsController.App.C.PlayerActions.ProxyInputActions.Dash.WasPressed
                    && _cooldownTimer <= 0 && !_dashing;

                InitialDash = false;

                if (!base.HorizontalActive)
                    return false;

                if ((input && _canDash || _dashing) && _cooldownTimer <= 0 && !_actionsController.Combat)
                {
                    if (input)
                    {
                        _direction = _actionsController.Model.transform.localScale.x > 0 ? 1 : -1;

                        var isNotvalid = _direction > 0 && RightCooldown > 0 || _direction < 0 && LeftCooldown > 0;

                        if (_direction > 0 && _actionsController.WallSlideCheck.Right ||
                            _direction < 0 && _actionsController.WallSlideCheck.Left)
                            isNotvalid = true;

                        if (isNotvalid)
                        {
                            _actionsController.HealthController.IsInvurnable = false;
                            return false;
                        }

                        InitialDash = true;
                        _actionsController.StartDash.Value = true;
                        _dashing = true;
                        _oldVelocity = _actionsController.Rigidbody.velocity;
                        _dashingTimer = _dashDuration;
                        _canDash = false;

                    }
                    _actionsController.HealthController.IsInvurnable = true;
                    return true;
                }
                _actionsController.HealthController.IsInvurnable = false;
                return false;
            }
        }

        public override bool VerticalActive
        {
            get { return HorizontalActive; }
        }

        public void FixedUpdate()
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= BetterTime.DeltaTime;
            if (_dashingTimer > 0)
                _dashingTimer -= BetterTime.DeltaTime;
            if (_actionsController.LastUsedVerticalMoveAbility != MoveAbility.Dash)
            {
                _dashingTimer = 0;
            }
            if (!_canDash && !_dashing)
            {
                if (_actionsController.OnGround || _actionsController.WallSlideCheck.Sides.Left ||
                    _actionsController.WallSlideCheck.Sides.Right)
                    _canDash = true;
            }
            if (LeftCooldown > 0)
                LeftCooldown -= BetterTime.DeltaTime;
            if (RightCooldown > 0)
                RightCooldown -= BetterTime.DeltaTime;
        }

        private void Update()
        {
            if (_dashing)
            {
                var colls = _actionsController.Hitbox.Sides.TargetColliders.Where(x => x.tag.Equals("Enemy"));

                if (colls.Any())
                {
                    foreach (var item in colls)
                    {
                        Character character = item.GetComponent<CollisionCheck>().Character;
                        if (!_dirtyColls.Contains(character))
                        {
                            _dirtyColls.Add(character);

                            character.HealthController.Damage(_damage, from: _actionsController, pos: transform.position);
                        }
                    }
                }
            }
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            if (_dashing && _direction > 0 && _actionsController.WallSlideCheck.Right
                || _dashing && _direction < 0 && _actionsController.WallSlideCheck.Left
                || _dashingTimer <= 0)
            {
                _dashingTimer = 0;
                velocity = _oldVelocity;
                _dashing = false;
                _cooldownTimer = _dashCooldown;
                _dirtyColls = new List<Character>();
            }
            else
                velocity = new Vector2(_dashCurve.Evaluate((_dashDuration - Mathf.Abs(_dashingTimer)) / _dashDuration) * _dashForce * _direction,
                    velocity.y);
            _actionsController.Flip(_direction);
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _actionsController.Rigidbody.CounterGravity(0));
        }
    }
}