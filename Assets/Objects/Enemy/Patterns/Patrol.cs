using CharacterController;
using UnityEngine;

namespace Assets.Objects.Enemy
{
    /// <summary>
    /// Purpose: Patrol pattern for all enemies patrolling an area
    /// Creator: MP
    /// </summary>
    public class Patrol : EnemyPattern
    {
        private Vector2 _patrolDirection = Vector2.right;

        [SerializeField]
        private Vector2 _startDirection = Vector2.right;

        void Awake()
        {
            _patrolDirection = _startDirection;
        }

        void Update()
        {
            int bumpingDirection = _enemyApplication.M.Character.BumpingDirection;

            //If we're bumping into something, change direction.
            if (bumpingDirection != 0)
                _patrolDirection.x = -_enemyApplication.M.Character.BumpingDirection;

            //If we're patrolling, move the enemy.
            if (_enemyApplication.CurrentState == EnemyState.Patrol)
                _enemyApplication.M.Character.SetVelocity(_patrolDirection);
        }
    }
}