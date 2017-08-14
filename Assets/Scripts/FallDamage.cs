using System;
using CharacterController;
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
    public class FallDamage : MonoBehaviour
    {
        private ActionController _actionController;
        private float _savedFlyingDistance; //Distance between last position and current to check for excetion.
        private bool _didCollideLastFrame; //Did we collide with the floor last frame?
        private float _lastYPosition;
        private float _savedYVelocity;

        [Header("Settings:")]
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

        [Header("References: REQUIRED"), SerializeField]
        private CharacterController.CharacterController _characterController;

        [SerializeField]
        private HealthController _healthControllerComp;

        void Start()
        {
            _lastYPosition = _characterController.Rigidbody.position.y;
            _actionController = _characterController as ActionController;
        }

        void Update()
        {
            bool isColliding = _characterController.OnGround;

            if (!_didCollideLastFrame && isColliding)
            {
                float dmgAmount = 0;

                switch (_damageType)
                {
                    case FallDamageType.LastPosition:
                        float yDist = Math.Abs(_lastYPosition - _characterController.Rigidbody.position.y);

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

                _healthControllerComp.Damage(Mathf.Clamp(dmgAmount, 0, _maxDmg));
            }

            //If controller is wall sliding save position when sliding.s
            bool isWallSliding = _actionController && (_actionController.WallSlide && _actionController.WallSlide.VerticalActive);

            if (isColliding || isWallSliding)
                _lastYPosition = _characterController.Rigidbody.position.y;
            else
            {
                //If the distance between current position and old position is increased, update last position.
                float currDist = Math.Abs(_lastYPosition - _characterController.Rigidbody.position.y);

                if (_savedFlyingDistance > currDist)
                {
                    _lastYPosition = _characterController.Rigidbody.position.y;
                }

                _savedFlyingDistance = currDist;
            }
                
            _didCollideLastFrame = isColliding;
            _savedYVelocity = Mathf.Abs(_characterController.Rigidbody.velocity.y);
        }
    }
}