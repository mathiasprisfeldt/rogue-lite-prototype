using CharacterController;
using Enemy;
using ItemSystem;
using Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            if (ItemHandler.Owner is ActionsController)
            {
                (ItemHandler.Owner as ActionsController).AbilityHandler.UnlockAbility(
                    _throwType == ThrowType.Throw ?
                     Abilitys.HandledAbility.Throw :
                     Abilitys.HandledAbility.Grenade);
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
                     Abilitys.HandledAbility.Throw :
                     Abilitys.HandledAbility.Grenade);
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