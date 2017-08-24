using UnityEngine;

namespace EnemySpawn
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _offSet;

        [SerializeField]
        private GameObject _enemy;

        public void Spawn()
        {
            Instantiate(_enemy, transform.position + new Vector3(_offSet.x, _offSet.y), Quaternion.identity);
        }
    }
}