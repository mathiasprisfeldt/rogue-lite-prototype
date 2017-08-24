using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{

    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
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
        private bool _startGrab;

        public override bool VerticalActive
        {
            get
            {
                if (_actionsController == null)
                    return false;

                if (!base.VerticalActive)
                    return false;

                var left = _actionsController.TriggerCheck.Sides.Left;
                var right = _actionsController.TriggerCheck.Sides.Right;
                var horizontalMovement = _actionsController.App.C.PlayerActions != null && (_actionsController.App.C.PlayerActions.Right && left ||
                                         _actionsController.App.C.PlayerActions.Left && right);
                if (_downTimer > 0 || _upTimer > 0)
                    return true;
                if ((right || left) && !(_actionsController.AbilityReferences.WallJump && _actionsController.AbilityReferences.WallJump.HorizontalActive) && _hangCooldownTimer <= 0 
                    && _actionsController.LastUsedCombatAbility == CombatAbility.None && !_actionsController.OnGround)
                {
                    List<Collider2D> colliders = right ? _actionsController.TriggerCheck.Sides.RightColliders : _actionsController.TriggerCheck.Sides.LeftColliders;
                    Collider2D col = colliders[0];
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

                    float hangPosX = right ? col.bounds.min.x : col.bounds.max.x;
                    var hangPosition = new Vector2(hangPosX, col.bounds.max.y - _hangDistance);
                    var thisColX = right ? _actionsController.TriggerCheck.CollidersToCheck[0].bounds.max.x : _actionsController.TriggerCheck.CollidersToCheck[0].bounds.min.x;
                    Vector2 temp = new Vector2(thisColX, _actionsController.CollisionCheck.CollidersToCheck[0].bounds.center.y);
                    if (Mathf.Abs(temp.y - hangPosition.y) <= _sensitivity)
                    {
                        TileBehaviour tile = col.gameObject.GetComponent<TileBehaviour>();
                        if (tile && tile.TopCollision)
                            return false;

                        PlatformBehavior platform = col.gameObject.GetComponent<PlatformBehavior>();
                        if (platform && (!platform.Istop || left && platform.Right || right && platform.Left))
                            return false;

                        if (col.gameObject.tag == "Ladder")
                            return false;

                        if (_actionsController.App.C.PlayerActions.Down &&
                            _actionsController.App.C.PlayerActions.Jump.IsPressed)
                        {
                            _downTimer = _pushDownDuration;
                            return true;
                        }
                        else if (_actionsController.App.C.PlayerActions.Jump.IsPressed)
                        {
                            _upTimer = _pushUpDuration;
                            return true;
                        }
                        
                        if (horizontalMovement)
                            return false;

                        var extend = right ? -_actionsController.CollisionCheck.CollidersToCheck[0].bounds.extents.x : _actionsController.CollisionCheck.CollidersToCheck[0].bounds.extents.x;
                        _actionsController.Rigidbody.position = Vector2.Lerp(_actionsController.Rigidbody.position, new Vector2(hangPosition.x + extend, hangPosition.y), .6f);
                        var dir = left ? -1 : 1;
                        _actionsController.Flip(dir);

                        if (!_startGrab)
                        {
                            _startGrab = true;
                            _actionsController.StartGrab.Value = true;
                        }
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
                _actionsController.LastUsedVerticalMoveAbility = MoveAbility.None;

            var temp = 0f;
            if (_upTimer > 0)
                temp += _pushUpForce / _pushUpDuration;
            if (_downTimer > 0)
                temp -= _pushDownForce / _pushDownDuration;
                velocity = new Vector2(velocity.x, _actionsController.Rigidbody.CounterGravity(temp));
            
        }
        public void Update()
        {
            if (_startGrab && !VerticalActive)
                _startGrab = false;
        }
    }


}