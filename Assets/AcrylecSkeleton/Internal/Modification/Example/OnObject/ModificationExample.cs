using UnityEngine;

namespace AcrylecSkeleton.ModificationSystem.Examples
{
    /// <summary>
    /// A modification example.
    /// </summary>
    public class ModificationExample : Modification {
        public ModificationExample(float time, float value, ModificationTypeEnum modificationType) : base(time, value, modificationType)
        {
        }

        public ModificationExample(float time, float value, ModificationTypeEnum modificationType, string name) : base(time, value, modificationType, name)
        {
        }

        public override void ApplyModificaiton()
        {
            Debug.Log("I'm a modification and I'm being applied");
        }

        public override void RemoveModificaiton()
        {
            Debug.Log("I'm a modification and I'm being removed");
        }

        public override void UpdateModificaiton()
        {
            Debug.Log("This is my time left: " + Time);
        }

        public override void FixedUpdateModificaiton()
        {
        }
    }
}