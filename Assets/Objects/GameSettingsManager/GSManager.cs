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

        [SerializeField]
        private int _corpsesSortingLayerId = 2;

        public float HealthContainerSize
        {
            get { return _healthContainerSize; }
            set { _healthContainerSize = value; }
        }

        public int CorpsesSortingLayerID
        {
            get { return _corpsesSortingLayerId; }
            set { _corpsesSortingLayerId = value; }
        }
    }
}