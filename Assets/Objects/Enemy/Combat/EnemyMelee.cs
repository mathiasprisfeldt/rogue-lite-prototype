using Enemy;
using Managers;

namespace Enemy
{
    /// <summary>
    /// Purpose: Base class for any melee attack states for enemies.
    /// Creator: MP
    /// </summary>
    public class EnemyMelee : EnemyAttack 
    {
        public override void Attack()
        {
            if (IsActive && CheckHitbox())
                GameManager.Instance.Player.M.ActionController.HealthController.Damage(Context.M.Character.Damage, from: Context.M.Character);

            base.Attack();
        }
    }
}