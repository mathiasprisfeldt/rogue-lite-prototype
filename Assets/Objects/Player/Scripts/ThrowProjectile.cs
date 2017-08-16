﻿using CharacterController;
using UnityEngine;

namespace Special
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class ThrowProjectile : MonoBehaviour
    {
        [SerializeField]
        private GameObject _projectilePrefab;

        [SerializeField]
        private float _throwForce;

        [SerializeField]
        private float _cooldown;

        [SerializeField]
        private ActionsController _actionsController;

        private bool _active;
        private float _cooldownTimer;
        private bool _projectileSpawned;

        public bool Active
        {
            get
            {
                if (_actionsController.App.C.PlayerActions.ProxyInputActions.Special.WasPressed
                    && _cooldownTimer <= 0)
                {
                    _projectileSpawned = false;
                    _active = true;
                    _cooldownTimer = _cooldown;
                    _actionsController.StartThrow = true;
                }

                return _active;
            }
        }

        public void Throw()
        {
            GameObject go = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rig = go.GetComponent<Rigidbody2D>();
            if (rig != null)
                rig.AddForce(new Vector2(_actionsController.LastHorizontalDirection, 0) * _throwForce, ForceMode2D.Impulse);
            _projectileSpawned = true;
        }

        public void Update()
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= Time.deltaTime;
        }

        public void ResetThrow()
        {
            _active = false;
            _actionsController.Combat = false;
        }
    }
}