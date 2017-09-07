using System.Collections.Generic;
using UnityEngine;

namespace Oneway
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class CompositeWhenParentChange : MonoBehaviour 
    {
        [SerializeField]
        private List<Collider2D> _colliders = new List<Collider2D>();

        private bool _done;
        private GameObject _startParent;

        public void update()
        {
            if (_done && transform.parent != _startParent)
            {
                foreach (var c in _colliders)
                {
                    c.usedByComposite = true;
                }
                _done = true;
            }
        }
    }
}