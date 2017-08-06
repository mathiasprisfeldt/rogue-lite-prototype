namespace AcrylecSkeleton.ModificationSystem.Examples
{
    /// <summary>
    /// This example shows if you want the modication to get a reference to something when added.
    /// </summary>
    public class OverrideModificationExample : Modification
    {

        public DataStorageExample DataStorageExample { get; set; }

        public OverrideModificationExample(float time, float value, ModificationTypeEnum modificationType) : base(time, value, modificationType)
        {
        }

        public OverrideModificationExample(float time, float value, ModificationTypeEnum modificationType, string name) : base(time, value, modificationType, name)
        {
        }

        public override void ApplyModificaiton()
        {
            if (DataStorageExample != null)
                DataStorageExample.DataExample += Value;
        }

        public void ApplyModificaiton(DataStorageExample dataStorageExample)
        {
            DataStorageExample = dataStorageExample;
            ApplyModificaiton();
        }

        public override void RemoveModificaiton()
        {
            if (DataStorageExample != null)
                DataStorageExample.DataExample -= Value;
        }

        public override void UpdateModificaiton()
        {
        
        }

        public override void FixedUpdateModificaiton()
        {
        
        }
    }
}