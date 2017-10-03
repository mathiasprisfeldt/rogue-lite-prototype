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
    public class ThrowGrenade : Ability
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
            get
            {
                if (!Active)
                    return Active;
                var throwOnWAll = _actionsController.TriggerCheck.Sides.Left && _actionsController.LastHorizontalDirection < 0
                    || _actionsController.TriggerCheck.Sides.Right && _actionsController.LastHorizontalDirection > 0;

                if (_actionsController.App.C.PlayerActions != null && _actionsController.App.C.PlayerActions.ProxyInputActions.Special2.WasPressed
                    && _cooldownTimer <= 0 && _actionsController.LastUsedCombatAbility == CombatAbility.None
                    && !throwOnWAll)
                {
                    _throwActive = true;
                    _cooldownTimer = _cooldown;
                    _actionsController.StartGrenade.Value = true;
                    Throw();
                    if (_actionsController.Horizontal != 0)
                        _actionsController.Flip(_actionsController.Horizontal);
                }

                return _throwActive;
            }
        }

        public void Throw()
        {
            Projectile projectile = Instantiate(_projectilePrefab, _actionsController.ThrowPoint.position, Quaternion.identity);
            projectile.Owner = _actionsController;
            projectile.Direction = new Vector2(_actionsController.LastHorizontalDirection, 0);
            projectile.Shoot();


        }

        public void Update()
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= BetterTime.DeltaTime;
        }

        public void ResetGrenade()
        {
            _throwActive = false;
            _actionsController.Combat = false;
        }
    }
}