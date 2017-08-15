using System;
using UnityEngine;

namespace AcrylecSkeleton.MVC
{
    /// <summary>
    /// Base class for all MVC objects.
    /// </summary>
    public class Element : MonoBehaviour
    {
        #region MVC Properties
        [SerializeField]
        private Application _application;

        protected Application App
        {
            get { return _application ?? (_application = GetComponentInParent<Application>()); }
            private set { _application = value; }
        }
        #endregion
    }

    /// <summary>
    /// Base class for all generic MVC objects.
    /// </summary>
    public class Element<A> : Element where A : Application
    {
        public new A App { get { return (A) base.App; } }
    }

    /// <summary>
    /// Base class for all MVC application objects.
    /// </summary>
    public class Application : MonoBehaviour
    {
        
    }

    [SelectionBase]
    public class Application<MType, VType, CType> : Application
        where MType : Model
        where VType : View
        where CType : Controller
    {
        #region MVC Properties
        /// <summary>
        /// Setting up MVC Application properties with serialized backing fields.
        /// </summary>
        [Header("MVC References:")]

        //MVC Model Property
        [SerializeField]
        private MType _model;

        public MType M
        {
            get { return _model ?? (_model = GetComponentInChildren<MType>()); }
            private set { _model = value; }
        }
        //

        //MVC View Property
        [SerializeField]
        private VType _view;

        public VType V
        {
            get { return _view ?? (_view = GetComponentInChildren<VType>()); }
            private set { _view = value; }
        }
        //

        //MVC Controller Property
        [SerializeField]
        private CType _controller;


        public CType C
        {
            get { return _controller ?? (_controller = GetComponentInChildren<CType>()); }
            private set { _controller = value; }
        }
        //
        #endregion
    }

    /// <summary>
    /// Base class for all generic MVC application objects.
    /// </summary>
    [SelectionBase]
    public class Application<MType, VType, CType, S> : Application<MType, VType, CType> 
        where MType : Model 
        where VType : View
        where CType : Controller
        where S : struct
    {
        [SerializeField, Tooltip("If changed, StateChanged will not be invoked!")]
        private S _currentState;

        /// <summary>
        /// Event invoked when changing state.
        /// <para>1 Arg: Old State</para>
        /// <para>2 Arg: New State</para>
        /// </summary>
        public event Action<S, S> StateChanged;

        public S CurrentState
        {
            get { return _currentState; }
            set { ChangeState(value); }
        }

        public S LastState { get; set; }

        protected virtual void Awake()
        {
            if (!typeof(S).IsEnum)
                Debug.LogError(String.Format("APPLICATION: {0} has wrong state type.", gameObject.name));
        }

        /// <summary>
        /// Method used to change applicant state.
        /// </summary>
        /// <param name="newState"></param>
        public virtual void ChangeState(S newState)
        {
            LastState = CurrentState;
            _currentState = newState; //Set current state to the new state

            var handler = StateChanged;
            if (handler != null) handler(LastState, CurrentState); //Check if StateChanged has any listeners, then invoke with prev and current state.
        }
    }
}
