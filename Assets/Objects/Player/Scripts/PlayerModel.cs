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
	    private ActionsController _actionController;

	    public ActionsController ActionController
	    {
	        get { return _actionController; }
	        set { _actionController = value; }
	    }
	}
}