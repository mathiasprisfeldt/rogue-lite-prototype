using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{

    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(Action))]
    public class LedgeHanging : global::Ability
    {
        [SerializeField]
        private float _hangDistance;

        [SerializeField]
        private float _sensitivity;

        [SerializeField]
        private float _pushUpForce;

        [SerializeField]
        private float _pushUpDuration;

        [SerializeField]
        private float _pushDownForce;

        [SerializeField]
        private float _pushDownDuration;

        private float _hangCooldownTimer;
        private float _downTimer;
        private float _upTimer;

        public override bool VerticalActive
        {
            get
            {
                if (_action == null)
                    return false;
                var left = _action.TriggerCheck.Sides.Left;
                var right = _action.TriggerCheck.Sides.Right;
                var horizontalMovement = _action.App.C.PlayerActions.Right && left ||
                                         _action.App.C.PlayerActions.Left && right;
                if (_downTimer > 0 || _upTimer > 0)
                    return true;
                if ((right || left) && !(_action.WallJump && _action.WallJump.HorizontalActive) && _hangCooldownTimer <= 0)
                {
                    List<Collider2D> colliders = right ? _action.TriggerCheck.Sides.RightColliders : _action.TriggerCheck.Sides.LeftColliders;
                    Collider2D col = colliders[0];
                    var distance = float.MaxValue;
                    foreach (var c in colliders)
                    {
                        var tempDistance = Mathf.Abs(_action.TriggerCheck.CollidersToCheck[0].bounds.center.y   - c.bounds.center.y);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            col = c;
                        }
                            
                    }

                    float hangPosX = right ? col.bounds.min.x : col.bounds.max.x;
                    var hangPosition = new Vector2(hangPosX, col.bounds.max.y - _hangDistance);
                    var thisColX = right ? _action.TriggerCheck.CollidersToCheck[0].bounds.max.x : _action.TriggerCheck.CollidersToCheck[0].bounds.min.x;
                    Vector2 temp = new Vector2(thisColX, _action.CollisionCheck.CollidersToCheck[0].bounds.center.y);
                    if (Mathf.Abs(temp.y - hangPosition.y) <= _sensitivity)
                    {
                        TileBehaviour tile = col.gameObject.GetComponent<TileBehaviour>();
                        if (tile && tile.TopCollision)
                            return false;

                        PlatformBehavior platform = col.gameObject.GetComponent<PlatformBehavior>();
                        if (platform && !platform.Istop)
                            return false;

                        if (_action.App.C.PlayerActions.Down &&
                            _action.App.C.PlayerActions.Jump.IsPressed)
                        {
                            _downTimer = _pushDownDuration;
                            return true;
                        }
                        else if (_action.App.C.PlayerActions.Jump.WasPressed)
                        {
                            _upTimer = _pushUpDuration;
                            return true;
                        }

                        if (horizontalMovement)
                            return false;

                        var extend = right ? -_action.CollisionCheck.CollidersToCheck[0].bounds.extents.x : _action.CollisionCheck.CollidersToCheck[0].bounds.extents.x;
                        _action.Rigidbody.position = Vector2.Lerp(_action.Rigidbody.position, new Vector2(hangPosition.x + extend, hangPosition.y), .6f);
                        var dir = left ? -1 : 1;
                        _action.Flip(dir);
                        return true;
                    }
                }
                return false;
            }
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            if (_upTimer > 0)
                _upTimer -= Time.fixedDeltaTime;

            if (_downTimer > 0)
                _downTimer -= Time.fixedDeltaTime;  

            if (_upTimer > 0 || _downTimer > 0)
                _action.LastUsedVerticalAbility = Ability.None;

            var temp = 0f;
            if (_upTimer > 0)
                temp += _pushUpForce / _pushUpDuration;
            if (_downTimer > 0)
                temp -= _pushDownForce / _pushDownDuration;
                velocity = new Vector2(velocity.x, _action.Rigidbody.CounterGravity(temp));
            
        }
    }
}