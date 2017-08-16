using System.Collections;
using System.Collections.Generic;
using CharacterController;
using UnityEngine;

public abstract class MovementAbility : MonoBehaviour
{
    [SerializeField]
    protected ActionsController _actionsController;

    [SerializeField]
    protected bool _active = true;

    public virtual bool VerticalActive
    {
        get { return _active; } 
    }

    public virtual bool HorizontalActive
    {
        get { return _active; }
    }

    protected bool Active
    {
        get { return _active; }
        set { _active = value; }
    }

    public virtual void Awake()
    {
        if(_actionsController == null)
            _actionsController = GetComponent<ActionsController>();
    }

    public abstract void HandleVertical(ref Vector2 velocity);
    public abstract void HandleHorizontal(ref Vector2 velocity);


}
