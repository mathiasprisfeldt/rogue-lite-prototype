using System.Reflection.Emit;
using AcrylecSkeleton.Utilities;
using Archon.SwissArmyLib.Automata;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose: Base class for all enemy states.
    /// Creator: MP
    /// </summary>
    public abstract class EnemyState : MonoBehaviour, IFsmState<EnemyApplication>
    {
        [SerializeField] private bool _isEnabled;

        public FiniteStateMachine<EnemyApplication> Machine { get; set; }
        public EnemyApplication Context { get; set; }

        public bool IsActive
        {
            get { return Machine.CurrentState == this; }
        }

        public virtual void Begin()
        {
            _isEnabled = true;
        }

        public virtual void Reason()
        {
        }

        public virtual void Think(float deltaTime)
        {
        }

        public virtual void End()
        {
            _isEnabled = false;
        }

        /// <summary>
        /// Should the StateMachine change to this state?
        /// </summary>
        /// <returns></returns>
        public abstract bool ShouldTakeover();

        /// <summary>
        /// Used to change state only if the state is registered.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public bool ChangeState<T>() where T : EnemyState
        {
            if (Machine.IsStateRegistered<T>())
            {
                Machine.ChangeState<T>();
                return true;
            }

            return false;
        }

        public bool IsState<T>()
        {
            return Machine.CurrentState is T;
        }
    }
}