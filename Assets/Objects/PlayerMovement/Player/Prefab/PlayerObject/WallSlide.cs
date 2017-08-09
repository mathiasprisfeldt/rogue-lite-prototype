
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof (PlayerMovement)), ExecuteInEditMode]
    public class WallSlide : MovementAbility
    {
        [SerializeField]
        private float _wallSlideForce;

        [SerializeField]
        private bool _active;

        private bool _falling;

        public override bool VerticalActive
        {
            get
            {
                var wallJumpActive = !(_playerMovement.WallJump && _playerMovement.WallJump.HorizontalActive);
                var falling = _playerMovement.Rigidbody.velocity.y < -1 || _falling;
                var rest = (_playerMovement.TriggerSides.Left || _playerMovement.TriggerSides.Right) && _playerMovement.Velocity.y == 0;

                return wallJumpActive && falling && rest;
            }
        }

        public void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerMovement.WallSlide = this;
        }

        public void Update()
        {
            if (_falling && _playerMovement.OnGround)
                _falling = false;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            _falling = true;
            velocity += new Vector2(0, _playerMovement.Rigidbody.gravityScale * -Physics2D.gravity.y + _wallSlideForce);
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }

        public void OnDisable()
        {
            _playerMovement.WallSlide = null;
        }
    }
}
