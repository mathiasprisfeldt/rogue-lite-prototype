using System;
using AcrylecSkeleton.MVC;
using CharacterController;
using Controllers;
using Flash;
using UnityEngine;

namespace Assets.Objects.PlayerMovement.Player.Prefab.Player
{
    /// <summary>
    /// View class for Player MVC object.
    /// Created by: mikke
    /// Data: Monday, August 07, 2017
    /// </summary>
    public class PlayerView : View<PlayerApplication>
    {
        [SerializeField]
	    private ActionsController _actionController;

        public ScreenFlash ScreenFlash { get; set; }

        public void Start()
	    {
	        if (App.C.Character.HealthController)
	        {
                App.C.Character.HealthController.OnHealEvent.AddListener(OnHeal);
                App.C.Character.HealthController.OnDamage.AddListener(OnDamage);
                App.C.Character.OnSafetyRespawn.AddListener(OnSafetyRespawn);
            }
	        

        }

	    private void OnSafetyRespawn()
	    {
	        if (ScreenFlash != null)
	        {
                ScreenFlash.StartFlash(Color.black, 1f, 0);
	            _actionController.WaitForInputHorizontal = true;
	        }
                
        }

	    private void OnDamage(Character arg0)
	    {
            if(ScreenFlash != null)
	            ScreenFlash.StartFlash(ScreenFlash.DamageColor,1f,0.1f);
	    }

	    private void OnHeal()
	    {
	        if (ScreenFlash != null)
                ScreenFlash.StartFlash(ScreenFlash.HealColor, 1f,0.1f);
        }
    }
}
        private ActionsController _actionController;

        public ScreenFlash ScreenFlash { get; set; }

        {
            if (App.C.Character.HealthController)
            {
                App.C.Character.HealthController.OnHealEvent.AddListener(OnHeal);
                App.C.Character.HealthController.OnDamage.AddListener(OnDamage);
                App.C.Character.OnSafetyRespawn.AddListener(OnSafetyRespawn);


        private void OnSafetyRespawn()
        {
            if (ScreenFlash != null)
            {
                _actionController.WaitForInputHorizontal = true;
            }

        private void OnDamage(Character arg0)
        {
            if (ScreenFlash != null)
                ScreenFlash.StartFlash(ScreenFlash.DamageColor, 1f, 0.1f);
        }
        private void OnHeal()
        {
            if (ScreenFlash != null)
                ScreenFlash.StartFlash(ScreenFlash.HealColor, 1f, 0.1f);