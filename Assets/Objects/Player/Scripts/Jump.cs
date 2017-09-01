using System.Collections;
using System.Collections.Generic;
using AcrylecSkeleton.ModificationSystem;
using InControl;
using RogueLiteInput;
using UnityEngine;

public class Jump : MovementAbility {

    [SerializeField]
    private float _jumpForce;

    [SerializeField]
    private float _initialJumpForce;

    [SerializeField]
    private float _initialJumpDuration;

    [SerializeField]
    private float _jumpDuration;

    [SerializeField]
    private float _gracePeriod;

    private float _jumpTimer;
    private float _initialJumpTimer;
    private float _gracePeriodTimer;

    // Update is called once per frame
    void Update ()
    {
        if (_actionsController.OnGround && _jumpTimer <= 0)
            _gracePeriodTimer = _gracePeriod;
        else if (_gracePeriodTimer > 0)
            _gracePeriodTimer -= Time.deltaTime;


        if (_jumpTimer > 0 && _initialJumpTimer <= 0)
            _jumpTimer -= Time.deltaTime;
        if (_initialJumpTimer > 0)
            _initialJumpTimer -= Time.deltaTime;
    }

    public override bool VerticalActive
    {
        get
        {
            InputActions pa = _actionsController.App.C.PlayerActions;
            if (!base.VerticalActive)
                return false;
            if (pa != null && pa.ProxyInputActions.Jump.WasPressed && _gracePeriodTimer > 0 && _jumpTimer <= 0)
            {
                _jumpTimer = _jumpDuration;
                _initialJumpTimer = _initialJumpDuration;
                _actionsController.StartJump.Value = true;
                _gracePeriodTimer = 0;
            }

            if(_initialJumpTimer <= 0)
                if (!(pa != null && pa.Jump.IsPressed) && _jumpTimer > 0)
                {
                    _jumpTimer = 0;
                }
                    

            if ((_actionsController.GroundCollisionCheck.Sides.BottomColliders != null 
                && _actionsController.GroundCollisionCheck.Sides.BottomColliders.Count > 0 
                && pa != null && pa.Down && pa.ProxyInputActions.Jump.WasPressed))
                return true;

            if (_actionsController.AbilityReferences.Dash.InitialDash)
            {
                _jumpTimer = 0;
                _initialJumpTimer = 0;
                _gracePeriodTimer = 0;
            }

            return _jumpTimer > 0 || _initialJumpTimer > 0;
        }
    }

    public override void HandleVertical(ref Vector2 velocity)
    {
        var force = _actionsController.Rigidbody.CalculateVerticalSpeed(_jumpForce / _jumpDuration);
        if (_initialJumpTimer > 0)
        {
            force = _actionsController.Rigidbody.CalculateVerticalSpeed(_initialJumpForce / _initialJumpDuration);
        }

        velocity = new Vector2(velocity.x, force);

        

    }

    public override void HandleHorizontal(ref Vector2 velocity)
    {
        
    }
}



