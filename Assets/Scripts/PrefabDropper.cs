using System.Collections.Generic;
using UnityEngine;

namespace PrefabDropper
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class PrefabDropper : MonoBehaviour 
    {
        [SerializeField]
        private List<GameObject> _prefabs = new List<GameObject>();

        [SerializeField,Range(0,360)]
        private float _dropAngle;

        [SerializeField]
        private float _minForce;

        [SerializeField]
        private float _maxForce;

        [SerializeField]
        private bool _try;

        [SerializeField,Range(0,1)]
        private float _changeToDrop;

        public void Update()
        {
            if (_try)
            {
                _try = false;
                Drop();
            }
        }

        public void Drop()
        {
            GameObject prefab = _prefabs[Random.Range(0,_prefabs.Count)];
            GameObject instantiatedGameObject = Instantiate(prefab, transform.position, Quaternion.identity);

            Rigidbody2D rigigbody = instantiatedGameObject.GetComponent<Rigidbody2D>();
            if (rigigbody)
            {
                var angle = Random.Range(0, _dropAngle) - _dropAngle/2;
                Vector2 direction = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
                var force = Random.Range(_minForce, _maxForce);
                rigigbody.AddForce(direction * force,ForceMode2D.Impulse);
            }
        }
    }
}