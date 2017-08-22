using Enemy;
using Managers;
using UnityEngine;

namespace EnemyMelee
{
    /// <summary>
    /// Purpose: Base class for any melee attack states for enemies.
    /// Creator: MP
    /// </summary>
    public class EnemyMelee : EnemyAttack 
    {
        public override void Attack()
        {
            if (CheckHitbox())
                GameManager.Instance.Player.M.ActionController.HealthController.Damage(App.M.Character.Damage, from: App.M.Character.Rigidbody.transform);

            base.Attack();
        }
    }
}