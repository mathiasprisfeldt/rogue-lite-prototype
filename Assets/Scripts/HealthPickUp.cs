using System;
using Health;
using UnityEngine;

namespace Pickups
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class HealthPickUp : Pickup
    {
        private enum HealthPickupType
        {
            Heal, MaxHealth
        }
        
        [Header("Health Properties"), SerializeField]
        private float _healthAmount;

        [SerializeField]
        private HealthPickupType _type;

        public void OnTriggerStay2D(Collider2D collision)
        {
            Check(collision.gameObject);
        }

        public override void Apply(GameObject go)
        {
            base.Apply(go);
            CollisionCheck cc = go.GetComponent<CollisionCheck>();
            HealthController hc = null;
            if (cc != null)
                hc = cc.Character.HealthController;
            if (hc != null )
            {
                switch (_type)
                {
                    case HealthPickupType.Heal:
                        hc.Heal(_healthAmount);
                        break;
                    case HealthPickupType.MaxHealth:
                        hc.MaxHealth += _healthAmount;
                        hc.Heal(_healthAmount);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }   
            Destroy(gameObject);

        }

    }
}

        
