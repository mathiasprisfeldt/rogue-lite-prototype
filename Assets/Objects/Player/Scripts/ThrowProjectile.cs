using Archon.SwissArmyLib.Utils;
using CharacterController;
using ItemSystem;
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
        private ActionsController _actionsController;

        [SerializeField]
        private int _manaCost;

        private bool _throwActive;

        public ShootItem Item { get; set; }

        public bool KnifeActive
        {
            get
            {
                if (!Active)
                    return Active;
                var throwOnWAll = _actionsController.TriggerCheck.Sides.Left && _actionsController.LastHorizontalDirection < 0
                    || _actionsController.TriggerCheck.Sides.Right && _actionsController.LastHorizontalDirection > 0;

                if (Item && Item.ActivationAction != null && Item.ActivationAction.WasPressed
                    && !Item.CooldownTimer.IsRunning && _actionsController.LastUsedCombatAbility == CombatAbility.None
                    && !throwOnWAll)
                {
                    _throwActive = true;
                    Item.CooldownTimer.StartTimer();
                    _actionsController.StartThrow.Value = true;
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

        public void ResetThrow()
        {
            _throwActive = false;
            _actionsController.Combat = false;
        }
    }
}