using RogueLiteMovement;
using UnityEngine;

/// <summary>
/// Purpose:
/// Creator:
/// </summary>
[RequireComponent(typeof(PlayerMovement)), ExecuteInEditMode]
public class DoubleJump : MonoBehaviour
{
    [SerializeField]
    private float _jumpForce;

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
            _playerMovement.PlayerState == PlayerState.InAir)
        {
            _playerMovement.Rigidbody.velocity = new Vector2(_playerMovement.Rigidbody.velocity.x, _jumpForce * Time.fixedDeltaTime);
            _hasJumped = true;
        }
            
        else if (_hasJumped && _playerMovement.PlayerState != PlayerState.InAir)
            _hasJumped = false;
    }

    public void OnDisable()
    {
        _playerMovement.DoubleJump = null;
    }

}
