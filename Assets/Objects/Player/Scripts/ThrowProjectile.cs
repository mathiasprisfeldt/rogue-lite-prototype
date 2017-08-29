using CharacterController;
using UnityEngine;

namespace Special
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class ThrowProjectile : Ability
    {
        [SerializeField]
        private GameObject _projectilePrefab;

        [SerializeField]
        private float _throwForce;

        [SerializeField]
        private float _cooldown;

        [SerializeField]
        private ActionsController _actionsController;

        [SerializeField]
        private int _manaCost;

        private bool _throwActive;
        private float _cooldownTimer;
        private bool _projectileSpawned;


        public bool KnifeActive
        {
            get {
                if (!Active)
                    return Active;
                if (_actionsController.App.C.PlayerActions != null && _actionsController.App.C.PlayerActions.ProxyInputActions.Special.WasPressed
                    && _cooldownTimer <= 0 && _actionsController.LastUsedCombatAbility == CombatAbility.None && _actionsController.ManaHandler.Mana >= _manaCost)
                {
                    _actionsController.ManaHandler.Mana -= _manaCost;
                    _projectileSpawned = false;
                    _throwActive = true;
                    _cooldownTimer = _cooldown;
                    _actionsController.StartThrow.Value = true;
                }

                return _throwActive;
            }
        }

        public void Throw()
        {
            GameObject go = Instantiate(_projectilePrefab, _actionsController.ThrowPoint.position, Quaternion.identity);
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
            _throwActive = false;
            _actionsController.Combat = false;
        }
    }
}