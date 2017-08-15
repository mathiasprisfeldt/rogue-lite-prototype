﻿using AcrylecSkeleton.Utilities;
using Controllers;
using UnityEngine;

namespace Health
{
    /// <summary>
    /// Purpose: Handles damage when colliding with something.
    /// Creator: MP
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HealthController))]
    public class CollisionDamage : MonoBehaviour
    {
        private HealthController _healthController;

        [SerializeField]
        private LayerMask _blacklist;

        void Awake()
        {
            _healthController = GetComponent<HealthController>();
        }

        void Update()
        {
            if (_healthController.Character.HealthController.IsInvurnable)
                return;

            foreach (Collider2D sidesTargetCollider in _healthController.Character.Hitbox.Sides.TargetColliders)
            {
                if (!_blacklist.Contains(sidesTargetCollider.gameObject.layer))
                {
                    _healthController.Character.HealthController.Damage(sidesTargetCollider.gameObject.GetComponent<CollisionCheck>().Character.Damage);
                }
            }
        }
    }
}