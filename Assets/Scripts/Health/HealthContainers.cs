using System;
using System.Collections;
using System.Collections.Generic;
using AcrylecSkeleton.Utilities;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Controllers;
using Health;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthContainers : MonoBehaviour
{
    [SerializeField]
    private List<Image> _healthContainers = new List<Image>();

    [SerializeField]
    private HealthController _healthController;

    [SerializeField]
    private TextMeshProUGUI _overflowText;

    [SerializeField]
    private Sprite _half;

    [SerializeField]
    private Sprite _full;

    private PlayerApplication _playerApplication;


    private void OnDamage(Character arg0)
    {
        UpdateHealthbar();
    }

    // Update is called once per frame
	void Update ()
    {
        if (_playerApplication == null && GameManager.Instance && GameManager.Instance.Player)
        {
            _playerApplication = GameManager.Instance.Player;
            _healthController = _playerApplication.C.Health;
            UpdateHealthbar();
            if (_healthController)
            {
                _healthController.OnDamage.AddListener(OnDamage);
                _healthController.OnHealEvent.AddListener(UpdateHealthbar);
            }
                
        }	    
    }

    private void UpdateHealthbar()
    {
        var temp = MathUtils.RoundToNearest(_healthController.HealthAmount, 2);
        for (int i = 0; i < _healthContainers.Count; i++)
        {
            if (Mathf.Floor(temp) >= i + 1)
            {
                _healthContainers[i].enabled = true;
                _healthContainers[i].sprite = _full;
            }
            else if (temp == i + .5f)
            {
                _healthContainers[i].enabled = true;
                _healthContainers[i].sprite = _half;
            }
            else
            {
                _healthContainers[i].enabled = false;
            }
            
        }
        _overflowText.text = _healthController.HealthAmount > _healthContainers.Count ? (Mathf.Floor(_healthController.HealthAmount - _healthContainers.Count)) + "x" : "";
    }

    public void OnDestroy()
    {
        if (_healthController)
        {
            _healthController.OnDamage.RemoveListener(OnDamage);
            _healthController.OnHealEvent.RemoveListener(UpdateHealthbar);

        }

    }
}
