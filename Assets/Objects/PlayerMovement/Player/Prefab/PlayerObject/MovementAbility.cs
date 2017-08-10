using System.Collections;
using System.Collections.Generic;
using CharacterController;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public abstract class MovementAbility : MonoBehaviour
{
    protected PlayerMovement _playerMovement;

    public virtual bool VerticalActive { get; set; }
    public virtual bool HorizontalActive { get; set; }

    public virtual void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public abstract void HandleVertical(ref Vector2 velocity);
    public abstract void HandleHorizontal(ref Vector2 velocity);


}
