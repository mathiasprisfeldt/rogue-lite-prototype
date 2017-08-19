using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
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

    private int _oldHealth;
    private PlayerApplication _playerApplication;


    private void FindPlayer()
    {
        _playerApplication = GameObject.FindObjectOfType<PlayerApplication>();
        if (_playerApplication && _playerApplication.C.Health != null)
        {
            _healthController = _playerApplication.C.Health;
            UpdateHealthbar();
        }
    }

    // Update is called once per frame
	void Update ()
    {
        if (_playerApplication == null && GameManager.Instance && GameManager.Instance.Player)
        {
            _playerApplication = GameManager.Instance.Player;
            _healthController = _playerApplication.C.Health;
            UpdateHealthbar();
        }
        if (_healthController)
        {
            if (_healthController.HealthAmount != _oldHealth)
                UpdateHealthbar();
            _oldHealth = (int)_healthController.HealthAmount;
        }
	    
    }

    private void UpdateHealthbar()
    {
        for (int i = 0; i < _healthContainers.Count; i++)
        {
            _healthContainers[i].enabled = _healthController.HealthAmount >= i + 1;
        }
        _overflowText.text = _healthController.HealthAmount > _healthContainers.Count ? (_healthController.HealthAmount - _healthContainers.Count) + "x" : "";
    }
}
