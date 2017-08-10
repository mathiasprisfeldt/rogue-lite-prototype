using UnityEngine;

namespace CharacterController
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(PlayerMovement)), ExecuteInEditMode]
    public class WallJump : MovementAbility
    {
        private float _horizontalTimer;
        private float _verticalTimer;

        [SerializeField]
        private float _verticalForce;

        [SerializeField]
        private float _horizontalForce;

        [SerializeField]
        private float _horizontalDuration;

        [SerializeField]
        private float _verticalDuration;

        [SerializeField]
        private float _timeLeftToMove;

        public float Direction { get; private set; }

        public override bool VerticalActive
        {
            get
            {
                var input = _playerMovement.App.C.PlayerActions != null && _playerMovement.App.C.PlayerActions.Jump.WasPressed;
                var collision = (_playerMovement.TriggerSides.Left || _playerMovement.TriggerSides.Right) && !_playerMovement.GroundCollisionCheck.Bottom;
                if (input && collision && _verticalTimer <= 0 && _horizontalTimer <= 0)
                {
                    _horizontalTimer = _horizontalDuration;
                    _verticalTimer = _verticalDuration;
                    Direction = _playerMovement.TriggerSides.Left ? 1 : -1;
                }
                                    
                return _verticalTimer > 0;
            }
        }

        public override bool HorizontalActive { get { return _horizontalTimer > 0; } }

        public override void Awake()
        {
            base.Awake();
            _playerMovement.WallJump = this;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {           
            velocity = new Vector2(velocity.x,_verticalForce / _verticalDuration + Physics2D.gravity.y * _playerMovement.Rigidbody.gravityScale);
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            velocity = new Vector2((_horizontalForce / _horizontalDuration) * Direction,velocity.y);
        }

        public void FixedUpdate()
        {
            if(_horizontalTimer > 0)
                _horizontalTimer -= Time.fixedDeltaTime;
            if (_verticalTimer > 0)
                _verticalTimer -= Time.fixedDeltaTime;
        }

        public void OnDisable()
        {
            _playerMovement.WallJump = null;
        }
    }
}