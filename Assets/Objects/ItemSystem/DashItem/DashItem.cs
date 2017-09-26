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
    }
}
