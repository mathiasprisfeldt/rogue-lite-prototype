using AcrylecSkeleton.Utilities;
using Archon.SwissArmyLib.Utils;
using Enemy;
using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Archon.SwissArmyLib.Events;
using System;
using System.Linq;
using Controllers;



public class EnemyDash : EnemyState, TellMeWhen.ITimerCallback
{
    private enum TellMeWhenReason
    {
        ChargeUp, Indicator
    }

    [SerializeField]
    private float _range;
    [SerializeField]
    private float _dashSpeed;
    [SerializeField]
    private float _ydashSpeed;
    [SerializeField]
    private Timer _dashTimer;
    [SerializeField]
    private AnimationCurve _dashCurve;
    [SerializeField]
    private AnimationCurve _ydashCurve;
    [SerializeField]
    private float _chargeUp;
    [SerializeField]
    private float _damage;

    private bool _isDashing;
    private Vector2 _dashDirection;
    private List<Character> _dirtyCols;

    public DashItem DashItem { get; set; }

    private void FixedUpdate()
    {
        if (_dashTimer.IsRunning)
        {

            float x = _dashCurve.Evaluate(
                (float)_dashTimer.Duration.TotalSeconds -
                Mathf.Abs((float)_dashTimer.Clock.TotalSeconds) /
                (float)_dashTimer.Duration.TotalSeconds) *
                (_dashSpeed / (float)_dashTimer.Duration.TotalSeconds) * _dashDirection.x;

            float y = _ydashCurve.Evaluate(
                (float)_dashTimer.Duration.TotalSeconds -
                Mathf.Abs((float)_dashTimer.Clock.TotalSeconds) /
                (float)_dashTimer.Duration.TotalSeconds) *
                (_ydashSpeed / (float)_dashTimer.Duration.TotalSeconds);

            Context.M.Character.SetVelocity(new Vector2(x, y * -1) * BetterTime.FixedDeltaTime);
        }
        else
        {
            Context.M.Character.StandStill();
        }
    }

    public override void Act(float deltaTime)
    {
        base.Act(deltaTime);

        var colls = Context.M.Character.Hitbox.Sides.TargetColliders.Where(x => x.tag.Equals("Player"));

        if (colls.Any())
        {
            Character character = colls.FirstOrDefault().GetComponent<CollisionCheck>().Character;

            if (!_dirtyCols.Contains(character))
            {
                character.HealthController.Damage(_damage, from: Context.M.Character, pos: transform.position);

                _dirtyCols.Add(character);
            }
        }
    }

    public override void Begin()
    {
        base.Begin();
        _dashDirection = Context.C.ToPlayer.normalized.x > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
        _isDashing = true;
        DashItem.Activate();
        TellMeWhen.Seconds(_chargeUp, this, (int)TellMeWhenReason.ChargeUp);
        _dirtyCols = new List<Character>();
        HandleAnimation(true);
    }

    public void OnDashEnded()
    {
        _isDashing = false;
        Context.M.AttackIndicator.HideIndicator(0.1f);
    }

    public void HandleAnimation(bool initialze)
    {
        if (!Context.M.Character.MainAnimator)
            return;
        if (initialze)
        {
            Context.M.Character.MainAnimator.SetTrigger("PreDash");
            Context.M.Character.MainAnimator.SetBool("Dash", true);
        }
        else
            Context.M.Character.MainAnimator.SetBool("Dash", false);
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

        switch (id)
        {
            case (int)TellMeWhenReason.ChargeUp:
                Context.M.AttackIndicator.ShowIndicator(0.05f);
                TellMeWhen.Seconds(0.1f, this, (int)TellMeWhenReason.Indicator);
                break;
            case (int)TellMeWhenReason.Indicator:
                HandleAnimation(false);
                Context.M.Character.MainAnimator.SetTrigger("Attack");
                _dashTimer.StartTimer();
                Context.M.AttackIndicator.HideIndicator(0.15f);
                break;
        }

        ///Starts the dash after the charge up

    }
}
