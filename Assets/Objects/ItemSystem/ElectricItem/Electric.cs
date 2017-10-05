using AcrylecSkeleton.ModificationSystem;
using AcrylecSkeleton.Utilities;
using Controllers;
using Enemy;
using Health;
using UnityEngine;

namespace ItemSystem.Items
{
    /// <summary>
    /// Purpose:
    /// Creator: MP
    /// </summary>
    public class Electric : Item
    {
        [SerializeField]
        private float _stunTime = .5f;

        public override void OnHit(HealthController victim)
        {
            base.OnHit(victim);
            victim.Character.GetComponent<EnemyController>().Stun(_stunTime);
        }
    }
}