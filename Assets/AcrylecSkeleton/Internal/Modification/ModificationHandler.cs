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

        /// <summary>
        /// Call this to add a modification
        /// </summary>
        /// <param name="modification"></param>
        public virtual void AddModification(Modification modification)
        {
            //Finds if the modification is allready active, found by it's name
            Modification tempModifier = _activeModifiers.FirstOrDefault(x => x.Name == modification.Name && modification.Name != "None");
            if (tempModifier != null)
            {
                //Removes the old modification
                tempModifier.RemoveModificaiton();
                _activeModifiers.Remove(tempModifier);
                ModificationRemoved(tempModifier);
            }

            //Applies the modification
            modification.ModificationHandler = this;
            _activeModifiers.Add(modification);
            modification.ApplyModificaiton();
            ModificationAdded(modification);
        }

        /// <summary>
        /// Is called every frame.
        /// </summary>
        public virtual void Update()
        {
            //Checks if a modification should be removed
            for (int i = _activeModifiers.Count - 1; i >= 0; i--)
            {
                //If the modification type is timed, then check if the modification should be removed
                if (_activeModifiers[i].ModificationType == Modification.ModificationTypeEnum.Timed)
                    if (_activeModifiers[i].Time <= 0)
                    {
                        _activeModifiers[i].RemoveModificaiton();
                        _activeModifiers.RemoveAt(i);
                        continue;
                    }
                //If the modification type is timed, time down it's timer
                if (_activeModifiers[i].ModificationType == Modification.ModificationTypeEnum.Timed)
                    _activeModifiers[i].Time -= Time.deltaTime;

                //Calls the modifications update
                _activeModifiers[i].UpdateModificaiton();
            }
        }

        /// <summary>
        /// Is called every frame.
        /// </summary>
        public virtual void FixedUpdate()
        {
            //Checks if a modification should be removed
            for (int i = _activeModifiers.Count - 1; i >= 0; i--)
            {
                //If the modification timer is over 0, then calls it's fixed update
                if (_activeModifiers[i].Time > 0)
                    _activeModifiers[i].FixedUpdateModificaiton();
            }
        }

        /// <summary>
        /// Removes every modification.
        /// </summary>
        public virtual void RemoveAllModifications()
        {
            for (int i = _activeModifiers.Count - 1; i >= 0; i--)
            {
                _activeModifiers[i].Time = 0;
                _activeModifiers[i].RemoveModificaiton();
                ModificationRemoved(_activeModifiers[i]);
                _activeModifiers.RemoveAt(i);
            }
        }

        /// <summary>
        /// Removes a modification.
        /// </summary>
        /// <param name="modification">The modification</param>
        public virtual void RemoveModification(Modification modification)
        {
            if (!_activeModifiers.Contains(modification))
                throw new Exception("Modification is not in the active list");
        
            //Removes the modification    
            _activeModifiers.Remove(modification);
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