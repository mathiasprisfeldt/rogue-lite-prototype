using System.Reflection.Emit;
using AcrylecSkeleton.Utilities;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose: Base class for all enemy states.
    /// Creator: MP
    /// </summary>
    [ExecuteInEditMode]
    public abstract class EnemyState : MonoBehaviour
    {
        [SerializeField]
        private bool _isActive;

        [SerializeField]
        private bool _isIsolated = true;

        [SerializeField]
        private EnemyApplication _app;

        /// <summary>
        /// Should the state run alone or parallel?
        /// </summary>
        public bool IsIsolated
        {
            get { return _isIsolated; }
            set { _isIsolated = value; }
        }

        public EnemyApplication App
        {
            get { return _app; }
            set { _app = value; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        protected virtual void Awake()
        {
            //Find application candidate, if it couldn't find one warn it to log.
            if (!App)
            {
                App = gameObject.GetComponentUp<EnemyApplication>();

                if (!App)
                    Debug.LogError("EnemyState: You forgot to drag the application reference into the field!", transform);
            }
        }

        /// <summary>
        /// Called when a state gets active.
        /// </summary>
        /// <returns></returns>
        public virtual void StateStart() { }

        /// <summary>
        /// Called when a state ends.
        /// </summary>
        public virtual void StateEnd() { }

        /// <summary>
        /// Update loop for logic when state is active.
        /// </summary>
        public virtual void StateUpdate() { }

        /// <summary>
        /// Method used to check if the state should take over.
        /// </summary>
        /// <returns>If it should continue to check, in overrides this means it should take over.</returns>
        public abstract bool CheckPrerequisite();
    }
}