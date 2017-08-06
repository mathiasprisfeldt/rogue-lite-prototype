using UnityEngine;

namespace AcrylecSkeleton.StateKit
{
	public abstract class SKMecanimState<T>
	{
		internal int mecanimStateHash;
		protected SKMecanimStateMachine<T> machine;
		protected T context;


	    protected SKMecanimState()
		{}


		/// <summary>
		/// constructor that takes the mecanim state name as a string. Note that if a mecanimStateName is passed into the constructor
		/// the reason and update methods will not be called until Mecanim finishes any transitions and is completely in the mecanim state.
		/// Do not pass in a mecanim state name if you do not want that behaviour.
		/// </summary>
		protected SKMecanimState( string mecanimStateName ) : this( Animator.StringToHash( mecanimStateName ) )
		{}


		/// <summary>
		/// constructor that takes the mecanim state hash
		/// </summary>
		protected SKMecanimState( int mecanimStateHash )
		{
			this.mecanimStateHash = mecanimStateHash;
		}


		internal void SetMachineAndContext( SKMecanimStateMachine<T> machine, T context )
		{
			this.machine = machine;
			this.context = context;
			OnInitialized();
		}


		/// <summary>
		/// called directly after the machine and context are set allowing the state to do any required setup
		/// </summary>
		public virtual void OnInitialized()
		{}


		public virtual void Begin()
		{}


		public virtual void Reason()
		{}


		public abstract void Update( float deltaTime, AnimatorStateInfo stateInfo );


		public virtual void End()
		{}

	}
}