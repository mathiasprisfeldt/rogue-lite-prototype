using AcrylecSkeleton.Utilities;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Purpose: Manages the whole game.
    /// Creator: MP
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        private PlayerApplication _player;

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
    }
}