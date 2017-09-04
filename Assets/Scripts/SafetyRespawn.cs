using System;
using AcrylecSkeleton.Extensions;
using Controllers;
using UnityEngine;


/// <summary>
/// Purpose:
/// Creator:
/// </summary>
public class SafetyRespawn : MonoBehaviour
{
    [SerializeField]
    private Character _character;

    [SerializeField]
    private CollisionCheck _collisionCheck;

    [SerializeField]
    private CollisionCheck _hitBox;

    private Vector3 _lastSavePosition;

    public void Update()
    {
        if (_collisionCheck.Bottom && _hitBox.IsColliding() == false)
            _lastSavePosition = transform.position;
    }

    public void Respawn()
    {
        float yDiffernce = Math.Abs(_lastSavePosition.y - transform.position.y);
        if (_character == null || yDiffernce < 0.7f)
            return;

        _character.OnSafetyRespawn.Invoke();
            
        _character.Rigidbody.position = _lastSavePosition;
        if (_character.KnockbackHandler)
        {
            _character.KnockbackHandler.Clear();
        }
        _character.Rigidbody.velocity = Vector2.zero;
    }
}