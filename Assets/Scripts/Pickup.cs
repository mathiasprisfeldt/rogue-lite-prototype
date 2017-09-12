using System.Collections.Generic;
using Archon.SwissArmyLib.Utils;
using UnityEngine;

namespace Pickups
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public abstract class Pickup : MonoBehaviour
    {
        [SerializeField]
        private bool _active;

        [SerializeField]
        private List<string> _targetTags = new List<string>();

        [SerializeField]
        private LayerMask _targetLayers;

        [SerializeField]
        private Rigidbody2D _rigigbody;


        [SerializeField]
        private Vector2 _travelDirection;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _distanceToSwitckDirection;

        private float _distanceTraveled;
        private Vector2 _currentDirection;


        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public List<string> TargetTags
        {
            get { return _targetTags; }
            set { _targetTags = value; }
        }

        public virtual void Check(GameObject go)
        {
            if (!(_targetTags.Count > 0 && _targetTags.Contains(go.tag)) || _targetLayers == (_targetLayers | (1 << gameObject.layer)) || !_active)
                return;
            Apply(go);
        }

        public abstract void Apply(GameObject go);


        public void Awake()
        {
            _currentDirection = _travelDirection.normalized;
        }

        public void FixedUpdate()
        {
            if (_distanceTraveled >= _distanceToSwitckDirection)
            {
                _distanceTraveled = 0;
                _currentDirection = (_currentDirection * -1f).normalized;
            }

            var currentSpeed = _speed * BetterTime.FixedDeltaTime;

            if ((_currentDirection * currentSpeed).magnitude + _distanceTraveled > _distanceToSwitckDirection)
                currentSpeed -= (_currentDirection * currentSpeed).magnitude + _distanceTraveled - _distanceToSwitckDirection;

            _distanceTraveled += currentSpeed;
            _rigigbody.velocity = _currentDirection * currentSpeed;
        }
    }
}