using System;
using CharacterController;
using Controllers;
using UnityEngine;

namespace Health
{
    enum FallDamageType
    {
        LastPosition,
        Magnitude
    }

    /// <summary>
    /// Purpose: Inflicts fall damage to a health system.
    /// Creator: Mathias Prisfeldt
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HealthController))]
    public class FallDamage : MonoBehaviour
    {
        private ActionsController _actionsController;
        private HealthController _healthController;
        private float _savedFlyingDistance; //Distance between last position and current to check for excetion.
        private bool _didCollideLastFrame; //Did we collide with the floor last frame?
        private Vector2 _lastPosition;
        private float _savedYVelocity;

        [Header("Settings:")]
        [SerializeField]
        private bool _drawGizmos;

        [SerializeField]
        private FallDamageType _damageType;

        [SerializeField]
        private float _distanceBeforeDmg = 1; //Amount of fall units before taking fall damage.

        [SerializeField]
        private bool _isDistanceBeforeDmgInclusive = false;

        [SerializeField]
        private float _fallSegments = 1; //Used with last position to split fall units up in segments.

        [SerializeField]
        private float _dmgPerFallUnit = 15; //How much dmg to inflict per fall unit or fall segments, chosen by damage type.

        [SerializeField]
        private float _maxDmg = Single.PositiveInfinity; //Max amount of fall damage it can take.

        void Awake()
        {
            _healthController = GetComponent<HealthController>();
        }

        void Start()
        {
            _lastPosition = _healthController.Character.Rigidbody.position;
        }

        void Update()
        {
            bool isColliding = _healthController.Character.OnGround;

            if (!_didCollideLastFrame && isColliding)
            {
                float dmgAmount = 0;

                switch (_damageType)
                {
                    case FallDamageType.LastPosition:
                        if (_healthController.Character.Rigidbody.position.y > _lastPosition.y)
                            break;

                        float yDist = Math.Abs(_lastPosition.y - _healthController.Character.Rigidbody.position.y);

                        if (yDist >= _distanceBeforeDmg)
                        {
                            if (!_isDistanceBeforeDmgInclusive)
                                yDist -= _distanceBeforeDmg;

                            int segmentAmount = Mathf.RoundToInt(yDist / _fallSegments);

                            dmgAmount = segmentAmount * _dmgPerFallUnit;
                        }
                        break;
                    case FallDamageType.Magnitude:
                        if (_savedYVelocity > _distanceBeforeDmg)
                            dmgAmount = _savedYVelocity * _dmgPerFallUnit;
                        break;
                }

                _healthController.Damage(Mathf.Clamp(dmgAmount, 0, _maxDmg));
            }

            //If controller is wall sliding save position when sliding.s
            bool isWallSliding = _actionsController && (_actionsController.AbilityReferences.WallSlide 
                && _actionsController.AbilityReferences.WallSlide.VerticalActive);

            if (isColliding || isWallSliding)
                _lastPosition = _healthController.Character.Rigidbody.position;
            else
            {
                //If the distance between current position and old position is increased, update last position.
                float currDist = Math.Abs(_lastPosition.y - _healthController.Character.Rigidbody.position.y);

                if (_savedFlyingDistance > currDist)
                {
                    _lastPosition = _healthController.Character.Rigidbody.position;
                }

                _savedFlyingDistance = currDist;
            }
                
            _didCollideLastFrame = isColliding;
            _savedYVelocity = Mathf.Abs(_healthController.Character.Rigidbody.velocity.y);
        }

        private void OnDrawGizmosSelected()
        {
            if (_drawGizmos)
                Gizmos.DrawSphere(_lastPosition, 0.5f);
        }
    }
}