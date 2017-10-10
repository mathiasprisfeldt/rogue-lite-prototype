using System.Collections.Generic;
using System.Linq;
using AcrylecSkeleton.Utilities;
using AcrylecSkeleton.Utilities.Collections;
using Archon.SwissArmyLib.Events;
using Archon.SwissArmyLib.Utils;
using UnityEngine;

namespace Pickups
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public abstract class Pickup : MonoBehaviour, TellMeWhen.ITimerCallback
    {
        [SerializeField]
        private bool _active;

        [SerializeField]
        private Tags _targetTags;

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

        [SerializeField]
        private float _timeUntilActive;

        private float _distanceTraveled;
        private Vector2 _currentDirection;
        private bool _used;

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
            if (!_targetTags.Contains(go) || _targetLayers.Contains(go.layer) ||
                !_active || _used)
                return;
            Apply(go);
        }



        public void Awake()
        {
            _currentDirection = _travelDirection.normalized;

            if (_timeUntilActive > 0)
            {
                TellMeWhen.Seconds(_timeUntilActive,this);
                _used = true;
            }
        }

        public void FixedUpdate()
        {
            if (_speed > 0)
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

        public void OnTimesUp(int id, object args)
        {
            _used = false;
        }

        public virtual void Apply(GameObject go)
        {
            _used = true;
        }
}
}