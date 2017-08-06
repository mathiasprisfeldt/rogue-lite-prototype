namespace AcrylecSkeleton.ModificationSystem.Examples
{
    public class StatModification : Modification
    {

        protected StatsExample _stats;

        public StatModification(float time, float value, ModificationTypeEnum modificationType, StatsExample stats) : base(time, value, modificationType)
        {
            _stats = stats;
        }

        public StatModification(float time, float value, ModificationTypeEnum modificationType, string name, StatsExample stats) : base(time, value, modificationType, name)
        {
            _stats = stats;
        }

        public override void ApplyModificaiton()
        {
        
        }

        public override void RemoveModificaiton()
        {
        
        }

        public override void UpdateModificaiton()
        {
        
        }

        public override void FixedUpdateModificaiton()
        {
        
        }
    }
}