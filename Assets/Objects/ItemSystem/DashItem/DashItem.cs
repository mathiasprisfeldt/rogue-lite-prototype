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
        ItemState _state;

        public override void OnEquipped()
        {
            base.OnEquipped();

            ActionsController ac = ItemHandler.Owner as ActionsController;

            if (ac)
            {
                _state = ItemState.player;
                ac.AbilityHandler.UnlockAbility(HandledAbility.Dash);
                (ac.AbilityHandler.GetAbility(HandledAbility.Dash) as Dash).Items.Add(this);

                CooldownTimer.ResetTimer();
            }
            else
            {
                var EnemyController = ItemHandler.Owner.GetComponent<EnemyController>();

                if (EnemyController)
                    _enemyDash = EnemyController.AiParent.GetComponent<EnemyDash>();

                if (_enemyDash)
                {
                    _state = ItemState.enemy;
                    _enemyDash.DashItem = this;
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
                    ac.AbilityHandler.UnlockAbility(HandledAbility.Dash, false);
                    (ac.AbilityHandler.GetAbility(HandledAbility.Dash) as Dash).Items.Remove(this);
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
