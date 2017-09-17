using System;
using UnityEngine;

namespace AcrylecSkeleton.Utilities.Collections
{
    /// <summary>
    /// Purpose: List of different tags from tags manager.
    /// Creator: MP
    /// </summary>
    [Serializable]
    public struct Tags
    {
        private static readonly string[] PossibleTags = UnityEditorInternal.InternalEditorUtility.tags;

        public int TagList; //Tags representated by a bit mask.

        /// <summary>
        /// Checks if this tag collection contains a specific tag.
        /// Note: If you're checking directly on a gameobject, use method with GameObject go for less memory allocation.
        /// </summary>
        public bool Contains(string tag)
        {
            int value = 0;

            for (var index = 0; index < PossibleTags.Length; index++)
            {
                string possibleTag = PossibleTags[index];

                if (tag == possibleTag)
                    value = index;
            }

            return TagList.Contains(value);
        }

        public bool Contains(GameObject go)
        {
            foreach (string possibleTag in PossibleTags)
                if (Contains(possibleTag))
                    return go.CompareTag(possibleTag);

            return false;
        }
    }
}