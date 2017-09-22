using Mana;
using Pickups;
using UnityEngine;

namespace ManaPickup
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class ManaPickup : Pickup
    {
        [SerializeField]
        private int _manaAmount;

        private bool _used;

        public override void Apply(GameObject go)
        {
            CollisionCheck cc = go.GetComponent<CollisionCheck>();
            ManaHandler mh = null;
            if (cc != null)
                mh = cc.Character.ManaHandler;
            if (mh != null && !_used)
            {
                _used = true;
                mh.Mana += _manaAmount;
                Destroy(gameObject);
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            Check(collision.gameObject);
        }


    }
}