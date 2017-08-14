using System.Collections;
using System.Collections.Generic;
using CharacterController;
using UnityEngine;

[RequireComponent(typeof(PlayerActions))]
public abstract class MovementAbility : MonoBehaviour
{
    protected PlayerActions _playerActions;

    public virtual bool VerticalActive { get; set; }
    public virtual bool HorizontalActive { get; set; }

    public virtual void Awake()
    {
        _playerActions = GetComponent<PlayerActions>();
    }

    public abstract void HandleVertical(ref Vector2 velocity);
    public abstract void HandleHorizontal(ref Vector2 velocity);


}
