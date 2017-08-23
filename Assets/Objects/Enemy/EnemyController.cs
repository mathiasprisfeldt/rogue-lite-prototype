using System.Collections.Generic;
using System.Linq;
using AcrylecSkeleton.Extensions;
using AcrylecSkeleton.MVC;
using Archon.SwissArmyLib.Automata;
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

	    public FiniteStateMachine<EnemyApplication> StateMachine { get; set; }

	    void Awake()
	    {
	        StateMachine = new FiniteStateMachine<EnemyApplication>(App);
            _states = new List<EnemyState>();
            
	        EnemyIdle idleState = gameObject.AddComponent<EnemyIdle>();
            StateMachine.RegisterState(idleState);

	        EnemyState[] states = GetComponentsInChildren<EnemyState>();
            //Setup all states.
            foreach (EnemyState state in states)
	        {
                if (!state.enabled)
                    continue;

	            state.Machine = StateMachine;
	            state.Context = App;
                StateMachine.RegisterState(state); 

                _states.Add(state);
            }

	        StateMachine.ChangeState(states.FirstOrDefault() ?? idleState);
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
	            bool isTargetBehind = false;
	            int lookDir = App.M.Character.LookDirection;

	            if (lookDir == -1)
	                isTargetBehind = ToPlayer.x > lookDir;
	            else if (lookDir == 1)
	                isTargetBehind = ToPlayer.x < lookDir;
                
                //If the target is behind and we still can target it, do so.
	            if (isTargetBehind && !App.M.TargetBehind)
	                canTarget = false;

                //Check if we can see the player
                if (canTarget && 
                    Physics2D.RaycastNonAlloc(ownPos, ToPlayer.normalized, viewResults, Mathf.Clamp(ToPlayer.magnitude, 0, App.M.ViewRadius), LayerMask.GetMask("Platform")) > 0)
	            {
                    //We cant see the player, lose interest.
	                canTarget = false;
	                App.M.Target = null;
	            }

	            //If target is behind the enemy and is targeted and we're arent turning, turn around.
	            if (!IsTurning && isTargetBehind && canTarget)
	                Turn(-1 * lookDir);

	            App.M.Target = canTarget ? GameManager.Instance.Player : App.M.Target;
	        }
	        else
	            App.M.Target = null;
            
            foreach (EnemyState enemyState in _states)
            {
                if (StateMachine.CurrentState != enemyState && enemyState.ShouldChange())
                    StateMachine.ChangeState(enemyState);
            }

	        StateMachine.Update(Time.deltaTime);

            Debug.Log(StateMachine.CurrentState);
        }

        /// <summary>
        /// Method used to turn the enemy around.
        /// If 0, it turns around.
        /// </summary>
	    public void Turn(int dir = 0)
        {
            if (dir == 0)
                dir = -1 * App.M.Character.LookDirection;

            if (IsTurning || dir == App.M.Character.LookDirection)
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
        /// Sets velocity on character, but turns if needed.
        /// </summary>
        /// <param name="vel"></param>
	    public void SetVelocity(Vector2 vel, bool overrideYVel = false, bool forceTurn = false)
	    {
	        if (!IsTurning)
	        {
                if (!overrideYVel)
                    vel = new Vector2(vel.x, App.M.Character.Rigidbody.velocity.y);

	            App.M.Character.SetVelocity(vel, true);

	            int xVel = Mathf.RoundToInt(vel.x);
                if (!App.M.CanBackPaddle || forceTurn && xVel != 0)
	                Turn(xVel);
	        }
	    }

	    public bool IsState<T>()
	    {
	        return StateMachine.CurrentState is T;
	    }
	}
}