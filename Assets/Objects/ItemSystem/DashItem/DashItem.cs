using System.Collections;
using System.Collections.Generic;
using Controllers;
using Health;
using UnityEngine;
using Enemy;
using CharacterController;
using Abilitys;

namespace ItemSystem
{
    public enum ItemState { player, enemy }
    public class DashItem : Item
    {
        EnemyDash _enemyDash;
        ItemState state;

        public override void OnEquipped()
        {
            base.OnEquipped();

            ActionsController ac = ItemHandler.Owner as ActionsController;
            
            if (ac)
            {
                ActivationAction = ac.App.C.PlayerActions.ProxyInputActions.Special1;
                ac.AbilityHandler.UnlockAbility(HandledAbility.Dash);
                (ac.AbilityHandler.GetAbility(HandledAbility.Dash) as Dash).Item = this;
            }
            else
            {
                var EnemyController = ItemHandler.Owner.GetComponent<EnemyController>();

                if (EnemyController)
                    _enemyDash = EnemyController.AiParent.GetComponent<EnemyDash>();

                if (_enemyDash)
                {
                    state = ItemState.enemy;
                    _enemyDash.DashItem = this;
                }
            }
        }

        public override void OnUnEquipped()
        {
            base.OnUnEquipped();

            switch (state)
            {
                case ItemState.player:
                    (ItemHandler.Owner as ActionsController).AbilityHandler.UnlockAbility(HandledAbility.Dash, false);
                    break;
                case ItemState.enemy:
                    _enemyDash.DashItem = null;
                    break;
                default:
                    break;
            }
        }
    }
}
