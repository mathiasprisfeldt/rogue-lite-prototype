using AcrylecSkeleton.MVC;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;

namespace Assets.Objects.Enemy
{
	/// <summary>
	/// Model class for Enemy MVC object.
	/// Created by: MP-L
	/// Data: Monday, August 14, 2017
	/// </summary>
	public class EnemyModel : Model<EnemyApplication>
	{
        [Header("References:"), SerializeField]
	    private CharacterController.CharacterController _characterController;

        [Space]
        [Header("Settings:")]
        [SerializeField]
	    private float _viewRadius;

        [Space]
        [SerializeField]
	    private PlayerApplication _target;

	    public CharacterController.CharacterController CharacterController
	    {
	        get { return _characterController; }
	        set { _characterController = value; }
	    }

	    public float ViewRadius
	    {
	        get { return _viewRadius; }
	        set { _viewRadius = value; }
	    }

	    public PlayerApplication Target
	    {
	        get { return _target; }
	        set { _target = value; }
	    }
	}
}