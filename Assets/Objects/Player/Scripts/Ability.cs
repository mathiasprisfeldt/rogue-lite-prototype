using InControl;
using UnityEngine;

/// <summary>
/// Purpose:
/// Creator:
/// </summary>
public class Ability : MonoBehaviour
{
    [SerializeField]
    protected bool _active;

    public PlayerAction ActivationAction { get; set; }

    public virtual bool Active
    {
        get { return _active; }
        set { _active = value; }
    }
}