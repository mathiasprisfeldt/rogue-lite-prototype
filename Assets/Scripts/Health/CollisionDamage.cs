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

                    _character.HealthController.Damage(GetComponentUp<Character>(sidesTargetCollider.gameObject).Damage);
                }
            }
        }

        public T GetComponentUp<T>(GameObject go) where T : MonoBehaviour
        {
            Transform parent = go.transform;
            T foundComp = null;

            while (parent)
            {
                foundComp = parent.GetComponentInChildren<T>();

                if (!foundComp)
                    parent = parent.parent;
                else
                    break;
            }

            return foundComp;
        }
    }
}