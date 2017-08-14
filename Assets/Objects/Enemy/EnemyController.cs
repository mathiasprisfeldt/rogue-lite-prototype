using AcrylecSkeleton.MVC;
using Managers;
using UnityEngine;

namespace Assets.Objects.Enemy
{
	/// <summary>
	/// Controller class for Enemy MVC object.
	/// Created by: MP-L
	/// Data: Monday, August 14, 2017
	/// </summary>
	public class EnemyController : Controller<EnemyApplication>
	{
	    void Update()
	    {
	        //Checking if player is in sight
	        if (GameManager.Instance.Player &&
	            Vector2.Distance(GameManager.Instance.Player.transform.position, App.transform.position) <=
	            App.M.ViewRadius)
	        {
	            App.M.Target = GameManager.Instance.Player;
	        }
	        else
	            App.M.Target = null;
	    }
	}
}