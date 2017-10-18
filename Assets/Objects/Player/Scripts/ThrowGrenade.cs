using Archon.SwissArmyLib.Utils;
using CharacterController;
using ItemSystem;
using Projectiles;
using System.Collections.Generic;
using System.Linq;
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
        private ActionsController _actionsController;

        [SerializeField]
        private int _manaCost;

        private bool _throwActive;

        private List<ShootItem> items = new List<ShootItem>();

        public List<ShootItem> Items { get { return items; } }
        public ShootItem CurrentItem { get; set; }

        public bool KnifeActive
        {
            get
            {
                if (!Active)
                    return Active;
                var throwOnWAll = _actionsController.WallSlideCheck.Sides.Left && _actionsController.LastHorizontalDirection < 0
                    || _actionsController.WallSlideCheck.Sides.Right && _actionsController.LastHorizontalDirection > 0;

                bool input = false;

                if (Items.Any())
                {
                    ShootItem tempItem = Items.FirstOrDefault(x => x.ActivationAction != null && x.ActivationAction.WasPressed && !x.CooldownTimer.IsRunning);
                    if (tempItem)
                    {
                        input = true;
                        CurrentItem = tempItem;
                    }
                }

                if (input && _actionsController.LastUsedCombatAbility == CombatAbility.None
                    && !throwOnWAll)
                {
                    _throwActive = true;
                    CurrentItem.CooldownTimer.StartTimer();
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

        public void ResetGrenade()
        {
            _throwActive = false;
            _actionsController.Combat = false;
        }
    }
}