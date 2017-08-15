using AcrylecSkeleton.Utilities;
using Controllers;
using UnityEngine;

namespace Health
{
    /// <summary>
    /// Purpose: Handles damage when colliding with something.
    /// Creator: MP
    /// </summary>
    public class CollisionDamage : MonoBehaviour 
    {
        [SerializeField]
        private LayerMask _blacklist;

        [SerializeField]
        private Character _character;

        void Update()
        {
            if (_character.HealthController.IsInvurnable)
                return;

            foreach (Collider2D sidesTargetCollider in _character.Hitbox.Sides.TargetColliders)
            {
                if (!_blacklist.Contains(sidesTargetCollider.gameObject.layer))
                {
                    _character.HealthController.Damage(sidesTargetCollider.gameObject.GetComponent<CollisionCheck>().Character.Damage);
                }
            }
        }
    }
}