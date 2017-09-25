using Archon.SwissArmyLib.Utils;
using CharacterController;
using Projectiles;
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
        private Projectile _projectilePrefab;

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

        public bool KnifeActive
        {
            get {
                if (!Active)
                    return Active;
                var throwOnWAll = _actionsController.TriggerCheck.Sides.Left && _actionsController.LastHorizontalDirection < 0 
                    || _actionsController.TriggerCheck.Sides.Right && _actionsController.LastHorizontalDirection > 0;

                if (_actionsController.App.C.PlayerActions != null && _actionsController.App.C.PlayerActions.ProxyInputActions.Special.WasPressed
                    && _cooldownTimer <= 0 && _actionsController.LastUsedCombatAbility == CombatAbility.None && _actionsController.ManaHandler.Mana >= _manaCost 
                    && !throwOnWAll)
                {
                    _actionsController.ManaHandler.Mana -= _manaCost;
                    _throwActive = true;
                    _cooldownTimer = _cooldown;
                    _actionsController.StartThrow.Value = true;
                    if (_actionsController.Horizontal != 0)
                        _actionsController.Flip(_actionsController.Horizontal);
                }

                return _throwActive;
            }
        }

        public void Throw()
        {
            Projectile go = Instantiate(_projectilePrefab, _actionsController.ThrowPoint.position, Quaternion.identity);
            go.Owner = _actionsController;

            Rigidbody2D rig = go.GetComponent<Rigidbody2D>();
            if (rig != null)
                rig.AddForce(new Vector2(_actionsController.LastHorizontalDirection, 0) * _throwForce, ForceMode2D.Impulse);
        }

        public void Update()
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= BetterTime.DeltaTime;
        }

        public void ResetThrow()
        {
            _throwActive = false;
            _actionsController.Combat = false;
        }
    }
}