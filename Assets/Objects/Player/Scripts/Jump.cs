using System.Collections;
using System.Collections.Generic;
using AcrylecSkeleton.ModificationSystem;
using InControl;
using RogueLiteInput;
using UnityEngine;

public class Jump : Ability {

    [SerializeField]
    private float _jumpForce;

    [SerializeField]
    private float _initialJumpForce;

    [SerializeField]
    private float _initialJumpDuration;

    [SerializeField]
    private float _jumpDuration;

    private float _jumpTimer;
    private float _initialJumpTimer;

    // Update is called once per frame
    void Update ()
    {
        if (_jumpTimer > 0 && _initialJumpTimer <= 0)
            _jumpTimer -= Time.deltaTime;
        if (_initialJumpTimer > 0)
            _initialJumpTimer -= Time.deltaTime;
    }

    public override bool VerticalActive
    {
        get
        {
            InputActions pa = _action.App.C.PlayerActions;
            if (pa != null && pa.Jump.WasPressed && _action.OnGround && _jumpTimer <= 0)
            {
                _jumpTimer = _jumpDuration;
                _initialJumpTimer = _initialJumpDuration;
            }

            if(_initialJumpTimer <= 0)
                if (!(pa != null && pa.Jump.IsPressed && _jumpTimer > 0))
                    _jumpTimer = 0;

            if ((_action.GroundCollisionCheck.Sides.BottomColliders != null && _action.GroundCollisionCheck.Sides.BottomColliders.Count > 0 && pa != null && pa.Down && pa.Jump.WasPressed))
                return true;

            return _jumpTimer > 0 || _initialJumpTimer > 0;
        }
    }

    public override void HandleVertical(ref Vector2 velocity)
    {
        var force = _action.Rigidbody.CalculateVerticalSpeed(_jumpForce / _jumpDuration);
        if (_initialJumpTimer > 0)
        {
            force = _action.Rigidbody.CalculateVerticalSpeed(_initialJumpForce / _initialJumpDuration);
        }

        velocity = new Vector2(velocity.x, force);

        if (_action.GroundCollisionCheck.Sides.BottomColliders != null && _action.GroundCollisionCheck.Sides.BottomColliders.Count > 0 
            && _action.App.C.PlayerActions.Down)
        {
            
            foreach (var c in _action.GroundCollisionCheck.Sides.BottomColliders)
            {
                if (c.gameObject.tag == "OneWayCollider")
                {
                    velocity = new Vector2(velocity.x, 0);
                    _action.ModificationHandler.AddModification(new TemporaryLayerChange(0.4f, "ChangeLayerOf" + c.gameObject.name, "NonPlayerCollision", c.gameObject));
                }
                
            }
        }

    }

    public override void HandleHorizontal(ref Vector2 velocity)
    {
        
    }
}

public class TemporaryLayerChange : Modification
{
    private LayerMask _targetLayer;
    private LayerMask _oldLayer;
    private GameObject _targetObject;

    public TemporaryLayerChange(float time, string name, string targetLayer, GameObject targetObject) : base(time, name)
    {
        _targetLayer = LayerMask.NameToLayer(targetLayer);
        if (_targetLayer == -1)
        {
            Debug.LogWarning(targetLayer + " layer dose not exist!");
            RemoveModificaiton();
        }

        else
        {
            _oldLayer = targetObject.layer;
            targetObject.layer = _targetLayer;
            _targetObject = targetObject;
        }
    }

    public override void ApplyModificaiton()
    {
    }

    public override void RemoveModificaiton()
    {
        if (_targetObject)
            _targetObject.layer = _oldLayer;
    }

    public override void UpdateModificaiton()
    {
    }

    public override void FixedUpdateModificaiton()
    {
    }
}

