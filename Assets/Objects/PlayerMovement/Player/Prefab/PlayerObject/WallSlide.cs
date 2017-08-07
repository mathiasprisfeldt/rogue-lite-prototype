using System.Collections;
using System.Collections.Generic;
using RogueLiteMovement;
using UnityEngine;


[RequireComponent(typeof(PlayerMovement)), ExecuteInEditMode]
public class WallSlide : MonoBehaviour
{
    [SerializeField]
    private float _wallSlideForce;

    [SerializeField]
    private bool _active;

    private PlayerMovement _playerMovement;
    private float _oldGrav;

    public void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.WallSlide = this;
    }

    public void ApplyWallSlide(ref float force)
    {
        //if (_playerMovement.PlayerState == PlayerState.OnWall || _active)
        //{
        //    if (_playerMovement.Rigidbody.gravityScale != 0)
        //        _oldGrav = _playerMovement.Rigidbody.gravityScale;
        //    _playerMovement.Rigidbody.gravityScale = 0;
        //    force = _wallSlideForce;
        //}
        //else if(_oldGrav > 0)
        //{
        //    _playerMovement.Rigidbody.gravityScale = _oldGrav;
        //    _oldGrav = 0;
        //}
            
    }

    public void OnDisable()
    {
        _playerMovement.WallSlide = null;
    }
}
