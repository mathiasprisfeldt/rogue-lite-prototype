using System;
using UnityEngine;

/// <summary>
/// Purpose:
/// Creator:
/// </summary>

namespace CharacterController
{
    [RequireComponent(typeof (PlayerMovement)), ExecuteInEditMode]
    public class DoubleJump : MonoBehaviour
    {
        [SerializeField] private float _jumpForce;

        private PlayerMovement _playerMovement;
        private bool _hasJumped;

        public void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerMovement.DoubleJump = this;
        }

        public void FixedUpdate()
        {

            if (!_hasJumped && _playerMovement.App.C.PlayerActions.Jump.WasPressed &&
                _playerMovement.State == CharacterState.InAir)
            {
                _playerMovement.Rigidbody.velocity = new Vector2(_playerMovement.Rigidbody.velocity.x,
                    _jumpForce*Time.fixedDeltaTime);
                _hasJumped = true;
            }

            else if (_hasJumped && _playerMovement.State != CharacterState.InAir)
                _hasJumped = false;
        }

        public void OnDisable()
        {
            _playerMovement.DoubleJump = null;
        }

    }
}
