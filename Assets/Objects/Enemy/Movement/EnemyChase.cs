using System.Runtime.InteropServices;
using AcrylecSkeleton.Extensions;
using AcrylecSkeleton.Utilities;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose: Chases the player.
    /// Creator: MP
    /// </summary>
    public class EnemyChase : EnemyState
    {
        [SerializeField]
        private float _targetLenght;

        void FixedUpdate()
        {
            if (!IsActive)
                return;

            if (Context.C.Target && Context.C.ToPlayer.magnitude > 1f)
            {
                if (Context.M.Character.OnGround && !Context.M.Character.IsFlying)
                {
                    float xDir = Mathf.Round(Context.C.ToPlayer.normalized.x);

                    var chase = _targetLenght == 0 ||
                                Vector2.Distance(transform.position.ToVector2(),
                                    Context.C.Target.transform.position.ToVector2()) >
                                _targetLenght;

                    if (!xDir.FastApproximately(Context.M.Character.BumpingDirection) && chase)
                        Context.C.Move(xDir * Vector2.right, forceTurn: true);
                    else
                        Context.M.Character.StandStill();
                }
                else if (Context.M.Character.IsFlying)
                    Context.C.Move(Context.C.ToPlayer.normalized, true, true);
            }
            else
                Context.M.Character.StandStill();
        }

        public override void Reason()
        {
            base.Reason();

            if (!Context.C.Target)
                ChangeState<EnemyIdle>();
        }

        public override bool ShouldTakeover()
        {
            if (Context.C.Target &&
                !IsState<EnemyAttack>() &&
                !IsState<EnemyAvoid>() &&
                !IsState<EnemyDash>())
                return true;

            return false;
        }
    }
}