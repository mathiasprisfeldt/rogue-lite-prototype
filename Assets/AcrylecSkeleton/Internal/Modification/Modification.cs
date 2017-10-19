using System;
using AcrylecSkeleton.Utilities;
using UnityEngine;
using Archon.SwissArmyLib.Utils.Editor;

namespace AcrylecSkeleton.ModificationSystem
{
    [Serializable]
    public class Modification
    {
        [SerializeField,ReadOnly]
        private string _name;

        [SerializeField, ReadOnly]
        private Timer _timer;

        public enum ModificationTypeEnum
        {
            Timed, Infinite
        }

        //A referce to it's modification handler, is set when the modification is added in the modification handler
        public ModificationHandler ModificationHandler { get; set; }

        //Propterties
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Timer Timer
        {
            get { return _timer; }
            set { _timer = value; }
        }

        public float Value { get; set; }
        public ModificationTypeEnum ModificationType { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The amount of time the modification should be active(in seconds), only valid if the modification type uses it</param>
        /// <param name="value">The amount of change the modification should do</param>
        /// <param name="modificationType">The modification type</param>
        protected Modification(Timer timer)
        {
            Timer = timer;
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
        protected Modification(Timer timer, string name)
        {
            Timer = timer;
            Name = name;
            ModificationType = ModificationTypeEnum.Timed;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected Modification()
        {
            Timer = new Timer(0);
            Name = "None";
            ModificationType = ModificationTypeEnum.Infinite;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the modification, is used to see if the modification is allready active(Please don't call the modification None)</param>
        protected Modification(string name)
        {
            Timer = new Timer(0);
            Name = name;
            ModificationType = ModificationTypeEnum.Infinite;
        }

        private void SetupTimers()
        {
            Timer.Elapsed.AddListener(OnTimerElapsed);
            Timer.Finished.AddListener(OnTimerFinished);
            Timer.StartTimer();
        }

        protected virtual void OnTimerElapsed() { }

        protected virtual void OnTimerFinished() { }

        /// <summary>
        /// Call this when applien the modification
        /// </summary>
        public virtual void ApplyModificaiton()
        {
            SetupTimers();
        }

        /// <summary>
        /// Call this when the modification should be removed
        /// </summary>
        public virtual void RemoveModificaiton() { }

        /// <summary>
        /// Call this in a update loop.
        /// </summary>
        public virtual void UpdateModificaiton() { }

        /// <summary>
        /// Call this in a fixed update loop.
        /// </summary>
        public virtual void FixedUpdateModificaiton() { }
    }
}