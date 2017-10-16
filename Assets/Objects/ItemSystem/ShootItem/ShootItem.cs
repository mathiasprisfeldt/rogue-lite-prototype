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
                    _state = ItemState.enemy;
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
                case ItemState.player:

                    var ac =
                    (ItemHandler.Owner as ActionsController);
                    ac.AbilityHandler.UnlockAbility(_throwType == ThrowType.Throw ?
                     HandledAbility.Throw :
                     HandledAbility.Grenade, false);

                    if (_throwType == ThrowType.Throw)
                        (ac.AbilityHandler.GetAbility(HandledAbility.Throw) as ThrowProjectile).Items.Remove(this);
                    else
                        (ac.AbilityHandler.GetAbility(HandledAbility.Grenade) as ThrowGrenade).Items.Remove(this);
                    break;
                case ItemState.enemy:
                    _enemyShoot.ShootItem = null;
                    break;
                default:
                    break;
            }
        }
    }
}