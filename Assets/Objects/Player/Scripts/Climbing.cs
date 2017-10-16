﻿using System.Linq;
using Archon.SwissArmyLib.Utils;
using CharacterController;
using Controllers;
using UnityEngine;

/// <summary>
/// Purpose:
/// Creator:
/// </summary>
public class Climbing : MovementAbility
{
    [SerializeField]
    private float _verticalSpeed;

    [SerializeField]
    private float _horizontalSpeed;

    private Climbable _climeObject;
    private bool _climbing;
    private float _cooldown;
    private bool _waitForJumpRelease;

    public void Start()
    {
        if(_actionsController.HealthController != null)
        _actionsController.HealthController.OnDamage.AddListener(OnDamage);
    }

    private void OnDamage(Character arg0)
    {
        _cooldown = 0.2f;
    }

    public override bool HorizontalActive
    {
        get
        {
            return VerticalActive;
        }
    }

    public override bool VerticalActive
    {
        get
        {
            if (_climeObject == null)
                _climeObject = GetClosestClimable();

            var input = _climeObject != null &&
                        (_actionsController.App.C.PlayerActions.Up ||
                         (_actionsController.App.C.PlayerActions.Down &&
                          !_actionsController.OnGround));

            var isActive =  input && !_waitForJumpRelease
                && !_actionsController.Combat || OnLadder(_actionsController.GroundCollisionCheck) != null 
                && _actionsController.App.C.PlayerActions.Down 
                && !_actionsController.Combat;

            if (!_climbing && isActive && _cooldown <= 0)
            {
                _actionsController.StartClimbing.Value = true;
                _climbing = true;
            }

            if (_actionsController.App.C.PlayerActions.ProxyInputActions.Jump.WasPressed && (isActive || _climbing))
            {
                _climbing = false;
                isActive = false;
                _cooldown = 0.2f;
            }
            return (isActive || _climbing) && _cooldown <= 0;
        }
    }

    public override void HandleVertical(ref Vector2 velocity)
    {
        
        if (_climeObject)
            velocity = new Vector2(velocity.x, velocity.y + _actionsController.Rigidbody.CounterGravity(_verticalSpeed * _actionsController.Vertical + _climeObject.Resistance));

        Collider2D col = OnLadder(_actionsController.GroundCollisionCheck);
        if (col != null && _actionsController.App.C.PlayerActions.Down)
        {
            if(_actionsController.ModificationHandler.ActiveModifiers.FirstOrDefault(x => x.Name == "ChangeLayerOf" + col.gameObject.name) == null)
                _actionsController.ModificationHandler.AddModification(
                    new TemporaryLayerChange("ChangeLayerOf" + col.gameObject.name, "NonPlayerCollision", col, _actionsController.NonPlatformTriggerCheck, 0.5f));
        }
    }

    public override void HandleHorizontal(ref Vector2 velocity)
    {
        if (_climeObject)
            velocity = new Vector2(_horizontalSpeed * _actionsController.Horizontal + _climeObject.Resistance, velocity.y);
    }

    private Collider2D OnLadder(CollisionCheck collisionCheck)
    {
        return collisionCheck.Sides.TargetColliders.FirstOrDefault(c => c.gameObject.tag == "Ladder");
    }

    private Climbable GetClosestClimable()
    {
        Climbable ca = null;
        if (!_actionsController.NonPlatformTriggerCheck.IsColliding())
            return null;
        var distance = float.MaxValue;
        foreach (var c in _actionsController.NonPlatformTriggerCheck.Sides.TargetColliders)
        {
            var tempDistance = Mathf.Abs(_actionsController.NonPlatformTriggerCheck.CollidersToCheck[0].bounds.center.y - c.bounds.center.y);

            if (tempDistance < distance && c.gameObject.tag == "Ladder")
            {
                Climbable caTemp = c.gameObject.GetComponent<Climbable>();
                if (caTemp != null)
                {
                    distance = tempDistance;
                    ca = caTemp;
                }

            }

        }
        return ca;
    }

    public void Update()
    {
        if (_actionsController.App.C.PlayerActions.ProxyInputActions.AnyWasPressed &&
            _waitForJumpRelease || _actionsController.OnGround)
            _waitForJumpRelease = false;
    }

    public void LateUpdate()
    {
        _climeObject = null;
        if (_climbing && (OnLadder(_actionsController.NonPlatformTriggerCheck) == null ) ||
            _actionsController.Combat)
            _climbing = false;
        if (_cooldown > 0)
            _cooldown -= BetterTime.DeltaTime;

            
    }

    public void ResetClimb()
    {
        _actionsController.ClimbEnd = true;
        _actionsController.Animator.SetBool("ClimbEnd",true);
    }
}