using Managers;
using Pickups;
using UnityEngine;

namespace Pickups
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class CoinPickup : Pickup
    {
        [SerializeField]
        private int goldValue;

        private bool _used;

        public override void Apply(GameObject go)
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.Gold += goldValue;
                _used = true;
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(_used)
                return;
            Check(collision.gameObject);
        }


    }
}