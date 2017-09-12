using System.Collections.Generic;
using System.Linq;
using AcrylecSkeleton.Extensions;
using AcrylecSkeleton.MVC;
using Archon.SwissArmyLib.Automata;
using Archon.SwissArmyLib.Events;
using Archon.SwissArmyLib.Utils;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
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
	public class EnemyController : Controller<EnemyApplication>, TellMeWhen.ITimerCallback
	{
	    private const int EVENT_TURN = 0,
	        EVENT_STAGGING = 1,
            EVENT_MEMORY = 2;

        private readonly RaycastHit2D[] viewResults = new RaycastHit2D[1]; //Array used to store results from testing player view.
	    private List<EnemyState> _states;
        private float _whereToTurnTo;

        [SerializeField]
	    private PlayerApplication _target;

	    public Vector2 ToPlayer { get; private set; }
	    public bool IsTurning { get; private set; }
	    public bool IsStagging { get; set; }
	    public bool IsRemembering { get; set; } //Is the enemy remembering the player?

	    public PlayerApplication Target
	    {
	        get { return _target; }
	        set
	        {
                if (value && !IsRemembering)
                    Remember();
                else if (value)
	                _target = value;
	        }
	    }

        public bool IsTargetBehind
	    {
	        get
	        {
	            PlayerApplication ply = GameManager.Instance.Player;

	            if (!ply)
	                return false;

                return (ply.transform.position.x < App.transform.position.x ? -1 : 1) != App.M.Character.LookDirection; 
	        }
	    }

	    public FiniteStateMachine<EnemyApplication> StateMachine { get; set; }

	    void Awake()
	    {
	        StateMachine = new FiniteStateMachine<EnemyApplication>(App);

	        _states = GetComponentsInChildren<EnemyState>().ToList();
            //Setup all states.
            foreach (EnemyState state in _states)
	        {
                if (!state.enabled)
                    continue;

	            state.Context = App;
	            state.Machine = StateMachine;
                StateMachine.RegisterState(state); 
            }

	        EnemyIdle idleState = gameObject.AddComponent<EnemyIdle>();
	        StateMachine.RegisterState(idleState);

            StateMachine.ChangeState(idleState);
	    }

	    void Start()
	    {
	        App.M.Character.HealthController.OnDamage.AddListener(OnDamage);
	        App.M.Character.HealthController.OnDead.AddListener(OnDead);
        }

	    void Update()
	    {
            if (App.M.Character.HealthController.IsDead)
                return;

            //Check if the player is in enemy sight
	        bool plyInView = false;
	        float viewBoxMaxLength = 0;
	        if (App.M.ViewBox)
	        {
	            viewBoxMaxLength = App.M.ViewBox.CollidersToCheck[0].bounds.max.magnitude;
	            plyInView = App.M.ViewBox.IsColliding();
	        }

	        PlayerApplication ply = GameManager.Instance.Player;

	        Vector2 ownPos = App.M.Character.Origin;
	        Vector2 plyPos = ply ? ply.transform.position.ToVector2() : Vector2.zero;

	        PlayerApplication newTarget = Target;
	        ToPlayer = plyPos - ownPos;
	        //Checking if player is in sight
	        if (ply &&
                !IsRemembering &&
	            plyInView)
	        {
                /* 
                 * First off, if the target is behind us and we aren't 
                 * allowed to target people behind us, dont do it.
                 */
                bool canTarget = !(IsTargetBehind && !App.M.TargetBehind);

                //If we're dead dont even bother targeting us.
	            if (ply.M.ActionController.HealthController.IsDead)
	            {
	                canTarget = false;
	                newTarget = null;
	            }

	            newTarget = canTarget ? ply : newTarget;
	        }
	        else if (!IsRemembering)
	            newTarget = null;

	        //Check if we can see the player
	        if (newTarget &&
	            !App.M.HasWallHack &&
	            Physics2D.RaycastNonAlloc(ownPos, ToPlayer.normalized, viewResults, Mathf.Clamp(ToPlayer.magnitude, 0, viewBoxMaxLength), LayerMask.GetMask("Platform")) > 0)
	        {
                //We cant see the player, lose interest.
	            newTarget = null;
	        }

	        Target = newTarget;

            //If target is behind the enemy and is targeted and we're arent turning, turn around.
            if (!IsTurning && IsTargetBehind && Target)
	            Turn(-1 * App.M.Character.LookDirection);

            /**
             * Update state stuff
             */
            foreach (EnemyState enemyState in _states)
            {
                if (enemyState.enabled && enemyState.ShouldTakeover())
                {
                    if (StateMachine.CurrentState as EnemyState != enemyState)
                        StateMachine.ChangeState(enemyState);

                    break;
                }
            }

	        StateMachine.Update(BetterTime.DeltaTime);
        }


	    /// <summary>
        /// Method used to turn the enemy around.
        /// If 0, it turns around.
        /// </summary>
	    public void Turn(int dir, bool flip = false)
        {
            if (IsState<EnemyAttack>() || IsStagging) //Dont turn if we're attacking.
                return;

            if (dir == 0 && flip)
                dir = -1 * App.M.Character.LookDirection;

            if (IsTurning || 
                dir == App.M.Character.LookDirection || 
                dir == 0 && !flip || 
                App.M.Character.HealthController.IsDead)
                return;

            App.M.Character.StandStill();

            //Turn around instantly if turn speed is 0.
            if (App.M.TurnSpeed == 0)
            {
                App.M.Character.Flip(dir);
                return;
            }

            IsTurning = true;
            TellMeWhen.CancelScaled(this, EVENT_TURN);
            TellMeWhen.Seconds(App.M.TurnSpeed, this, EVENT_TURN);

            _whereToTurnTo = dir;
        }

	    public void StopTurn()
	    {
	        IsTurning = false;
            TellMeWhen.CancelScaled(this, EVENT_TURN);
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
	        if (!IsTurning && !IsStagging)
	        {
	            Vector2 vel = dir;

                if (!overrideYVel)
                    vel = new Vector2(dir.x, App.M.Character.Rigidbody.velocity.y);

	            App.M.Character.SetVelocity(vel * BetterTime.FixedDeltaTime, true, Target ? App.M.EngageSpeed : 0, overrideYVel);

	            int xDir = Mathf.RoundToInt(dir.x);
                if (!App.M.CanBackPaddle || forceTurn)
	                Turn(xDir);
	        }
	    }

	    public bool IsState<T>()
	    {
	        return StateMachine.CurrentState is T;
	    }

        /// <summary>
        /// If it can, remember the player in x duration with what is given in model.
        /// </summary>
	    public void Remember()
	    {
	        //If the enemy can remember the player, do so.
	        if (App.M.MemoryDuration != 0)
	        {
	            _target = GameManager.Instance.Player;

                if (App.M.NeverForget)
                    return;

	            IsRemembering = true;
	            TellMeWhen.CancelScaled(this, EVENT_MEMORY);
	            TellMeWhen.Seconds(App.M.MemoryDuration, this, EVENT_MEMORY);
	        }
        }

	    private void OnDamage(Character from)
	    {
            Remember();

            //For the rest of OnDamage code, if we're in attack state dont do anything.
            if (IsState<EnemyAttack>())
                return;

	        if (App.M.TurnOnBackstab && IsTargetBehind)
	            Turn();

            float staggingDuration = App.M.StaggerDuration;

            if (staggingDuration != 0 && !IsStagging)
	        {
                StopTurn();

	            IsStagging = true;

                TellMeWhen.CancelScaled(this, EVENT_STAGGING);
	            TellMeWhen.Seconds(staggingDuration, this, EVENT_STAGGING);

                App.M.Character.StandStill();

                if (App.M.StaggerIndicator)
                    App.M.StaggerIndicator.ShowIndicator(.1f);
            }
	    }

	    private void OnDead()
	    {
	        IsTurning = false;

            if(App.M.AttackIndicator.Show)
                App.M.AttackIndicator.HideIndicator(.1f);

	        if (App.M.StaggerIndicator)
	            App.M.StaggerIndicator.HideIndicator(.1f);

            if (GameManager.Instance != null)
	            GameManager.Instance.EnemiesChange.Invoke();

            App.M.Character.SetSortingLayer(GSManager.Instance.CorpsesSortingLayerID);
        }

	    public void OnTimesUp(int id, object args)
	    {
            if (!this)
                return;

	        switch (id)
	        {
	            case EVENT_TURN:
	                App.M.Character.Flip(_whereToTurnTo);
	                IsTurning = false;
	                break;
	            case EVENT_STAGGING:
	                IsStagging = false;

                    if (App.M.StaggerIndicator)
                        App.M.StaggerIndicator.HideIndicator(.1f);
	                break;
	            case EVENT_MEMORY:
	                _target = null;
	                IsRemembering = false;
	                break;
	        }
	    }
	}
}