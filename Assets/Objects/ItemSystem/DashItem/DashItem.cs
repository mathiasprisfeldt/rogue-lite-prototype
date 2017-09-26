using System.Collections;
using System.Collections.Generic;
using Controllers;
using Health;
using UnityEngine;
using Enemy;

namespace ItemSystem
{
    public class DashItem : Active
    {
        public override void OnHit(HealthController victim)
        {
            victim.Damage(10);
        }

        public void Setup()
        {
            //Check if picked up by enemy or player, if player enable player dash, otherwise
            //do dash code yourself.
        }

        public override void OnTrigger()
        {

        }

        public void Update()
        {

        }
    }
}
