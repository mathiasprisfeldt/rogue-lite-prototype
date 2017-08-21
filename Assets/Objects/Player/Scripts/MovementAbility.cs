using System.Collections;
using System.Collections.Generic;
using CharacterController;
using UnityEngine;

public abstract class MovementAbility : Ability
{
    [SerializeField]
    protected ActionsController _actionsController;

    public virtual bool VerticalActive
    {
        get { return _active; } 
    }

    public virtual bool HorizontalActive
    {
        get { return _active; }
    }

    public virtual void Awake()
    {
        if(_actionsController == null)
            _actionsController = GetComponent<ActionsController>();
    }

    public abstract void HandleVertical(ref Vector2 velocity);
    public abstract void HandleHorizontal(ref Vector2 velocity);


}
