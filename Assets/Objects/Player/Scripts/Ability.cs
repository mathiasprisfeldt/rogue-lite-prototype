﻿using System.Collections;
using System.Collections.Generic;
using CharacterController;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField]
    protected ActionsController _actionsController;

    public virtual bool VerticalActive { get; set; }
    public virtual bool HorizontalActive { get; set; }

    public virtual void Awake()
    {
        if(_actionsController == null)
            _actionsController = GetComponent<ActionsController>();
    }

    public abstract void HandleVertical(ref Vector2 velocity);
    public abstract void HandleHorizontal(ref Vector2 velocity);


}
