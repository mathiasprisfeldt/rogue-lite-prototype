namespace AcrylecSkeleton.ModificationSystem
{
    public abstract class Modification
    {
        public enum ModificationTypeEnum
        {
            Timed, Infinite
        }

        //A referce to it's modification handler, is set when the modification is added in the modification handler
        public ModificationHandler ModificationHandler { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The amount of time the modification should be active(in seconds), only valid if the modification type uses it</param>
        /// <param name="value">The amount of change the modification should do</param>
        /// <param name="modificationType">The modification type</param>
        protected Modification(float time)
        {
            Time = time;
            Name = "None";
            ModificationType = ModificationTypeEnum.Timed;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The amount of time the modification should be active(in seconds), only valid if the modification type uses it</param>
        /// <param name="value">The amount of change the modification should do</param>
        /// <param name="name">The name of the modification, is used to see if the modification is allready active(Please don't call the modification None)</param>
        /// /// <param name="modificationType">The modification type</param>
        protected Modification(float time, string name)
        {
            Time = time;
            Name = name;
            ModificationType = ModificationTypeEnum.Timed;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected Modification()
        {
            Time = 0;
            Name = "None";
            ModificationType = ModificationTypeEnum.Infinite;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the modification, is used to see if the modification is allready active(Please don't call the modification None)</param>
        protected Modification(string name)
        {
            Time = 0;
            Name = name;
            ModificationType = ModificationTypeEnum.Infinite;
        }

        //Propterties
        public string Name { get; set; }
        public float Time { get; set; }
        public float Value { get; set; }
        public ModificationTypeEnum ModificationType { get; set; }

        /// <summary>
        /// Call this when applien the modification
        /// </summary>
        public abstract void ApplyModificaiton();

        /// <summary>
        /// Call this when the modification should be removed
        /// </summary>
        public virtual void RemoveModificaiton()
        {
            ModificationHandler.RemoveModification(this);
        }

        /// <summary>
        /// Call this in a update loop.
        /// </summary>
        public abstract void UpdateModificaiton();

        /// <summary>
        /// Call this in a fixed update loop.
        /// </summary>
        public abstract void FixedUpdateModificaiton();
    }
}