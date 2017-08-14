using UnityEngine;

namespace Assets.Objects.Enemy
{
    /// <summary>
    /// Purpose: Used as super class for all enemy patterns
    /// Creator: MP
    /// </summary>
    public class EnemyPattern : MonoBehaviour
    {
        [Header("References:"), SerializeField]
        protected EnemyApplication _enemyApplication;
    }
}