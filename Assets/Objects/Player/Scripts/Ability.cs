using System.Collections;
using System.Collections.Generic;
using CharacterController;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField]
    protected Action _action;

    public virtual bool VerticalActive { get; set; }
    public virtual bool HorizontalActive { get; set; }

    public virtual void Awake()
    {
        if(_action == null)
            _action = GetComponent<Action>();
    }

    public abstract void HandleVertical(ref Vector2 velocity);
    public abstract void HandleHorizontal(ref Vector2 velocity);


}
