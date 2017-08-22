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

        public override void Think(float deltaTime)
        {
            Context.C.SetVelocity(new Vector2(-Mathf.Round(Context.C.ToPlayer.normalized.x), 0));
        }

        public override bool ShouldChange()
        {
            if (Context.M.Character.BumpingDirection == 0 && Context.C.ToPlayer.magnitude < avoidDistance)
                return true;

            return false;
        }

        public override void Reason()
        {
            if (!ShouldChange())
            {
                if (Machine.PreviousState is EnemyPatrol && Context.M.CanBackPaddle)
                    Context.C.Turn();

                ChangeState<EnemyIdle>();
            }

            base.Reason();
        }
    }
}