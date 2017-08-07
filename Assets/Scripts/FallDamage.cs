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
    [RequireComponent(typeof(Health))]
    public class FallDamage : MonoBehaviour
    {
        private Health _health;

        [SerializeField]
        private FallDamageType _damageType;

        [SerializeField]
        private Rigidbody2D _rigidbody2D;

        void Awake()
        {
            _health = GetComponent<Health>();
        }
    }
}