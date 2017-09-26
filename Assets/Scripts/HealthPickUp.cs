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
        [Header("Health Properties"), SerializeField]
        private float _healthAmount;

        private bool _used;

        public void OnTriggerStay2D(Collider2D collision)
        {
            Check(collision.gameObject);
        }

        public override void Apply(GameObject go)
        {
            CollisionCheck cc = go.GetComponent<CollisionCheck>();
            HealthController hc = null;
            if (cc != null)
                hc = cc.Character.HealthController;
            if (hc != null && !_used)
            {
                _used = true;
                hc.Heal(_healthAmount);
                Destroy(gameObject);
            }

        }
    }
}