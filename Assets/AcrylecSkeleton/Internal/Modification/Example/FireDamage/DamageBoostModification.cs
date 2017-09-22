using UnityEngine;
using Random = UnityEngine.Random;

namespace AcrylecSkeleton.ModificationSystem.Examples
{
    public class DamageBoostModification : StatModification
    {

        private float _timer;

        public DamageBoostModification(float time, StatsExample stats) : base(time, stats)
        {
        }

        public DamageBoostModification(float time, string name, StatsExample stats) : base(time, name, stats)
        {
        }

        public override void ApplyModificaiton()
        {
            _timer = 0;
            var previousDamage = _stats.Damage;
            _stats.Damage += Value;
            Debug.Log("Damage boost was added, previous damage was " + previousDamage + " new damage is " + _stats.Damage);
        }

        public override void UpdateModificaiton()
        {
            _timer += UnityEngine.Time.deltaTime;
            if (_timer > 0)
            {
                var randomValue = Random.Range(0, 350);
                _timer = 0;
                if (randomValue == 0)
                {
                    _stats.Damage -= Value;
                    Debug.Log("The damage boost was randomly removed, current damage is " + _stats.Damage);
                    ModificationHandler.RemoveModification(this);
                }
            }
        }
    }
}