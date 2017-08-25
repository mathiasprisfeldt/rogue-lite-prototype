using Enemy;
using Managers;
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
            GameObject go = Instantiate(_enemy, transform.position + new Vector3(_offSet.x, _offSet.y), Quaternion.identity);
            EnemyApplication ea = go.GetComponent<EnemyApplication>();
            if(ea != null && GameManager.Instance != null)
                GameManager.Instance.Enemies.Add(ea);
        }
    }
}