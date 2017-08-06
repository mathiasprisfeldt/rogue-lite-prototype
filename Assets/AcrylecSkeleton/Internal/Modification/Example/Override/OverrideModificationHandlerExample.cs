namespace AcrylecSkeleton.ModificationSystem.Examples
{
    public class OverrideModificationHandlerExample : ModificationHandler
    {
        private DataStorageExample _dataStorageExample;

        public void Awake()
        {
            _dataStorageExample = new DataStorageExample();
        }

        public override void ModificationAdded(Modification modification)
        {
            if (modification is OverrideModificationExample)
                ModificationAdded((OverrideModificationExample)modification);
        }

        private void ModificationAdded(OverrideModificationExample overrideModificationExample)
        {
            overrideModificationExample.DataStorageExample = _dataStorageExample;
        }
    }

    public class DataStorageExample
    {
        public float DataExample { get; set; }
    }
}