using AcrylecSkeleton.MVC;
using Health;
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
        public HealthController Health { get; set; }

        public void Awake()
        {
            Health = GetComponentInChildren<HealthController>();
            PlayerActions = new InputActions();
        }

        public void Update()
        {
            if(PlayerActions == null)
                PlayerActions = new InputActions();
        }
    }
}