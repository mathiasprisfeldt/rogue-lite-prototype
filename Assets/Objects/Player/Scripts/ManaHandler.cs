using UnityEngine;
using UnityEngine.Events;

namespace Mana
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class ManaHandler : MonoBehaviour 
    {
        [SerializeField]
        private int _mana;

        private int _oldMana;

        public UnityEvent OnManaChange;

        public int Mana
        {
            get
            {
                return _mana;
            }

            set
            {
                _mana = value;
            }
        }

        public void Start()
        {
            _oldMana = _mana;
            OnManaChange.Invoke();
        }

        public void Update()
        {
            if (_oldMana != _mana)
            {
                OnManaChange.Invoke();
                _oldMana = _mana;
            }
        }
    }
}