using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Health;

public class Lifesteal : Item
{
    [SerializeField]
    private float _stealAmount;

    public override void OnHit(HealthController victim)
    {
        base.OnHit(victim);

        victim.Damage(_stealAmount, triggerItemhandler:false);
        ItemHandler.Owner.HealthController.Heal(_stealAmount);
    }
}
