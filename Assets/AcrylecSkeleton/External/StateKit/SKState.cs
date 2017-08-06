using UnityEngine;

namespace AcrylecSkeleton.StateKit
{
	public abstract class SKState<T>
	{
		protected int mecanimStateHash;
		protected SKStateMachine<T> machine;
		protected T context;


	    protected SKState()
	    {
	    }


	    /// <summary>
	    /// constructor that takes the mecanim state name as a string
	    /// </summary>
	    protected SKState(string mecanimStateName) : this(Animator.StringToHash(mecanimStateName))
	    {
	    }


	    /// <summary>
		/// constructor that takes the mecanim state hash
		/// </summary>
	    protected SKState( int mecanimStateHash )
		{
			this.mecanimStateHash = mecanimStateHash;
		}


	    internal void SetMachineAndContext(SKStateMachine<T> machine, T context)
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


		public virtual void Begin(object o)
		{}
		
		
		public virtual void Reason()
		{}
		
		
		public abstract void Update( float deltaTime );
		
		
		public virtual void End()
		{}

	    public override string ToString()
	    {
	        return GetType().Name;
	    }
	}
}