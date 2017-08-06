using UnityEngine;
using System;
using System.Collections.Generic;

namespace AcrylecSkeleton.StateKit
{
	public sealed class SKStateMachine<T>
	{
	    private readonly T _context;
		#pragma warning disable
		public event Action OnStateChanged;
		#pragma warning restore

		public SKState<T> currentState { get { return _currentState; } }
		public SKState<T> previousState;
		public float elapsedTimeInState = 0f;

		private readonly Dictionary<System.Type, SKState<T>> _states = new Dictionary<System.Type, SKState<T>>();
		private SKState<T> _currentState;


		public SKStateMachine( T context, SKState<T> initialState )
		{
			this._context = context;

			// setup our initial state
			AddState( initialState );
			_currentState = initialState;
			_currentState.Begin(null);
		}

	    public SKStateMachine(T context)
	    {
	        _context = context;
	    }

	    public void SetIdle()
	    {
            if (_currentState != null)
                _currentState.End();

	        previousState = _currentState;
            _currentState = null;
	        elapsedTimeInState = 0;

            if (OnStateChanged != null)
	            OnStateChanged();
	    }

	    public void ForceChangeState<R>() where R : SKState<T>
	    {
	        var type = typeof(R);

            CheckStateSanity(type);

	        _currentState = _states[type];
            _currentState.Begin(null);
	    }

		/// <summary>
		/// adds the state to the machine
		/// </summary>
		public void AddState( SKState<T> state )
		{
			state.SetMachineAndContext( this, _context );
			_states[state.GetType()] = state;
		}


		/// <summary>
		/// ticks the state machine with the provided delta time
		/// </summary>
		public void Update( float deltaTime )
		{
		    if (_currentState == null)
		        return;

			elapsedTimeInState += deltaTime;
			_currentState.Reason();
			_currentState.Update( deltaTime );
		}

	    public void CheckStateSanity(Type type)
	    {
#if UNITY_EDITOR
            // do a sanity check while in the editor to ensure we have the given state in our state list
            if (!_states.ContainsKey(type))
            {
                var error = GetType() + ": state " + type + " does not exist. Did you forget to add it by calling addState?";
                Debug.LogError(error);
                throw new Exception(error);
            }
#endif
        }

        internal void ChangeState<R>() where R : SKState<T>
        {
            ChangeState<R>(null);
        }

        /// <summary>
        /// changes the current state
        /// </summary>
        public R ChangeState<R>(object o) where R : SKState<T>
		{
			// avoid changing to the same state
			var newType = typeof( R );
			if(_currentState != null && _currentState.GetType() == newType )
				return _currentState as R;

			// only call end if we have a currentState
			if( _currentState != null )
				_currentState.End();

            CheckStateSanity(newType);

			// swap states and call begin
			previousState = _currentState;
			_currentState = _states[newType];
			_currentState.Begin(o);
			elapsedTimeInState = 0f;

			// fire the changed event if we have a listener
			if( OnStateChanged != null )
				OnStateChanged();

			return _currentState as R;
		}
	}
}