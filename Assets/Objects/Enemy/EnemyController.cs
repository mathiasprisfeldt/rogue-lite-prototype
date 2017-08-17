using System.Collections.Generic;
using System.Linq;
using AcrylecSkeleton.Extensions;
using AcrylecSkeleton.MVC;
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
	    private float _whereToTurnTo;
	    private float _turnTimer;

	    private EnemyState _initialState;

	    public bool IsTurning { get; private set; }
        public List<EnemyState> States { get; set; }
	    public EnemyState LastState { get; set; }
	    public EnemyState CurrentState { get; set; }

	    void Start()
	    {
            States = new List<EnemyState>();

	        //Setup all states.
	        foreach (EnemyState state in GetComponentsInChildren<EnemyState>())
	        {
                if (!state.enabled)
                    continue;

	            States.Add(state);

	            if (!_initialState && state.IsActive)
	                _initialState = state;

	            state.IsActive = false;
	        }

	        if (!States.Any())
	        {
	            Debug.LogWarning("EnemyController has no AI states.", transform);
                return;
	        }

	        //Setting initial state to active.
            //If initial state isn't found yet, try to find one.
	        if (!_initialState)
	            _initialState = States.FirstOrDefault();

            ChangeState(!_initialState ? States.FirstOrDefault() : _initialState);
	    }

	    void Update()
	    {
	        Vector2 ownPos = App.M.Character.Rigidbody.position;
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

	        Vector2 plyDist = ownPos - plyPos;
	        //Checking if player is in sight
	        if (GameManager.Instance.Player &&
	            plyDist.magnitude <=
	            App.M.ViewRadius)
	        {
	            bool canTarget = true;

	            //Check if the player is behind the player and if can be target if behind.
	            bool isTargetBehind = false;
	            int lookDir = App.M.Character.LookDirection;

	            if (lookDir == -1)
	                isTargetBehind = plyDist.x < lookDir;
	            else if (lookDir == 1)
	                isTargetBehind = plyDist.x > lookDir;
                
                //If the target is behind and we still can target it, do so.
	            if (isTargetBehind && !App.M.TargetBehind)
	                canTarget = false;

                //Check if we can see the player
	            if (canTarget && 
                    Physics2D.RaycastAll(ownPos, ownPos.DirectionTo(plyPos), 10, LayerMask.GetMask("Platform")).Any())
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

	        //Check the states prerequisites & parallel updates.
	        foreach (EnemyState enemyState in States)
	        {
	            if (enemyState.CheckPrerequisite())
	                ChangeState(enemyState, enemyState.IsIsolated);

                if (enemyState.IsActive)
                    enemyState.StateUpdate();
	        }
	    }

        /// <summary>
        /// Changes the current enemy state to desired one.
        /// </summary>
        /// <typeparam name="T">The state to change to.</typeparam>
        /// <param name="isolate">Should it disable all other states.</param>
	    public void ChangeState<T>(bool isolate = true)
	    {
	        //Find requested state
	        EnemyState newState = States.FirstOrDefault(state => state is T);
            ChangeState(newState, isolate);
	    }

	    public void ChangeState(EnemyState desiredState, bool isolate = true)
	    {
	        //If we cant find it, log warn.
	        if (!desiredState)
	        {
	            Debug.LogWarning("EnemyController tried to find state of type " + desiredState.GetType() + " but couldn't. Ignoring request.", transform);
	            return;
	        }

	        if (CurrentState == desiredState)
	        {
	            Debug.LogWarning("EnemyController tried to change state to the same one. Ignoring request.", transform);
	            return;
	        }

	        //Current is now last
	        if (CurrentState)
	        {
	            LastState = CurrentState;
	            LastState.StateEnd();
	        }

	        //Disable all other state components if isolated.
            if (isolate)
	            States.Where(state => state != desiredState).ToList().ForEach(state => state.IsActive = false);

	        desiredState.IsActive = true;

	        CurrentState = desiredState;
            CurrentState.StateStart();
        }

        /// <summary>
        /// Method used to turn the enemy around.
        /// </summary>
	    public void Turn(int dir)
        {
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
	    public void SetVelocity(Vector2 vel)
	    {
	        if (!IsTurning)
	        {
	            App.M.Character.SetVelocity(vel);
	            Turn(Mathf.RoundToInt(vel.x));
	        }
	    }

        /// <summary>
        /// Changes the state back to the initial one.
        /// </summary>
	    public void ResetToInitial()
	    {
	        ChangeState(_initialState);
	    }

        /// <summary>
        /// Changes state to the last one.
        /// </summary>
	    public void ResetToLast()
	    {
	        ChangeState(LastState);
	    }
	}
}