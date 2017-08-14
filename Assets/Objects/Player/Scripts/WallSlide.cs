
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof (Action))]
    public class WallSlide : global::Ability
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
                var wallJumpActive = !(_action.WallJump && _action.WallJump.HorizontalActive);
                var falling = _action.Rigidbody.velocity.y < -1 || _falling;
                var rest = (_action.WallSlideCheck.Left || _action.WallSlideCheck.Right);

                if(wallJumpActive && falling && rest)
                {
                    dir = _action.TriggerCheck.Sides.Left ? 1 : -1;
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
                       !(_action.WallSlideCheck.Left && _action.App.C.PlayerActions.Right ||
                       _action.WallSlideCheck.Right && _action.App.C.PlayerActions.Left);
            }
        }

        public void Update()
        {
            if (_falling && _action.OnGround)
                _falling = false;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            _falling = true;
            velocity += new Vector2(0, _action.Rigidbody.CounterGravity(_wallSlideForce));
            _action.Flip(dir);
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            velocity = new Vector2(0,velocity.y);
        }
    }
}
