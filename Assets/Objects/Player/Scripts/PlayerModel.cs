using AcrylecSkeleton.MVC;
using CharacterController;
using UnityEngine;

namespace Assets.Objects.PlayerMovement.Player.Prefab.Player
{
	/// <summary>
	/// Model class for Player MVC object.
	/// Created by: mikke
	/// Data: Monday, August 07, 2017
	/// </summary>
	public class PlayerModel : Model<PlayerApplication>
	{
        [SerializeField]
	    private Action _actionController;

	    public Action ActionController
	    {
	        get { return _actionController; }
	        set { _actionController = value; }
	    }
	}
}