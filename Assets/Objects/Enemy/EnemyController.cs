using System.Collections.Generic;
using System.Linq;
using AcrylecSkeleton.MVC;
using AcrylecSkeleton.StateKit;
using Managers;
using UnityEngine;

namespace Assets.Enemy
{
	/// <summary>
	/// Controller class for Enemy MVC object.
	/// Created by: MP-L
	/// Data: Monday, August 14, 2017
	/// </summary>
	public class EnemyController : Controller<EnemyApplication>
	{
	    private EnemyState _initialState;

	    public List<EnemyState> States { get; set; }
	    public EnemyState LastState { get; set; }
	    public EnemyState CurrentState { get; set; }

	    void Start()
	    {
            States = new List<EnemyState>();

	        //Setup all states.
	        foreach (EnemyState state in GetComponentsInChildren<EnemyState>())
	        {
	            States.Add(state);

	            if (!_initialState && state.IsActive)
	                _initialState = state;

	            state.IsActive = false;
	        }

            //Setting initial state to active.
            ChangeState(!_initialState ? States.FirstOrDefault() : _initialState);
	    }

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
            
            //Check the states prerequisites & parallel updates.
	        foreach (EnemyState enemyState in States)
	        {
	            if (enemyState.CheckPrerequisite())
	            {
	                ChangeState(enemyState, enemyState.IsIsolated);
	                break;
	            }
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
	        LastState = CurrentState;

	        //Disable all other state components if isolated.
            if (isolate)
	            States.Where(state => state != desiredState).ToList().ForEach(state => state.IsActive = false);

	        desiredState.IsActive = true;
	        CurrentState = desiredState;
        }
	}
}