using Archon.SwissArmyLib.Automata;
using Archon.SwissArmyLib.Utils.Editor;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose: Base class for all enemy states.
    /// Creator: MP
    /// </summary>
    public abstract class EnemyState : MonoBehaviour, IFsmState<EnemyApplication>
    {
#pragma warning disable 414
        [SerializeField, ReadOnly] private bool _isEnabled;
#pragma warning restore 414
        
        public FiniteStateMachine<EnemyApplication> Machine { get; set; }
        public EnemyApplication Context { get; set; }

        public bool IsActive
        {
            get { return Machine.CurrentState as EnemyState == this; }
        }

        public virtual void Begin()
        {
            _isEnabled = true;
        }

        public virtual void Reason()
        {
        }

        public virtual void Act(float deltaTime)
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