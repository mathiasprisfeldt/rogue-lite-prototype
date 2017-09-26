using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDash : EnemyState
{
    [SerializeField]
    private float _range;

    private bool _isDashing;

    public override void Act(float deltaTime)
    {
        base.Act(deltaTime);

        //Dash in the saved direction
    }

    public override void Begin()
    {
        base.Begin();

        //Find the direction to dash, and save for later
    }

    /// <summary>
    /// Reason
    /// </summary>
    public override void Reason()
    {
        base.Reason();

        if (!Context.C.Target // Cooldown
            )
            ChangeState<EnemyIdle>();
    }

    /// <summary>
    /// Returns true when this state shoud be selected
    /// </summary>
    /// <returns></returns>
    public override bool ShouldTakeover()
    {
        if (Context.C.Target && Context.C.ToPlayer.magnitude < _range &&  && 
            !IsState<EnemyAttack>() && !IsState<EnemyAvoid>())
            return true;

        return false;
    }
}
