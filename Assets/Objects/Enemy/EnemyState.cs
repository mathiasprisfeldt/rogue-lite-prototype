using Assets.Enemy;
using UnityEngine;

namespace Assets.Enemy
{
    /// <summary>
    /// Purpose: Base class for all enemy states.
    /// Creator: MP
    /// </summary>
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

        /// <summary>
        /// Method used to check if the state should take over.
        /// </summary>
        /// <returns>If it should continue to check, in overrides this means it should take over.</returns>
        public abstract bool CheckPrerequisite();
    }
}