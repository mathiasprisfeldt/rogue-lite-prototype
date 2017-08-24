using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class EnemyAttackAnim : MonoBehaviour
    {
        [SerializeField]
        private EnemyController _enemyController;

        public void Attacked()
        {
            if (_enemyController.IsState<EnemyAttack>())
                (_enemyController.StateMachine.CurrentState as EnemyAttack).Attack();
        }
    }
}