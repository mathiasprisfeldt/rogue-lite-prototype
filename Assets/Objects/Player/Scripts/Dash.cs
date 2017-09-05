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

        public bool InitialDash { get; set; }

        private bool _dashing;
        private float _dashingTimer;
        private float _cooldownTimer;
        private Vector2 _oldVelocity;
        private float _direction;
        private bool _canDash;

        public float CoolDownTimer { get; set; }

        public override bool HorizontalActive
        {
            get
            {
                var input = _actionsController.App.C.PlayerActions != null && _actionsController.App.C.PlayerActions.ProxyInputActions.Dash.WasPressed && _cooldownTimer <= 0 && !_dashing;

                InitialDash = false;

                if (!base.HorizontalActive || CoolDownTimer > 0)
                    return false;

                if ((input && _canDash|| _dashing) && _cooldownTimer <= 0 && !_actionsController.Combat)
                {
                    if (input)
                    {
                        _actionsController.StartDash.Value = true;
                        _direction = _actionsController.Model.transform.localScale.x > 0 ? 1 : -1;
                        InitialDash = true;

                        if (_direction > 0 && _actionsController.WallSlideCheck.Right || _direction < 0 && _actionsController.WallSlideCheck.Left)
                            return false;

                        _dashing = true;
                        _oldVelocity = _actionsController.Rigidbody.velocity;
                        _dashingTimer = _dashDuration;
                        _canDash = false;
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

        public void FixedUpdate()
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= Time.deltaTime;
            if (_dashingTimer > 0)
                _dashingTimer -= Time.deltaTime;
            if (_actionsController.LastUsedVerticalMoveAbility != MoveAbility.Dash)
            {
                _cooldownTimer = 0;
                _dashingTimer = 0;
            }
            if (!_canDash && !_dashing)
            {
                if (_actionsController.OnGround || _actionsController.WallSlideCheck.Sides.Left ||
                    _actionsController.WallSlideCheck.Sides.Right)
                    _canDash = true;
            }
            if (CoolDownTimer > 0)
                CoolDownTimer -= Time.deltaTime;
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            if (_dashing && _direction > 0 && _actionsController.WallSlideCheck.Right
                || _dashing && _direction < 0 && _actionsController.WallSlideCheck.Left)
            {
                _dashingTimer = 0;
                _dashing = false;
            }

            _actionsController.Flip(_direction);
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
            velocity = new Vector2(velocity.x, _actionsController.Rigidbody.CounterGravity(0));
        }
    }
}