using AcrylecSkeleton.MVC;
using RogueLiteInput;

namespace Assets.Objects.PlayerMovement.Player.Prefab.Player
{
    /// <summary>
    /// Controller class for Player MVC object.
    /// Created by: Mikkel Nielsen
    /// Data: Monday, August 07, 2017
    /// </summary>
    public class PlayerController : Controller<PlayerApplication>
    {
        public InputActions PlayerActions { get; set; }

        public void Awake()
        {
            PlayerActions = new InputActions();
        }

    }
}