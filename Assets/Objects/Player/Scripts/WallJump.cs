using UnityEngine;

namespace CharacterController
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class WallJump : MovementAbility
    {
        [SerializeField]
        private float _verticalForce;

        [SerializeField]
        private float _horizontalForce;

        [SerializeField]
        private float _horizontalDuration;

        [SerializeField]
        private float _verticalDuration;

        [SerializeField, Range(0, 1)]
        private float _whenToSwicthDirection;

        private float _horizontalTimer;
        private float _verticalTimer;
        private bool _directionSwitched;

        public float Direction { get; private set; }

        public override bool VerticalActive
        {
            get
            {
                if (!base.VerticalActive)
                    return false;

                var input = _actionsController.App.C.PlayerActions != null && _actionsController.App.C.PlayerActions.ProxyInputActions.Jump.WasPressed;
                var collision = (_actionsController.WallSlideCheck.Sides.Left || _actionsController.WallSlideCheck.Sides.Right) && !_actionsController.GroundCollisionCheck.Bottom;
                if (input && collision && _verticalTimer <= 0 && _horizontalTimer <= 0 && _actionsController.LastUsedCombatAbility == CombatAbility.None)
                {
                    _horizontalTimer = _horizontalDuration;
                    _verticalTimer = _verticalDuration;
                    Direction = _actionsController.WallSlideCheck.Sides.Left ? 1 : -1;
                    _directionSwitched = false;
                    _actionsController.StartJump = true;
                }

                return _verticalTimer > 0;
            }
        }

        public override bool HorizontalActive
        {
            get
            {
                if (!base.HorizontalActive)
                    return false;
                return _horizontalTimer > 0 && _actionsController.LastUsedCombatAbility == CombatAbility.None;
            }
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _actionsController.Rigidbody.CalculateVerticalSpeed(_verticalForce / _verticalDuration));
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            if (_horizontalTimer / _horizontalDuration <= _whenToSwicthDirection && !_directionSwitched)
            {
                if (Direction > 0 && _actionsController.App.C.PlayerActions.Right
                    || Direction < 0 && _actionsController.App.C.PlayerActions.Left)
                {
                    Direction = -Direction;
                    _directionSwitched = true;
                }
                else
                    _horizontalTimer = 0;

            }
            if (_directionSwitched && Direction > 0 && _actionsController.App.C.PlayerActions.Left ||
                _directionSwitched && Direction < 0 && _actionsController.App.C.PlayerActions.Right)
            {
                _horizontalTimer = 0;
                return;
            }
            velocity = new Vector2((_horizontalForce / _horizontalDuration) * Direction, velocity.y);
        }

        public void FixedUpdate()
        {
            //If horizontal is active and a collision is detected in the current direction, then cancel horizontal
            if (_horizontalTimer > 0 && (_actionsController.WallSlideCheck.Sides.Left && Direction < 0 || _actionsController.WallSlideCheck.Sides.Right && Direction > 0))
                _horizontalTimer = 0;

            if (_horizontalTimer > 0)
                _horizontalTimer -= Time.fixedDeltaTime;

            //If vertical is active and a collision is detected in the current direction, then cancel vertical
            if (_verticalTimer > 0 && (_actionsController.WallSlideCheck.Sides.Left && Direction < 0 || _actionsController.WallSlideCheck.Sides.Right && Direction > 0))
                _verticalTimer = 0;
            if (_verticalTimer > 0)
                _verticalTimer -= Time.fixedDeltaTime;
        }
    }
}