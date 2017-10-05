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
                ActivationAction = ac.App.C.PlayerActions.ProxyInputActions.Special2;
                (ItemHandler.Owner as ActionsController).AbilityHandler.UnlockAbility(
                       _throwType == ThrowType.Throw ?
                        HandledAbility.Throw :
                        HandledAbility.Grenade);

                if (_throwType == ThrowType.Throw)
                    (ac.AbilityHandler.GetAbility(HandledAbility.Throw) as ThrowProjectile).Item = this;
                else
                    (ac.AbilityHandler.GetAbility(HandledAbility.Grenade) as ThrowGrenade).Item = this;
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
                    (ItemHandler.Owner as ActionsController).AbilityHandler.UnlockAbility(
                    _throwType == ThrowType.Throw ?
                     HandledAbility.Throw :
                     HandledAbility.Grenade, false);
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