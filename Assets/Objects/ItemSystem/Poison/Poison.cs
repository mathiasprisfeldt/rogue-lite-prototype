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

        private PoisonTrail _trail;

        public float OnHitDamage { get { return _onHitDamage; } }

        public override void OnHit(HealthController victim)
        {
            base.OnHit(victim);
            victim.Character.ModificationHandler.AddModification(new PoisonModification(victim, this, "Poison"));
        }

        public override void OnEquipped()
        {
            base.OnEquipped();
            _trail = GetComponentInChildren<PoisonTrail>();
            _trail.Owner = this;
        }

        protected override void DoubleUp()
        {
            base.DoubleUp();

            _trail.StartEmmision();
        }

        protected override void DoubleDown()
        {
            base.DoubleDown();

            _trail.StopEmmision();
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