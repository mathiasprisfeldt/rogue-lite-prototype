using AcrylecSkeleton.MVC;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Controllers;
using UnityEngine;

namespace Enemy
{
	/// <summary>
	/// Model class for Enemy MVC object.
	/// Created by: MP-L
	/// Data: Monday, August 14, 2017
	/// </summary>
	public class EnemyModel : Model<EnemyApplication>
	{
        [Header("References:")]
	    [SerializeField]
	    private Character _character;

        [SerializeField]
        private EnemyAttackAnim _enemyAttackAnim;

        [Space]
        [Header("Enemy Settings:")]
        [SerializeField]
	    private float _viewRadius;

	    [SerializeField]
	    private float _attackCooldown;

	    [SerializeField, Tooltip("How long it takes to do an action.")]
	    private float _indicatorDuration;

        [SerializeField, Tooltip("Can the enemy target player if behind him?")]
	    private bool _targetBehind;

	    [SerializeField, Tooltip("Amount of time it takes the enemy to turn around.")]
	    private float _turnSpeed;

        [SerializeField]
	    private bool _canBackPaddle;

	    [SerializeField, Tooltip("Should the enemy turn if hit from behind?")]
	    private bool _turnOnBackstab;

	    [SerializeField, Tooltip("Speed added to enemy when player is targeted.")]
	    private float _engageSpeed;

	    [SerializeField]
	    private bool _isFlying;

        [Space]
        [SerializeField]
	    private PlayerApplication _target;

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

	    public Character Character
	    {
	        get { return _character; }
	        set { _character = value; }
	    }

	    public EnemyAttackAnim AttackAnim
	    {
	        get { return _enemyAttackAnim; }
	    }

        public float AttackCooldown
	    {
	        get { return _attackCooldown; }
	        set { _attackCooldown = value; }
	    }

	    public float IndicatorDuration
	    {
	        get { return _indicatorDuration; }
	        set { _indicatorDuration = value; }
	    }

	    public bool TargetBehind
	    {
	        get { return _targetBehind; }
	        set { _targetBehind = value; }
	    }

	    public float TurnSpeed
	    {
	        get { return _turnSpeed; }
	        set { _turnSpeed = value; }
	    }

	    public bool CanBackPaddle
	    {
	        get { return _canBackPaddle; }
	        set { _canBackPaddle = value; }
	    }

	    public bool TurnOnBackstab
	    {
	        get { return _turnOnBackstab; }
	        set { _turnOnBackstab = value; }
	    }

	    public float EngageSpeed
	    {
	        get { return _engageSpeed; }
	        set { _engageSpeed = value; }
	    }

	    public bool IsFlying
	    {
	        get { return _isFlying; }
	        set { _isFlying = value; }
	    }
	}
}