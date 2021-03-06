
using System.Collections.Generic;
using Archon.SwissArmyLib.Utils;
using UnityEngine;

namespace CharacterController
{
    public class WallSlide : MovementAbility
    {
        [SerializeField]
        private float _wallSlideForce;

        [SerializeField]
        private float _timeUntilSlide;

        private bool _falling;
        private float dir;
        private float _slideTimer;
        private bool _isDirty;
        private bool _dirtyValue;

        public override bool VerticalActive
        {
            get
            {
                if (_isDirty)
                    return _dirtyValue;

                if (!base.VerticalActive)
                {
                    _isDirty = true;
                    _dirtyValue = false;
                    return false;
                }


                var wallJumpActive = !(_actionsController.AbilityReferences.WallJump && _actionsController.AbilityReferences.WallJump.HorizontalActive);
                var falling = _actionsController.Rigidbody.velocity.y < 0 || _falling;
                var rest = (_actionsController.WallSlideCheck.Left || _actionsController.WallSlideCheck.Right);

                List<Collider2D> colliders = _actionsController.WallSlideCheck.Right ?
                    _actionsController.WallSlideCheck.Sides.RightColliders : _actionsController.WallSlideCheck.Sides.LeftColliders;
                Collider2D col = colliders.Count > 0 ? colliders[0] : null;

                if (col != null)
                {
                    var distance = float.MaxValue;
                    foreach (var c in colliders)
                    {
                        var tempDistance = Mathf.Abs(_actionsController.TriggerCheck.CollidersToCheck[0].bounds.center.y - c.bounds.center.y);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            col = c;
                        }

                    }

                    TileBehaviour tile = col.gameObject.GetComponent<TileBehaviour>();
                    if ((tile && !(!tile.LeftCollision || !tile.RightCollision)) || tile && !tile.IsSlideable)
                    {
                        _isDirty = true;
                        _dirtyValue = false;
                        return false;
                    }

                    PlatformBehavior platform = col.gameObject.GetComponent<PlatformBehavior>();
                    if ((platform && !(!platform.Left || !platform.Right)) || platform && !platform.IsSlideable)
                    {
                        _isDirty = true;
                        _dirtyValue = false;
                        return false;
                    }
                }

                if (wallJumpActive && falling && rest && _actionsController.LastUsedCombatAbility == CombatAbility.None)
                {
                    dir = _actionsController.TriggerCheck.Sides.Left ? 1 : -1;
                    if (_slideTimer > 0)
                    {
                        _slideTimer -= BetterTime.FixedDeltaTime;
                        _isDirty = true;
                        _dirtyValue = false;
                        return false;

                    }
                    else
                    {
                        if (!_actionsController.AbilityReferences.WallJump.VerticalActive)
                        {
                            _isDirty = true;
                            _dirtyValue = true;
                            return true;
                        }
                    }
                }
                else
                    _slideTimer = _timeUntilSlide;

                _isDirty = true;
                _dirtyValue = false;
                return false;
            }
        }

        public override bool HorizontalActive
        {
            get
            {
                if (!base.HorizontalActive)
                    return false;
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

        public void LateUpdate()
        {
            _isDirty = false;
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
            velocity = new Vector2(0, velocity.y);
        }
    }
}
