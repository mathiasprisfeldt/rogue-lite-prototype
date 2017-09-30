using System.Collections;
using System.Collections.Generic;
using Controllers;
using Health;
using UnityEngine;
using Enemy;
using CharacterController;

namespace ItemSystem
{
    public class DashItem : Item
    {
        private enum DashState { player, enemy }

        EnemyDash _enemyDash;
        DashState state;

        public override void OnEquipped()
        {
            base.OnEquipped();

            if (ItemHandler.Owner is ActionsController)
            {
                (ItemHandler.Owner as ActionsController).AbilityHandler.UnlockAbility(Abilitys.HandledAbility.Dash);
            }
            else
            {
                var EnemyController = ItemHandler.Owner.GetComponent<EnemyController>();

                if (EnemyController)
                    _enemyDash = EnemyController.AiParent.GetComponent<EnemyDash>();

                if (_enemyDash)
                {
                    state = DashState.enemy;
                    _enemyDash.DashItem = this;
                }
            }
        }

        public override void OnUnEquipped()
        {
            base.OnUnEquipped();

            switch (state)
            {
                case DashState.player:
                    (ItemHandler.Owner as ActionsController).AbilityHandler.UnlockAbility(Abilitys.HandledAbility.Dash);
                    break;
                case DashState.enemy:
                    _enemyDash.DashItem = null;
                    break;
                default:
                    break;
            }
        }
    }
}
