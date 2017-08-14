
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof (ActionController)), ExecuteInEditMode]
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
                var wallJumpActive = !(_actionController.WallJump && _actionController.WallJump.HorizontalActive);
                var falling = _actionController.Rigidbody.velocity.y < -1 || _falling;
                var rest = (_actionController.WallSlideCheck.Left || _actionController.WallSlideCheck.Right);

                if(wallJumpActive && falling && rest)
                {
                    dir = _actionController.TriggerCheck.Sides.Left ? 1 : -1;
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
                       !(_actionController.WallSlideCheck.Left && _actionController.App.C.PlayerActions.Right ||
                       _actionController.WallSlideCheck.Right && _actionController.App.C.PlayerActions.Left);
            }
        }

        public override void Awake()
        {
            base.Awake();
            _actionController.WallSlide = this;
        }

        public void Update()
        {
            if (_falling && _actionController.OnGround)
                _falling = false;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            _falling = true;
            velocity += new Vector2(0, _actionController.Rigidbody.CounterGravity(_wallSlideForce));
            _actionController.Flip(dir);
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            velocity = new Vector2(0,velocity.y);
        }

        public void OnDisable()
        {
            _actionController.WallSlide = null;
        }
    }
}
