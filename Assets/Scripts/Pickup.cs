using System.Collections.Generic;
using System.Linq;
using AcrylecSkeleton.Utilities;
using AcrylecSkeleton.Utilities.Collections;
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

        [SerializeField] private Tags _targetTags;

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

        public Tags TargetTags
        {
            get { return _targetTags; }
            set { _targetTags = value; }
        }

        public virtual void Check(GameObject go)
        {
            if (!_targetTags.Contains(go) || _targetLayers.Contains(go.layer) || !_active)
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

            var currentSpeed = _speed * Time.fixedDeltaTime;

            if ((_currentDirection * currentSpeed).magnitude + _distanceTraveled > _distanceToSwitckDirection)
                currentSpeed -= (_currentDirection * currentSpeed).magnitude + _distanceTraveled - _distanceToSwitckDirection;

            _distanceTraveled += currentSpeed;
            _rigigbody.velocity = _currentDirection * currentSpeed;
        }
    }
}