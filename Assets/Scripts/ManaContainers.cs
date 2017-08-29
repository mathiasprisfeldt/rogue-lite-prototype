using System.Collections.Generic;
using AcrylecSkeleton.Utilities;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Controllers;
using Health;
using Mana;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ManaContainers
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class ManaContainers : MonoBehaviour 
    {
        [SerializeField]
        private List<Image> _manaContrainers = new List<Image>();

        [SerializeField]
        private ManaHandler _manaHandler;

        [SerializeField]
        private TextMeshProUGUI _overflowText;

        [SerializeField]
        private Sprite _half;

        [SerializeField]
        private Sprite _full;

        private PlayerApplication _playerApplication;


        // Update is called once per frame
        void Update()
        {
            if (_playerApplication == null && GameManager.Instance && GameManager.Instance.Player)
            {
                _playerApplication = GameManager.Instance.Player;
                _manaHandler = _playerApplication.C.Character.ManaHandler;
                UpdateManabar();
                if (_manaHandler)
                {
                    _manaHandler.OnManaChange.AddListener(UpdateManabar);
                }

            }
        }

        private void UpdateManabar()
        {
            var temp = MathUtils.RoundToNearest(_manaHandler.Mana, 2);
            for (int i = 0; i < _manaContrainers.Count; i++)
            {
                if (Mathf.Floor(temp) >= i + 1)
                {
                    _manaContrainers[i].enabled = true;
                    _manaContrainers[i].sprite = _full;
                }
                else if (temp == i + .5f)
                {
                    _manaContrainers[i].enabled = true;
                    _manaContrainers[i].sprite = _half;
                }
                else
                {
                    _manaContrainers[i].enabled = false;
                }

            }
            _overflowText.text = _manaHandler.Mana > _manaContrainers.Count ? (Mathf.Floor(_manaHandler.Mana - _manaContrainers.Count)) + "x" : "";
        }

        public void OnDestroy()
        {
            if (_manaHandler)
            {
                _manaHandler.OnManaChange.RemoveListener(UpdateManabar);

            }

        }
    }
}