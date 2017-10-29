using CharacterController;
using Enemy;
using ItemSystem;
using Projectiles;
using Special;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilitys;

namespace ItemSystem
{
    public class ShootItem : Item
    {
        private enum ThrowType { Throw, Grenade }

        private EnemyShoot _enemyShoot;
        private ItemState _state;

        [SerializeField]
        private ThrowType _throwType;

        [SerializeField]
        private Projectile _projectile;

        public override void OnEquipped()
        {
            base.OnEquipped();

            ActionsController ac = ItemHandler.Owner as ActionsController;

            if (ac)
            {
                _state = ItemState.Player;

                CooldownTimer.ResetTimer();

                (ItemHandler.Owner as ActionsController).AbilityHandler.UnlockAbility(
                       _throwType == ThrowType.Throw ?
                        HandledAbility.Throw :
                        HandledAbility.Grenade);

                if (_throwType == ThrowType.Throw)
                {
                    (ac.AbilityHandler.GetAbility(HandledAbility.Throw) as ThrowProjectile).Items.Add(this);
                }
                else
                {
                    (ac.AbilityHandler.GetAbility(HandledAbility.Grenade) as ThrowGrenade).Items.Add(this);
                }
            }
            else
            {
                EnemyController enemyController = ItemHandler.Owner.GetComponent<EnemyController>();

                if (enemyController)
                    _enemyShoot = enemyController.AiParent.GetComponent<EnemyShoot>();

                if (_enemyShoot)
                {
                    _state = ItemState.Enemy;
                    _enemyShoot.Projectile = _projectile;
                    _enemyShoot.ShootItem = this;
                }
            }
        }

        public override void OnUnEquipped()
        {
            base.OnUnEquipped();

            switch (_state)
            {
                case ItemState.Player:

                    var ac = ItemHandler.Owner as ActionsController;

                    if (!ac)
                        return;

                    var shoot = ac.AbilityHandler.GetAbility(HandledAbility.Throw) as ThrowProjectile;
                    var grenade = ac.AbilityHandler.GetAbility(HandledAbility.Grenade) as ThrowGrenade;

                    bool takeAbility = false;

                    if (_throwType == ThrowType.Throw && shoot)
                    {
                        if (shoot.Items.Count <= 1)
                            takeAbility = true;

                        shoot.Items.Remove(this);
                    }
                    else if (grenade)
                    {
                        if (grenade.Items.Count <= 1)
                            takeAbility = true;

                        grenade.Items.Remove(this);
                    }

                    if (takeAbility)
                        ac.AbilityHandler.UnlockAbility(_throwType == ThrowType.Throw ?
                            HandledAbility.Throw :
                            HandledAbility.Grenade, false);
                    break;
                case ItemState.Enemy:
                    _enemyShoot.ShootItem = null;
                    break;
                default:
                    break;
            }
        }
    }
}