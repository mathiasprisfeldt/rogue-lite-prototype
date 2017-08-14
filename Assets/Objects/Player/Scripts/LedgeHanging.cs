using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{

    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(ActionController)), ExecuteInEditMode]
    public class LedgeHanging : MovementAbility
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
                var left = _actionController.TriggerCheck.Sides.Left;
                var right = _actionController.TriggerCheck.Sides.Right;
                var horizontalMovement = _actionController.App.C.PlayerActions.Right && left ||
                                         _actionController.App.C.PlayerActions.Left && right;
                if (_downTimer > 0 || _upTimer > 0)
                    return true;
                if ((right || left) && !(_actionController.WallJump && _actionController.WallJump.HorizontalActive) && _hangCooldownTimer <= 0)
                {
                    List<Collider2D> colliders = right ? _actionController.TriggerCheck.Sides.RightColliders : _actionController.TriggerCheck.Sides.LeftColliders;
                    Collider2D col = colliders[0];
                    var distance = float.MaxValue;
                    foreach (var c in colliders)
                    {
                        var tempDistance = Mathf.Abs(_actionController.TriggerCheck.Colliders[0].bounds.center.y   - c.bounds.center.y);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            col = c;
                        }
                            
                    }

                    float hangPosX = right ? col.bounds.min.x : col.bounds.max.x;
                    var hangPosition = new Vector2(hangPosX, col.bounds.max.y - _hangDistance);
                    var thisColX = right ? _actionController.TriggerCheck.Colliders[0].bounds.max.x : _actionController.TriggerCheck.Colliders[0].bounds.min.x;
                    Vector2 temp = new Vector2(thisColX, _actionController.CollisionCheck.Colliders[0].bounds.center.y);
                    if (Mathf.Abs(temp.y - hangPosition.y) <= _sensitivity)
                    {
                        TileBehaviour tile = col.gameObject.GetComponent<TileBehaviour>();
                        if (tile && tile.TopCollision)
                            return false;

                        PlatformBehavior platform = col.gameObject.GetComponent<PlatformBehavior>();
                        if (platform && !platform.Istop)
                            return false;

                        if (_actionController.App.C.PlayerActions.Down &&
                            _actionController.App.C.PlayerActions.Jump.IsPressed)
                        {
                            _downTimer = _pushDownDuration;
                            return true;
                        }
                        else if (_actionController.App.C.PlayerActions.Jump.WasPressed)
                        {
                            _upTimer = _pushUpDuration;
                            return true;
                        }

                        if (horizontalMovement)
                            return false;

                        var extend = right ? -_actionController.CollisionCheck.Colliders[0].bounds.extents.x : _actionController.CollisionCheck.Colliders[0].bounds.extents.x;
                        _actionController.Rigidbody.position = Vector2.Lerp(_actionController.Rigidbody.position, new Vector2(hangPosition.x + extend, hangPosition.y), .6f);
                        var dir = left ? -1 : 1;
                        _actionController.Flip(dir);
                        return true;
                    }
                }
                return false;
            }
        }

        public override void Awake()
        {
            base.Awake();
            _actionController.LedgeHanging = this;
        }

        public void OnDisable()
        {
            _actionController.LedgeHanging = null;
        }

        public void FixedUpdate()
        {
            if (_hangCooldownTimer > 0)
                _hangCooldownTimer -= Time.fixedDeltaTime;

            if (_upTimer > 0)
            {
                _upTimer -= Time.fixedDeltaTime;
            }
                
            if (_downTimer > 0)
                _downTimer -= Time.fixedDeltaTime;
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            if(_upTimer > 0 || _downTimer > 0)
                _actionController.LastUsedVerticalAbility = Ability.None;

            var temp = 0f;
            if (_upTimer > 0)
                temp += _pushUpForce / _pushUpDuration;
            if (_downTimer > 0)
                temp -= _pushDownForce / _pushDownDuration;


            velocity = new Vector2(velocity.x, _actionController.Rigidbody.CounterGravity(temp));
        }
    }
}