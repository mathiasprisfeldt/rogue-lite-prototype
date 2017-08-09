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
        private float _wallJumpTimer;

        [SerializeField]
        private float _verticalForce;

        [SerializeField]
        private float _horizontalForce;

        [SerializeField]
        private float _duration;

        public float Direction { get; private set; }

        public override bool VerticalActive
        {
            get
            {
                var input = _playerMovement.App.C.PlayerActions != null && _playerMovement.App.C.PlayerActions.Jump.WasPressed;
                var collision = (_playerMovement.TriggerSides.Left || _playerMovement.TriggerSides.Right) && !_playerMovement.GroundCollisionCheck.Bottom;
                return input && collision;
            }
        }

        public override bool HorizontalActive { get { return _wallJumpTimer > 0; } }

        public void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerMovement.WallJump = this;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            Direction = _playerMovement.TriggerSides.Left ? 1 : -1;
            _wallJumpTimer = _duration;
            velocity = new Vector2(0,_verticalForce);
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            velocity = new Vector2((_horizontalForce / _duration) * Direction,velocity.y);
        }

        public void Update()
        {
            if(_wallJumpTimer > 0)
                _wallJumpTimer -= Time.deltaTime;
        }

        public void OnDisable()
        {
            _playerMovement.WallJump = null;
        }
    }
}