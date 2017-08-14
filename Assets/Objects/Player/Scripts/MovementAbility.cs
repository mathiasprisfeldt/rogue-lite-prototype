using System.Collections;
using System.Collections.Generic;
using CharacterController;
using UnityEngine;

[RequireComponent(typeof(ActionController))]
public abstract class MovementAbility : MonoBehaviour
{
    protected ActionController _actionController;

    public virtual bool VerticalActive { get; set; }
    public virtual bool HorizontalActive { get; set; }

    public virtual void Awake()
    {
        _actionController = GetComponent<ActionController>();
    }

    public abstract void HandleVertical(ref Vector2 velocity);
    public abstract void HandleHorizontal(ref Vector2 velocity);


}
