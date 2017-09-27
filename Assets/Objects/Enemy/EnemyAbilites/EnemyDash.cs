using AcrylecSkeleton.Utilities;
using Archon.SwissArmyLib.Utils;
using Enemy;
using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Archon.SwissArmyLib.Events;
using System;

public class EnemyDash : EnemyState, TellMeWhen.ITimerCallback
{
    [SerializeField]
    private float _range;
    [SerializeField]
    private float _dashSpeed;
    [SerializeField]
    private Timer _dashTimer;
    [SerializeField]
    private AnimationCurve _dashCurve;
    [SerializeField]
    private float _chargeUp;

    private bool _isDashing;
    private Vector2 _dashDirection;

    public DashItem DashItem { get; set; }

    public override void Act(float deltaTime)
    {
        base.Act(deltaTime);

        if (_dashTimer.IsRunning)
        {
            Vector2 vel = new Vector2(_dashCurve.Evaluate((float)_dashTimer.Duration.TotalSeconds - Mathf.Abs((float)_dashTimer.Clock.TotalSeconds) / (float)_dashTimer.Duration.TotalSeconds) * (_dashSpeed / (float)_dashTimer.Duration.TotalSeconds) * _dashDirection.x, 0);

            vel *= deltaTime;

            Context.M.Character.SetVelocity(vel);
        }
        else
        {
            Context.M.Character.StandStill(); ;
        }
    }

    public override void Begin()
    {
        base.Begin();
        _dashDirection = new Vector2(Context.C.ToPlayer.normalized.x, 0);
        _isDashing = true;
        DashItem.Activate();
        TellMeWhen.Seconds(_chargeUp, this);
    }

    public void OnDashEnded()
    {
        _isDashing = false;
    }

    /// <summary>
    /// Reason
    /// </summary>
    public override void Reason()
    {
        base.Reason();

        if (!Context.C.Target || (!_isDashing && !_dashTimer.IsRunning))
            ChangeState<EnemyIdle>();
    }

    /// <summary>
    /// Returns true when this state should be selected
    /// </summary>
    /// <returns></returns>
    public override bool ShouldTakeover()
    {
        if (Context.C.Target && Context.C.ToPlayer.magnitude < _range &&
            DashItem.IsActivationReady && !IsState<EnemyAttack>() && !IsState<EnemyAvoid>())
            return true;

        return false;
    }

    public void OnTimesUp(int id, object args)
    {
        ///Starts the dash after the charge up
        _dashTimer.StartTimer();
    }
}
