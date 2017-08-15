using Assets.Enemy;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;

namespace EnemyMelee
{
    /// <summary>
    /// Purpose: Melee behaviour for enemies.
    /// Creator: MP
    /// </summary>
    public class EnemyMelee : EnemyPattern 
    {
        [SerializeField]
        private float _attackDistance;

        void Update()
        {
            PlayerApplication plyApplication = _enemyApplication.M.Target;

            if (plyApplication && Vector2.Distance(plyApplication.transform.position, _enemyApplication.transform.position) < _attackDistance)
            {
                Attack();
            }
        }

        private void Attack()
        {
        }
    }
}