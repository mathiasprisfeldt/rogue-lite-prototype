using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(PlayerMovement)), ExecuteInEditMode]
    public class LedgeHanging : MovementAbility
    {
        [SerializeField]
        private float _hangDistance;

        [SerializeField]
        private float _sensitivity;

        public override bool VerticalActive
        {
            get
            {
                var left = _playerMovement.TriggerSides.Left;
                var right = _playerMovement.TriggerSides.Right;
                var horizontalMovement = _playerMovement.App.C.PlayerActions.Right.IsPressed && left ||
                                         _playerMovement.App.C.PlayerActions.Left.IsPressed && right;
                if ((right || left) && (_playerMovement.WallJump && !_playerMovement.WallJump.HorizontalActive) && 
                    !(_playerMovement.App.C.PlayerActions.Down.IsPressed && _playerMovement.App.C.PlayerActions.Jump.IsPressed) && !horizontalMovement)
                {
                    List<Collider2D> colliders = right ? _playerMovement.TriggerSides.RightColliders : _playerMovement.TriggerSides.LeftColliders;
                    Collider2D col = colliders[0];
                    var distance = float.MaxValue;
                    foreach (var c in colliders)
                    {
                        var tempDistance = Mathf.Abs(_playerMovement.TriggerCheck.Colliders[0].bounds.center.y   - c.bounds.center.y);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            col = c;
                        }
                            
                    }

                    float hangPosX = right ? col.bounds.min.x : col.bounds.max.x;
                    var hangPosition = new Vector2(hangPosX, col.bounds.max.y - _hangDistance);
                    var thisColX = right ? _playerMovement.TriggerCheck.Colliders[0].bounds.max.x : _playerMovement.TriggerCheck.Colliders[0].bounds.min.x;
                    Vector2 temp = new Vector2(thisColX, _playerMovement.TriggerCheck.Colliders[0].bounds.center.y);
                    if (Mathf.Abs(temp.y -hangPosition.y) <= _sensitivity)
                    {
                        var tile = col.gameObject.GetComponent<TileBehaviour>();
                        if (tile && tile.TopCollision)
                            return false;
                        var extend = right ? -_playerMovement.CollisionCheck.Colliders[0].bounds.extents.x : _playerMovement.CollisionCheck.Colliders[0].bounds.extents.x;
                        _playerMovement.Rigidbody.position = Vector2.Lerp(_playerMovement.Rigidbody.position, new Vector2(hangPosition.x + extend, hangPosition.y), .35f);
                        var dir = left ? 1 : -1;
                        _playerMovement.Flip(dir);
                        return true;
                    }
                }
                return false;
            }
        }

        public void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerMovement.LedgeHanging = this;
        }

        public void OnDisable()
        {
            _playerMovement.LedgeHanging = null;
        }

        public override void HandleHorizontal(ref Vector2 velocity)
        {
            velocity = new Vector2(velocity.x, _playerMovement.Rigidbody.gravityScale * -Physics2D.gravity.y);
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            throw new NotImplementedException();
        }
    }
}