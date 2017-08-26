using AcrylecSkeleton.Utilities;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Purpose: Contains all settings about the game.
    /// Creator: Mathias Prisfeldt
    /// </summary>
    public class GSManager : Singleton<GSManager>
    {
        [SerializeField]
        private float _healthContainerSize = 1;

        public float HealthContainerSize
        {
            get { return _healthContainerSize; }
            set { _healthContainerSize = value; }
        }
    }
}