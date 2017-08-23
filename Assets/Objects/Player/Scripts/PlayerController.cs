using AcrylecSkeleton.MVC;
using Controllers;
using Health;
using RogueLiteInput;
using UnityEngine;

namespace Assets.Objects.PlayerMovement.Player.Prefab.Player
{
    /// <summary>
    /// Controller class for Player MVC object.
    /// Created by: Mikkel Nielsen
    /// Data: Monday, August 07, 2017
    /// </summary>
    public class PlayerController : Controller<PlayerApplication>
    {
        [SerializeField]
        private Character _character;

        public InputActions PlayerActions { get; set; }
        public HealthController Health { get; set; }

        public Character Character
        {
            get { return _character; }
            set { _character = value; }
        }

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