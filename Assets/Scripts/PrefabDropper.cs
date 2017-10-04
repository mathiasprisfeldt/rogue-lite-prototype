using System;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

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

        [Tooltip("This value is used to determine how big " +
                 "a chance the top object in the prefab list " +
                 "is to drop. Every time the values isn't eached " +
                 "randomly, then the object that will be spawned " +
                 "will go down in the list until the last object")]
        [SerializeField, Range(0, 1)]
        private float _raityValue;

        [SerializeField, Range(0, 360)]
        private float _dropAngle;

        private Vector2 _angleDirection;

        [SerializeField]
        private int _minDropAmount;

        [SerializeField]
        private int _maxDropAmount;

        [SerializeField]
        private float _minForce;

        [SerializeField]
        private float _maxForce;

        [SerializeField, Range(0, 1)]
        private float _changeToDrop;

        [SerializeField]
        private Character _character;

        [SerializeField]
        private bool _try;

        public void Awake()
        {
            if (_character && _character.HealthController)
                _character.HealthController.OnDead.AddListener(OnDeath);
        }

        public void Update()
        {
            if (_try)
            {
                Drop();
                _try = false;
            }
        }

        private void OnDeath()
        {
            Drop();
        }

        public void Drop()
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue > _changeToDrop)
                return;

            int amountOfDrops = Random.Range(_minDropAmount, _maxDropAmount);

            for (int i = 0; i < amountOfDrops; i++)
            {
                GameObject prefab = _prefabs[0];

                for (var j = 0; j < _prefabs.Count; j++)
                {
                    randomValue = Random.Range(0f, 1f);
                    if (j == _prefabs.Count - 1 || randomValue <= _raityValue)
                    {
                        prefab = _prefabs[j];
                        break;
                    }
                }

                GameObject instantiatedGameObject = Instantiate(prefab, transform.position, Quaternion.identity);

                Rigidbody2D rigigbody = instantiatedGameObject.GetComponent<Rigidbody2D>();
                if (rigigbody)
                {
                    var angle = Random.Range(0, _dropAngle) - _dropAngle / 2;
                    Vector2 direction = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
                    var force = Random.Range(_minForce, _maxForce);
                    rigigbody.AddForce(direction * force, ForceMode2D.Impulse);
                }
            }

            
        }
    }
}