using System;
using System.Collections.Generic;
using System.Linq;
using AcrylecSkeleton.Extensions;
using AcrylecSkeleton.MVC;
using Archon.SwissArmyLib.Automata;
using Controllers;
using Managers;
using UnityEngine;

namespace Enemy
{
	/// <summary>
	/// Controller class for Enemy MVC object.
	/// Created by: MP-L
	/// Data: Monday, August 14, 2017
	/// </summary>
	public class EnemyController : Controller<EnemyApplication>
	{
        private readonly RaycastHit2D[] viewResults = new RaycastHit2D[1]; //Array used to store results from testing player view.
	    private List<EnemyState> _states;
	    private float _whereToTurnTo;
	    private float _turnTimer;

	    public Vector2 ToPlayer { get; private set; }
	    public bool IsTurning { get; private set; }
	    public bool IsTargetBehind { get; private set; }

        public FiniteStateMachine<EnemyApplication> StateMachine { get; set; }

	    void Awake()
	    {
	        StateMachine = new FiniteStateMachine<EnemyApplication>(App);
            _states = new List<EnemyState>();
            
	        EnemyIdle idleState = gameObject.AddComponent<EnemyIdle>();
            StateMachine.RegisterState(idleState);

	        _states = GetComponentsInChildren<EnemyState>().ToList();
	        _states.Reverse();
            //Setup all states.
            foreach (EnemyState state in _states)
	        {
                if (!state.enabled)
                    continue;

	            state.Machine = StateMachine;
	            state.Context = App;
                StateMachine.RegisterState(state); 
            }

	        StateMachine.ChangeState(_states.FirstOrDefault() ?? idleState);

	    }

	    void Start()
	    {
	        App.M.Character.HealthController.OnDamage.AddListener(OnDamage);
	        App.M.Character.HealthController.OnDead.AddListener(OnDead);
        }

	    void Update()
	    {
	        Vector2 ownPos = App.M.Character.Origin;
	        Vector2 plyPos = GameManager.Instance.Player.transform.position.ToVector2();

            //Calculate if the enemy can turn around.
            if (IsTurning)
	        {
	            _turnTimer -= Time.deltaTime;

	            if (_turnTimer <= 0)
	            {
	                App.M.Character.Flip(_whereToTurnTo);
	                IsTurning = false;
	            }
            }

	        ToPlayer = plyPos - ownPos;
	        //Checking if player is in sight
	        if (GameManager.Instance.Player &&
	            ToPlayer.magnitude <=
	            App.M.ViewRadius)
	        {
	            bool canTarget = true;

	            //Check if the player is behind the player and if can be target if behind.
	            IsTargetBehind = false;
	            int lookDir = App.M.Character.LookDirection;

	            IsTargetBehind = (plyPos.x < ownPos.x ? -1 : 1) != lookDir;

                //If the target is behind and we still can target it, do so.
	            if (IsTargetBehind && !App.M.TargetBehind)
	                canTarget = false;

                //Check if we can see the player
                if (canTarget && 
                    Physics2D.RaycastNonAlloc(ownPos, ToPlayer.normalized, viewResults, Mathf.Clamp(ToPlayer.magnitude, 0, App.M.ViewRadius), LayerMask.GetMask("Platform")) > 0)
	            {
                    //We cant see the player, lose interest.
	                canTarget = false;
	                App.M.Target = null;
	            }

                //If we're dead dont even bother targeting us.
	            if (GameManager.Instance.Player.M.ActionController.HealthController.IsDead)
	            {
	                canTarget = false;
	                App.M.Target = null;
	            }

	            //If target is behind the enemy and is targeted and we're arent turning, turn around.
	            if (!IsTurning && IsTargetBehind && canTarget)
	                Turn(-1 * lookDir);

	            App.M.Target = canTarget ? GameManager.Instance.Player : App.M.Target;
	        }
	        else
	            App.M.Target = null;
            
            foreach (EnemyState enemyState in _states)
            {
                if (StateMachine.CurrentState != enemyState && enemyState.ShouldTakeover())
                    StateMachine.ChangeState(enemyState);
            }

	        StateMachine.Update(Time.deltaTime);
        }


	    /// <summary>
        /// Method used to turn the enemy around.
        /// If 0, it turns around.
        /// </summary>
	    public void Turn(int dir, bool flip = false)
        {
            if (dir == 0 && flip)
                dir = -1 * App.M.Character.LookDirection;

            if (IsTurning || 
                dir == App.M.Character.LookDirection || 
                dir == 0 && !flip || 
                App.M.Character.HealthController.IsDead)
                return;

            //Turn around instantly if turn speed is 0.
            if (App.M.TurnSpeed == 0)
            {
                App.M.Character.Flip(dir);
                return;
            }

            App.M.Character.SetVelocity(Vector2.zero);
            IsTurning = true;
            _turnTimer = App.M.TurnSpeed;
            _whereToTurnTo = dir;
        }

        /// <summary>
        /// Flips/Turns the enemy around.
        /// </summary>
	    public void Turn()
	    {
	        Turn(0, true);
	    }

	    /// <summary>
	    /// Moves the enemy to a specific direction, but turns if needed.
	    /// It calculates the velocity with movement speed and fixedDeltaTime.
	    /// </summary>
	    /// <param name="dir">Direction movement</param>
	    /// <param name="overrideYVel">If false, it doesn't touch Y axis.</param>
	    /// <param name="forceTurn">Should it force turning around?</param>
	    public void Move(Vector2 dir, bool overrideYVel = false, bool forceTurn = false)
        {
	        if (!IsTurning)
	        {
	            Vector2 vel = dir;

                if (!overrideYVel)
                    vel = new Vector2(dir.x, App.M.Character.Rigidbody.velocity.y);

	            App.M.Character.SetVelocity(vel * Time.fixedDeltaTime, true, App.M.Target ? App.M.EngageSpeed : 0);

	            int xDir = Mathf.RoundToInt(dir.x);
                if (!App.M.CanBackPaddle || forceTurn)
	                Turn(xDir);
	        }
	    }

	    public bool IsState<T>()
	    {
	        return StateMachine.CurrentState is T;
	    }

	    private void OnDamage(Character from)
	    {
	        if (App.M.TurnOnBackstab && IsTargetBehind)
                Turn();
	    }

	    private void OnDead()
	    {
	        IsTurning = false;
	    }
	}
}