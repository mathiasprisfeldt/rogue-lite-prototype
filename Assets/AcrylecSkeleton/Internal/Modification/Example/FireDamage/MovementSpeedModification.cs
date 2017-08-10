using UnityEngine;

namespace AcrylecSkeleton.ModificationSystem.Examples
{
    public class MovementSpeedModification : StatModification {

        public MovementSpeedModification(float time, StatsExample stats) : base(time, stats)
        {
        }

        public MovementSpeedModification(float time, string name, StatsExample stats) : base(time, name, stats)
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