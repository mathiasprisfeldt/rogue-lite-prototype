using System;
using AcrylecSkeleton.MVC;
using Controllers;
using Flash;

namespace Assets.Objects.PlayerMovement.Player.Prefab.Player
{
	/// <summary>
	/// View class for Player MVC object.
	/// Created by: mikke
	/// Data: Monday, August 07, 2017
	/// </summary>
	public class PlayerView : View<PlayerApplication>
	{
        public ScreenFlash ScreenFlash { get; set; }

        public void Start()
	    {
	        if (App.C.Character.HealthController)
	        {
                App.C.Character.HealthController.OnHealEvent.AddListener(OnHeal);
                App.C.Character.HealthController.OnDamage.AddListener(OnDamage);
            }
	        

        }

	    private void OnDamage(Character arg0)
	    {
	        ScreenFlash.StartFlash(ScreenFlash.DamageColor,1f);
	    }

	    private void OnHeal()
	    {
            ScreenFlash.StartFlash(ScreenFlash.HealColor, 1f);

        }

	    public void OnDestroy()
	    {
            if (App.C.Character.HealthController)
            {
                App.C.Character.HealthController.OnHealEvent.RemoveListener(OnHeal);
                App.C.Character.HealthController.OnDamage.RemoveListener(OnDamage);
            }
        }
	}
}