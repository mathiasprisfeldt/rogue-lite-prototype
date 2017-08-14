using AcrylecSkeleton.MVC;
using BindingsExample;
using UnityEngine;

namespace Assets.Objects.PlayerMovement.Player.Prefab.Player
{
    /// <summary>
    /// Base state enum for Player MVC object.
    /// </summary>
    public enum PlayerState
    {
    }

    /// <summary>
    /// Application class for Player MVC object.
    /// Created by: mikke
    /// Data: Monday, August 07, 2017
    /// </summary>
    public class PlayerApplication : Application<PlayerModel, PlayerView, PlayerController, PlayerState>
    {
    }
}