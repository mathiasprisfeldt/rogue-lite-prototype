using UnityEngine;

namespace CharacterController
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(ActionController))]
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
            _actionController.Dash = this;
        }

        public override bool HorizontalActive
        {
            get
            {
                var input = _actionController.App.C.PlayerActions != null && _actionController.App.C.PlayerActions.Dash.WasPressed && _cooldownTimer <= 0 && !_dashing;
                if ((input || _dashing) && _cooldownTimer <= 0)
                {
                    if (input)
                    {
                        _direction = _actionController.Model.transform.localScale.x > 0 ? 1 : -1;
                        var leftInput = _actionController.App.C.PlayerActions.Left;
                        var rightInput = _actionController.App.C.PlayerActions.Right;

                        if (!leftInput && _actionController.TriggerCheck.Left && _direction == -1 && !_actionController.App.C.PlayerActions.Left
                            || !rightInput && _actionController.TriggerCheck.Left && _direction == -1 && !_actionController.App.C.PlayerActions.Left)
                            _direction = leftInput ? -1 : 1;

                        if (_direction > 0 && _actionController.TriggerCheck.Right || _direction < 0 && _actionController.TriggerCheck.Left)
                            return false;

                        _dashing = true;
                        _oldVelocity = _actionController.Rigidbody.velocity;
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
            if (_dashing && _direction > 0 && _actionController.WallSlideCheck.Right
                || _dashing && _direction < 0 && _actionController.WallSlideCheck.Left)
            {
                _dashingTimer = 0;
                _dashing = false;

            }

            _actionController.Flip(_direction);
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
            velocity = new Vector2(velocity.x, _actionController.Rigidbody.CounterGravity(0));
        }
    }
}