using UnityEngine;

namespace AcrylecSkeleton.ModificationSystem.Examples
{
    public class FireDamageModification : StatModification
    {

        private float _timer;

        public FireDamageModification(float time, float value, ModificationTypeEnum modificationType, StatsExample stats) : base(time, value, modificationType, stats)
        {
        }

        public FireDamageModification(float time, float value, ModificationTypeEnum modificationType, string name, StatsExample stats) : base(time, value, modificationType, name, stats)
        {
        }

        public override void ApplyModificaiton()
        {
            base.ApplyModificaiton();
            _timer = 0;
        }

        public override void UpdateModificaiton()
        {
            _timer += UnityEngine.Time.deltaTime;
            if (_timer >= 1)
            {
                _timer = 0;
                var randonAdder = Random.Range(-3, 3);
                _stats.Health -= Value + randonAdder;
                Debug.Log("Burn Damage did " + (Value + randonAdder) + " damage and health is at " + _stats.Health);
            }
        }
    }
}