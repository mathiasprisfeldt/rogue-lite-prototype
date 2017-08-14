
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof (PlayerActions)), ExecuteInEditMode]
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
                var wallJumpActive = !(_playerActions.WallJump && _playerActions.WallJump.HorizontalActive);
                var falling = _playerActions.Rigidbody.velocity.y < -1 || _falling;
                var rest = (_playerActions.WallSlideCheck.Left || _playerActions.WallSlideCheck.Right);

                if(wallJumpActive && falling && rest)
                {
                    dir = _playerActions.TriggerCheck.Sides.Left ? 1 : -1;
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

        public override bool HorizontalActive
        {
            get
            {
                return VerticalActive &&
                       !(_playerActions.WallSlideCheck.Left && _playerActions.App.C.PlayerActions.Right ||
                       _playerActions.WallSlideCheck.Right && _playerActions.App.C.PlayerActions.Left);
            }
        }

        public override void Awake()
        {
            base.Awake();
            _playerActions.WallSlide = this;
        }

        public void Update()
        {
            if (_falling && _playerActions.OnGround)
                _falling = false;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            _falling = true;
            velocity += new Vector2(0, _playerActions.Rigidbody.CounterGravity(_wallSlideForce));
            _playerActions.Flip(dir);
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            velocity = new Vector2(0,velocity.y);
        }

        public void OnDisable()
        {
            _playerActions.WallSlide = null;
        }
    }
}
