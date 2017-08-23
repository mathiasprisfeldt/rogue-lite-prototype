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
        private float avoidDistance = 2;

        void Start()
        {
            //Only here so you can disable this component in inspector.
        }

        void FixedUpdate()
        {
            if (IsActive)
                Context.C.SetVelocity(-Mathf.Round(Context.C.ToPlayer.normalized.x) * Vector2.right * Time.fixedDeltaTime);
        }

        public override bool ShouldChange()
        {
            if (Context.M.Character.BumpingDirection == 0 && Context.C.ToPlayer.magnitude < avoidDistance)
                return true;

            return false;
        }

        public override void Reason()
        {
            if (Context.M.Character.BumpingDirection != 0 || Context.C.ToPlayer.magnitude > avoidDistance)
            {
                if (Machine.PreviousState is EnemyPatrol && Context.M.CanBackPaddle)
                    Context.C.Turn();

                ChangeState<EnemyIdle>();
            }

            base.Reason();
        }
    }
}