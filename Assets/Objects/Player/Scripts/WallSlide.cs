
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof (ActionsController))]
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
                var wallJumpActive = !(_actionsController.WallJump && _actionsController.WallJump.HorizontalActive);
                var falling = _actionsController.Rigidbody.velocity.y < -1 || _falling;
                var rest = (_actionsController.WallSlideCheck.Left || _actionsController.WallSlideCheck.Right);

                if(wallJumpActive && falling && rest && _actionsController.LastUsedCombatAbility == CombatAbility.None)
                {
                    dir = _actionsController.TriggerCheck.Sides.Left ? 1 : -1;
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
                       !(_actionsController.WallSlideCheck.Left && _actionsController.App.C.PlayerActions.Right ||
                       _actionsController.WallSlideCheck.Right && _actionsController.App.C.PlayerActions.Left);
            }
        }

        public void Update()
        {
            if (_falling && _actionsController.OnGround)
                _falling = false;
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            _falling = true;
            velocity += new Vector2(0, _actionsController.Rigidbody.CounterGravity(_wallSlideForce));

            if (_actionsController.LastUsedCombatAbility == CombatAbility.None)
                _actionsController.Flip(dir);
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            velocity = new Vector2(0,velocity.y);
        }
    }
}
