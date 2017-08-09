using System;
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
                if((right || left) && _playerMovement.WallJump && !_playerMovement.WallJump.HorizontalActive && !_playerMovement.App.C.PlayerActions.Down.IsPressed)
                {
                    var col = right ? _playerMovement.TriggerSides.RightColliders[0] : _playerMovement.TriggerSides.LeftColliders[0];
                    float hangPosX = right ? col.bounds.min.x : col.bounds.max.x;
                    var hangPosition = new Vector2(hangPosX,col.bounds.max.y - _hangDistance);
                    foreach (var c in _playerMovement.Sides.Colliders)
                    {
                        var thisColX = right ? _playerMovement.CollisionCheck.Colliders[0].bounds.max.x : _playerMovement.CollisionCheck.Colliders[0].bounds.min.x;
                        Vector2 temp = new Vector2(thisColX, _playerMovement.TriggerCheck.Colliders[0].bounds.center.y);
                        if (Vector2.Distance(temp,hangPosition) <= _sensitivity)
                        {
                            var extend = right ? -c.bounds.extents.x : c.bounds.extents.x;
                            _playerMovement.Rigidbody.position = Vector2.Lerp(_playerMovement.Rigidbody.position, new Vector2(hangPosition.x + extend, hangPosition.y),.35f);
                            return true;
                        }
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
            throw new NotImplementedException();
        }

        public override void HandleVertical(ref Vector2 velocity)
        {
            throw new NotImplementedException();
        }
    }
}