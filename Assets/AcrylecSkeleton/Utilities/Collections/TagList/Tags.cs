using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<string> _tagList;

        public List<string> TagList
        {
            get
            {
                if (_tagList == null && TagListUnSplitted != null)
                {
                    _tagList = TagListUnSplitted.Split('@').ToList();
                    _tagList.RemoveAt(_tagList.Count - 1);
                }

                return _tagList;
            }
        }

        public string TagListUnSplitted; //Tags represented by a string splitted on @
        public int TagMask; //Tags representated by a bit mask.

        /// <summary>
        /// Checks if this tag collection contains a specific tag.
        /// Note: If you're checking directly on a gameobject, use method with GameObject go for less memory allocation.
        /// </summary>
        public bool Contains(string tag)
        {
            return TagList.Contains(tag);
        }

        public bool Contains(GameObject go)
        {
            return TagList != null && TagList.Any(go.CompareTag);
        }
    }
}