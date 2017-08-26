using System;
using System.Collections.Generic;
using AcrylecSkeleton.Utilities;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Enemy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    /// <summary>
    /// Purpose: Manages the whole game.
    /// Creator: MP
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        private PlayerApplication _player;

        private List<EnemyApplication> _enemies = new List<EnemyApplication>();
        public UnityEvent EnemiesChange = new Button.ButtonClickedEvent();

        public PlayerApplication Player
        {
            get
            {
                _player = _player ?? (_player = FindObjectOfType<PlayerApplication>());

                if (!_player)
                {
                    Debug.LogWarning("Couldn't find a player, make sure a player is spawned.", transform);
                    return null;
                }

                return _player;
            }
            set { _player = value; }
        }

        public List<EnemyApplication> Enemies
        {
            get
            {
                return _enemies;
            }

            set
            {
               _enemies = value;
            }
        }

        public void Start()
        {
            SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
        }

        private void SceneManagerOnSceneUnloaded(Scene arg0)
        {
            Enemies.Clear();
        }
    }
}