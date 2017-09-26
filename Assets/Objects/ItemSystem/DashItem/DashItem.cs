using System.Collections;
using System.Collections.Generic;
using Controllers;
using Health;
using UnityEngine;
using Enemy;

namespace ItemSystem
{
    public class DashItem : Item
    {
        public override void Hit(HealthController victim)
        {
            victim.Damage(10);
        }

        public override void OnEquipped()
        {
            //Check if picked up by enemy or player, if player enable player dash, otherwise
            //do dash code yourself.
        }
    }
}
