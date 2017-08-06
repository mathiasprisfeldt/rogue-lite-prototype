using UnityEngine;

namespace AcrylecSkeleton.ModificationSystem.Examples
{
    public class MovementSpeedModification : StatModification {

        public MovementSpeedModification(float time, float value, ModificationTypeEnum modificationType, StatsExample stats) : base(time, value, modificationType, stats)
        {
        }

        public MovementSpeedModification(float time, float value, ModificationTypeEnum modificationType, string name, StatsExample stats) : base(time, value, modificationType, name, stats)
        {
        }

        public override void ApplyModificaiton()
        {
            var previousSpeed = _stats.Speed;
            _stats.Speed += Value;

            Debug.Log("Speed boost added, preveious speed was " + previousSpeed + " new speed is " + _stats.Speed);
        }

        public override void RemoveModificaiton()
        {
            var previousSpeed = _stats.Speed;
            _stats.Speed -= Value;

            Debug.Log("Speed boost removed, preveious speed was " + previousSpeed + " new speed is " + _stats.Speed);
        }
    }
}