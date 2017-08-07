namespace AcrylecSkeleton.ModificationSystem.Examples
{
    public class StatModification : Modification
    {

        protected StatsExample _stats;

        public StatModification(float time, StatsExample stats) : base(time)
        {
            _stats = stats;
        }

        public StatModification(float time, string name, StatsExample stats) : base(time, name)
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