using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AcrylecSkeleton.ModificationSystem
{
    public class ModificationHandler : MonoBehaviour
    {
        //The list containing the active modfications
        private List<Modification> _activeModifiers = new List<Modification>();

        public List<Modification> ActiveModifiers
        {
            get { return _activeModifiers; }
        }

        /// <summary>
        /// Call this to add a modification
        /// </summary>
        /// <param name="modification"></param>
        public virtual void AddModification(Modification modification)
        {
            //Finds if the modification is allready active, found by it's name
            Modification tempModifier = ActiveModifiers.FirstOrDefault(x => x.Name == modification.Name && modification.Name != "None");
            if (tempModifier != null)
            {
                //Removes the old modification
                tempModifier.RemoveModificaiton();
                ActiveModifiers.Remove(tempModifier);
                ModificationRemoved(tempModifier);
            }

            //Applies the modification
            modification.ModificationHandler = this;
            ActiveModifiers.Add(modification);
            modification.ApplyModificaiton();
            ModificationAdded(modification);
        }

        /// <summary>
        /// Is called every frame.
        /// </summary>
        public virtual void Update()
        {
            //Checks if a modification should be removed
            for (int i = ActiveModifiers.Count - 1; i >= 0; i--)
            {
                //If the modification type is timed, then check if the modification should be removed
                if (ActiveModifiers[i].ModificationType == Modification.ModificationTypeEnum.Timed)
                    if (ActiveModifiers[i].Time <= 0)
                    {
                        ActiveModifiers[i].RemoveModificaiton();
                        ActiveModifiers.RemoveAt(i);
                        continue;
                    }
                //If the modification type is timed, time down it's timer
                if (ActiveModifiers[i].ModificationType == Modification.ModificationTypeEnum.Timed)
                    ActiveModifiers[i].Time -= Time.deltaTime;

                //Calls the modifications update
                ActiveModifiers[i].UpdateModificaiton();
            }
        }

        /// <summary>
        /// Is called every frame.
        /// </summary>
        public virtual void FixedUpdate()
        {
            //Checks if a modification should be removed
            for (int i = ActiveModifiers.Count - 1; i >= 0; i--)
            {
                //If the modification timer is over 0, then calls it's fixed update
                if (ActiveModifiers[i].Time > 0)
                    ActiveModifiers[i].FixedUpdateModificaiton();
            }
        }

        /// <summary>
        /// Removes every modification.
        /// </summary>
        public virtual void RemoveAllModifications()
        {
            for (int i = ActiveModifiers.Count - 1; i >= 0; i--)
            {
                ActiveModifiers[i].Time = 0;
                ActiveModifiers[i].RemoveModificaiton();
                ModificationRemoved(ActiveModifiers[i]);
                ActiveModifiers.RemoveAt(i);
            }
        }

        /// <summary>
        /// Removes a modification.
        /// </summary>
        /// <param name="modification">The modification</param>
        public virtual void RemoveModification(Modification modification)
        {
            if (!ActiveModifiers.Contains(modification))
                throw new Exception("Modification is not in the active list");
        
            //Removes the modification    
            ActiveModifiers.Remove(modification);
            ModificationRemoved(modification);
            modification.RemoveModificaiton();
        }

        /// <summary>
        /// Called when a modification is added.
        /// </summary>
        /// <param name="modification">The modification</param>
        public virtual void ModificationAdded(Modification modification) { }

        /// <summary>
        /// Called when a modification is removed.
        /// </summary>
        /// <param name="modification">The modification</param>
        public virtual void ModificationRemoved(Modification modification) { }
    }
}