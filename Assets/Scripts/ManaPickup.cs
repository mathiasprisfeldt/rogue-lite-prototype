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


        public override void Apply(GameObject go)
        {
            base.Apply(go);
            CollisionCheck cc = go.GetComponent<CollisionCheck>();
            ManaHandler mh = null;
            if (cc != null)
                mh = cc.Character.ManaHandler;
            if (mh != null)
            {
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