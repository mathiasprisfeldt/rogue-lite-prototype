using System;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Managers;
using TMPro;
using UnityEngine;

public class PlayerGoalDoor : MonoBehaviour
{
    [SerializeField]
    private Sprite _open;

    [SerializeField]
    private Sprite _closed;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private TextMeshProUGUI _text;

    private float _numberOfEnemies;
    private float _currentNumberOfEnemies;
    private bool _isOpen = true;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemiesChange.AddListener(OnEnemyChange);
            _numberOfEnemies = GameManager.Instance.Enemies.Count;
            _currentNumberOfEnemies = _numberOfEnemies;
            OnEnemyChange();
        }

    }

    private void OnEnemyChange()
    {
        _isOpen = true;
        _currentNumberOfEnemies = 0;
        if (GameManager.Instance != null)
        {
            foreach (var e in GameManager.Instance.Enemies)
            {
                if (!e.M.Character.HealthController.IsDead)
                {
                    _currentNumberOfEnemies++;
                    _isOpen = false;
                }
            }
        }
        _spriteRenderer.sprite = _isOpen ? _open : _closed;
        _text.text = _currentNumberOfEnemies + "/" + _numberOfEnemies;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && _isOpen)
        {
            LevelManager.Instance.LoadNextLevel();
        }
    }

    public void OnDestroy()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.EnemiesChange.RemoveListener(OnEnemyChange);
    }
}