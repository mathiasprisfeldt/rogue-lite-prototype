using System;
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
        private float _savedFlyingDistance; //Distance between last position and current to check for excetion.
        private bool _didCollideLastFrame; //Did we collide with the floor last frame?
        private float _lastYPosition;

        [Header("Settings:")]
        [SerializeField]
        private FallDamageType _damageType;

        [SerializeField]
        private float _distanceBeforeDmg = 1; //Amount of units before taking fall damage.

        [SerializeField]
        private bool _isDistanceBeforeDmgInclusive = false;

        [SerializeField]
        private float _fallSegments = 1; //Segments fall distance is splitted in.

        [SerializeField]
        private float _dmgPerSegment = 15; //How much dmg to inflict per segment.

        [SerializeField]
        private float _maxDmg = Single.PositiveInfinity; //Max amount of fall damage it can take.

        [Header("References: REQUIRED"), SerializeField]
        private CollisionCheck _collisionCheck;

        [SerializeField]
        private Health _healthComp;

        [SerializeField]
        private Rigidbody2D _rigidbody2D;

        void Start()
        {
            _lastYPosition = _rigidbody2D.position.y;
        }

        void Update()
        {
            bool isColliding = _collisionCheck.IsColliding();

            if (!_didCollideLastFrame && isColliding)
            {
                float dmgAmount = 0;

                switch (_damageType)
                {
                    case FallDamageType.LastPosition:
                        float yDist = Math.Abs(_lastYPosition - _rigidbody2D.position.y);

                        if (yDist >= _distanceBeforeDmg)
                        {
                            if (!_isDistanceBeforeDmgInclusive)
                                yDist -= _distanceBeforeDmg;

                            int segmentAmount = Mathf.RoundToInt(yDist / _fallSegments);

                            dmgAmount = segmentAmount * _dmgPerSegment;
                        }
                        break;
                    case FallDamageType.Magnitude:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _healthComp.Damage(Mathf.Clamp(dmgAmount, 0, _maxDmg));
            }

            if (isColliding)
                _lastYPosition = _rigidbody2D.position.y;
            else
            {
                //If the distance between current position and old position is increased, update last position.
                float currDist = Math.Abs(_lastYPosition - _rigidbody2D.position.y);

                if (_savedFlyingDistance > currDist)
                {
                    _lastYPosition = _rigidbody2D.position.y;
                }

                _savedFlyingDistance = currDist;
            }
                
            _didCollideLastFrame = isColliding;
        }
    }
}