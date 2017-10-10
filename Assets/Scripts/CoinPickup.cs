using Archon.SwissArmyLib.Events;
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

        public override void Apply(GameObject go)
        {
            base.Apply(go);
            if (GameManager.Instance)
            {
                GameManager.Instance.Gold += goldValue;
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        { 
            Check(collision.gameObject);
        }


    }
}