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

    [SerializeField]
    private Sprite _empty;

    [SerializeField]
    private Image _background;

    [SerializeField]
    private GameObject _hearth;

    [SerializeField]
    private HorizontalLayoutGroup _layout;

    [SerializeField]
    private float _widthPerHearth;

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
        float backgroundXSize = 0;

        if (_healthController.MaxHealth < _healthContainers.Count)
        {
            for (int i = (_healthContainers.Count - 1); i >= _healthController.MaxHealth; i--)
            {
                if(_healthContainers[i] != null)
                    Destroy(_healthContainers[i].gameObject);
                _healthContainers.RemoveAt(i);
            }
        }

        for (int i = 0; i < _healthController.MaxHealth; i++)
        {
            if (_healthContainers.Count <= i || _healthContainers[i] == null)
            {
                GameObject newHearth = Instantiate(_hearth, _layout.transform);
                if (_healthContainers.Count > i && _healthContainers[i] == null)
                    _healthContainers[i] = newHearth.GetComponent<Image>();
                else
                    _healthContainers.Add(newHearth.GetComponent<Image>());
            }

            if (Mathf.Floor(temp) >= i + 1)
            {
                _healthContainers[i].sprite = _full;
            }
            else if (temp == i + .5f)
            {
                _healthContainers[i].sprite = _half;
            }
            else
            {
                _healthContainers[i].sprite = _empty;
            }
            backgroundXSize += _healthContainers[i].rectTransform.rect.width;
        }



        _background.rectTransform.sizeDelta = new Vector2(_widthPerHearth * _healthContainers.Count, _background.rectTransform.sizeDelta.y);

        //_overflowText.text = _healthController.HealthAmount > _healthContainers.Count ? "+" + (Mathf.Floor(_healthController.HealthAmount - _healthContainers.Count)) : "";
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
