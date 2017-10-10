using AcrylecSkeleton.ModificationSystem;
using AcrylecSkeleton.Utilities;
using Controllers;
using Health;
using UnityEngine;

namespace ItemSystem.Items
{
    /// <summary>
    /// Purpose:
    /// Creator: MP
    /// </summary>
    public class Poison : Item
    {
        [SerializeField]
        private float _onHitDamage = .5f;

        public float OnHitDamage { get { return _onHitDamage; } }

        public override void OnHit(HealthController victim)
        {
            base.OnHit(victim);
            victim.Character.ModificationHandler.AddModification(new PoisonModification(victim, this, "Poison"));
        }
    }

    public class PoisonModification : Modification
    {
        HealthController _victim;
        Poison _item;

        public PoisonModification(HealthController victim, Poison item, string name) : base(item.CooldownTimer.Clone(), name)
        {
            _victim = victim;
            _item = item;
        }

        public override void RemoveModificaiton()
        {
            Timer.Elapsed.Invoke();
            base.RemoveModificaiton();
        }

        protected override void OnTimerElapsed()
        {
            _victim.Damage(_item.OnHitDamage, triggerItemhandler: false);
        }
    }
}