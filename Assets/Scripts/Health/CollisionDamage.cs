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
        private LayerMask _whitelist;

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
                if (_whitelist.Contains(sidesTargetCollider.gameObject.layer))
                {
                    Character targetCharacter = sidesTargetCollider.gameObject
                        .GetComponent<CollisionCheck>()
                        .Character;

                    if (targetCharacter.HealthController.IsDead)
                        continue;

                    _healthController.Character.HealthController.Damage(targetCharacter.Damage,
                        from: sidesTargetCollider.transform);
                }
            }
        }
    }
}