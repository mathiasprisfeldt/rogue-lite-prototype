using UnityEngine;
using Archon.SwissArmyLib.Automata;

namespace Enemy
{
    /// <summary>
    /// Purpose: Tries to keep a certain distance from the target.
    /// Creator: MP
    /// </summary>
    public class EnemyAvoid : EnemyState 
    {
        [SerializeField]
        private bool _waitForPlayer;

        [SerializeField]
        private float _avoidDistance = 2;

        void Start()
        {
            //Only here so you can disable this component in inspector.
        }

        void FixedUpdate()
        {
            if (IsActive)
            {
                float dir = 0;

                if (Context.C.ToPlayer.magnitude < _avoidDistance)
                    dir = -Mathf.Round(Context.C.ToPlayer.normalized.x);

                Context.C.Move(dir * Vector2.right);
            }
        }

        public override bool ShouldTakeover()
        {
            if (Context.M.Target && 
                Context.M.Character.BumpingDirection == 0 && 
                Context.C.ToPlayer.magnitude < _avoidDistance)
                return true;

            return false;
        }

        public override void Reason()
        {
            if (!Context.M.Target)
            {
                ChangeState<EnemyIdle>();
                return;
            }

            if (Context.M.Character.BumpingDirection != 0 || Context.C.ToPlayer.magnitude > _avoidDistance && !_waitForPlayer)
            {
                if (Machine.PreviousState is EnemyPatrol && Context.M.CanBackPaddle)
                    Context.C.Turn();

                ChangeState<EnemyIdle>();
                return;
            }

            base.Reason();
        }
    }
}