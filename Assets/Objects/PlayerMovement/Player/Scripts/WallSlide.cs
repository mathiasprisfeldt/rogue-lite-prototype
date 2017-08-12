
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof (PlayerMovement)), ExecuteInEditMode]
    public class WallSlide : MovementAbility
    {
        [SerializeField]
        private float _wallSlideForce;

        [SerializeField]
        private float _timeUntilSlide;

        private bool _falling;
        private float dir;
        private float _slideTimer;

        public override bool VerticalActive
        {
            get
            {
                var wallJumpActive = !(_playerMovement.WallJump && _playerMovement.WallJump.HorizontalActive);
                var falling = _playerMovement.Rigidbody.velocity.y < -1 || _falling;
                var rest = (_playerMovement.TriggerSides.Left || _playerMovement.TriggerSides.Right);

                if(wallJumpActive && falling && rest)
                {
                    dir = _playerMovement.TriggerSides.Left ? 1 : -1;
                    if (_slideTimer > 0)
                    {
                        _slideTimer -= Time.fixedDeltaTime;
                        return false;
                    }                       
                    return true;
                }
                else
                    _slideTimer = _timeUntilSlide;

                return false;
            }
        }

        public override void Awake()
        {
            base.Awake();
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
            velocity += new Vector2(0, _playerMovement.Rigidbody.CounterGravity(_wallSlideForce));
            _playerMovement.Flip(dir);
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
