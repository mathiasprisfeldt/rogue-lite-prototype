using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;
using AcrylecSkeleton.ModificationSystem;
using Combat;
using Controllers;
using Special;

namespace CharacterController
{
    public enum Ability
    {
        None, DoubleJump, WallJump, Wallslide, LedgeHanging, Dash, Jump
    }

    public enum CombatAbility
    {
        None, Melee, Throw
    }

    public class ActionsController : Character
    {
        [Header("Component References"), SerializeField]
        private PlayerApplication _app;

        [SerializeField]
        private WallJump _wallJump;

        [SerializeField]
        private DoubleJump _doubleJump;

        [SerializeField]
        private WallSlide _wallSlide;

        [SerializeField]
        private LedgeHanging _ledgeHanging;

        [SerializeField]
        private Dash _dash;

        [SerializeField]
        private Jump _jump;

        [SerializeField]
        private Melee _melee;

        [SerializeField]
        private ThrowProjectile _throwProjectile;

        [SerializeField]
        private ModificationHandler _modificationHandler;

        [SerializeField]
        private CollisionCheck _triggerCheck;

        [SerializeField]
        private CollisionCheck _collisionCheck;

        [SerializeField]
        private CollisionCheck _wallSlideCheck;

        [SerializeField]
        private float _horizontalSpeed;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private float _maxFallSpeed;

        [SerializeField]
        private Transform _throwPoint;

        private Vector2 _velocity;

        private bool _shouldDash;
        private float _dashTimer;
        private Vector2 _savedVelocity;
        private bool _shouldHang;
        private Ability _lastUsedVerticalAbility;
        private Ability _lastUsedHorizontalAbility;
        private CombatAbility _lastUsedCombatAbility;
        private bool _dashEnded;


        public WallJump WallJump
        {
            get { return _wallJump; }
            set { _wallJump = value; }
        }

        public DoubleJump DoubleJump
        {
            get { return _doubleJump; }
            set { _doubleJump = value; }
        }

        public WallSlide WallSlide
        {
            get { return _wallSlide; }
            set { _wallSlide = value; }
        }

        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public LedgeHanging LedgeHanging
        {
            get { return _ledgeHanging; }
            set { _ledgeHanging = value; }
        }

        public CollisionCheck TriggerCheck
        {
            get { return _triggerCheck; }
            set { _triggerCheck = value; }
        }

        public PlayerApplication App
        {
            get { return _app; }
            set { _app = value; }
        }

        public Dash Dash
        {
            get { return _dash; }
            set { _dash = value; }
        }

        public GameObject Model
        {
            get { return _model; }
        }

        public Animator Animator
        {
            get { return _animator; }
            set { _animator = value; }
        }

        public Ability LastUsedHorizontalAbility
        {
            get { return _lastUsedHorizontalAbility; }
            set { _lastUsedHorizontalAbility = value; }
        }

        public Ability LastUsedVerticalAbility
        {
            get { return _lastUsedVerticalAbility; }
            set { _lastUsedVerticalAbility = value; }
        }

        public CombatAbility LastUsedCombatAbility
        {
            get { return _lastUsedCombatAbility; }
            set { _lastUsedCombatAbility = value; }
        }

        public Jump Jump
        {
            get { return _jump; }
            set { _jump = value; }
        }

        public ModificationHandler ModificationHandler
        {
            get { return _modificationHandler; }
            set { _modificationHandler = value; }
        }

        public CollisionCheck CollisionCheck
        {
            get { return _collisionCheck; }
            set { _collisionCheck = value; }
        }

        public CollisionCheck WallSlideCheck
        {
            get { return _wallSlideCheck; }
        }

        public Transform ThrowPoint
        {
            get { return _throwPoint; }
            set { _throwPoint = value; }
        }

        public bool Combat { get; set; }
        public bool StartJump { get; set; }
        public bool StartDash { get; set; }
        public bool StartGrab { get; set; }
        public bool StartCombat { get; set; }
        public bool StartThrow { get; set; }
        public bool StartMelee{ get; set; }

        public float LastHorizontalDirection { get; set; }


        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            if (App.C.PlayerActions != null)
                App.C.PlayerActions.UpdateProxy();
        }

        public void Awake()
        {
            LastHorizontalDirection = 1;
        }

        void FixedUpdate()
        {
            _velocity = new Vector2(0, 0);

            HandleCombat();
            HandleHorizontalMovement(ref _velocity);
            HandleVerticalMovement(ref _velocity);  
            HandleAnimationParameters();

            float y = 0;
            float x = 0;

            x = _velocity.x*Time.fixedDeltaTime;
            y = Velocity.y != 0 || _knockbackHandler.Active ? _velocity.y*Time.fixedDeltaTime : Rigidbody.velocity.y;

            SetVelocity(new Vector2(x,y ));                

            if (_dashTimer > 0)
                _dashTimer -= Time.fixedDeltaTime;

            HandleMaxSpeed();
            if (App.C.PlayerActions != null)
                App.C.PlayerActions.ResetProxy();
        }

        private void HandleMaxSpeed()
        {
            var predictGravity = Rigidbody.velocity.y + Physics2D.gravity.y*Rigidbody.gravityScale;
            if (predictGravity <= -_maxFallSpeed)
            {
                Rigidbody.velocity -= new Vector2(0,Rigidbody.CounterGravity(-Mathf.Abs(predictGravity - _maxFallSpeed))*Time.fixedDeltaTime);

            }
        }


        protected override void UpdateState()
        {
            if (OnGround && (App.C.PlayerActions != null && App.C.PlayerActions.Horizontal != 0))
                State = CharacterState.Moving;
            else if (OnGround)
                State = CharacterState.Idle;
            else
                State = CharacterState.InAir;
        }


        private void HandleAnimationParameters()
        {
            //Running
            Animator.SetBool("Running", State == CharacterState.Moving);

            //OnGround
            Animator.SetBool("OnGround", OnGround);

            //Combat
            Animator.SetBool("InCombat", Combat);

            //LedgeGrabbed
            Animator.SetBool("LedgeGrabbed", LastUsedVerticalAbility == Ability.LedgeHanging);

            //Onwall
            Animator.SetBool("OnWall", LastUsedVerticalAbility == Ability.Wallslide);

            //Dash
            if (LastUsedHorizontalAbility == Ability.Dash && StartDash && !Combat)
            {
                StartDash = false;
                Animator.SetTrigger("Dash");
            }
             

            //Melee
            if (StartMelee)
            {
                Animator.SetTrigger("Melee");
                StartMelee = false;
            }
                

            //Throw
            if (StartThrow)
            {
                Animator.SetTrigger("Throw");
                StartThrow = false;
            }

            //Start Jump
            if (StartJump)
            {
                StartJump = false;
                Animator.SetTrigger("Jump");
            }

            //GrabLedge
            if (StartGrab)
            {
                StartGrab = false;
                Animator.SetTrigger("GrabLedge");
            }

            //GrabLedge
            if (StartCombat)
            {
                StartCombat = false;
                Animator.SetTrigger("Combat");
            }

        }

        private void HandleCombat()
        {
            if (_throwProjectile && _throwProjectile.Active)
            {
                BeginCombat(CombatAbility.Throw);
            }
            else if (_melee && _melee.Active)
            {
                BeginCombat(CombatAbility.Melee);
            }
            else
            {
                LastUsedCombatAbility = CombatAbility.None;
                Combat = false;
            }
                
        }


        private void BeginCombat(CombatAbility combatAbility)
        {
            if (LastUsedCombatAbility != combatAbility)
            {
                StartCombat = true;
                LastUsedCombatAbility = combatAbility;
                
            }

            Combat = true;

        }

        private void HandleVerticalMovement(ref Vector2 velocity)
        {
            LastUsedVerticalAbility = Ability.None;
            List<Collider2D> col = new List<Collider2D>();
            if (CollisionCheck.Sides.BottomColliders != null)
                col = CollisionCheck.Sides.BottomColliders.FindAll(x => x.gameObject.tag == "OneWayCollider").ToList();

            if (LedgeHanging && LedgeHanging.VerticalActive)
            {
                LastUsedVerticalAbility = Ability.LedgeHanging;
                LedgeHanging.HandleVertical(ref velocity);
            }
            else if (Dash && Dash.VerticalActive)
            {
                LastUsedVerticalAbility = Ability.Dash;
                Dash.HandleVertical(ref velocity);
            }
            else if (WallJump && WallJump.VerticalActive &&
                     !(LedgeHanging && LedgeHanging.VerticalActive))
            {
                LastUsedVerticalAbility = Ability.WallJump;
                WallJump.HandleVertical(ref velocity);
            }
            else if (DoubleJump && DoubleJump.VerticalActive)
            {
                LastUsedVerticalAbility = Ability.DoubleJump;
                DoubleJump.HandleVertical(ref velocity);
            }
            else if (Jump && Jump.VerticalActive)
            {
                Jump.HandleVertical(ref velocity);
                LastUsedVerticalAbility = Ability.Jump;
            }
            else if (_wallSlide && _wallSlide.VerticalActive)
            {
                LastUsedVerticalAbility = Ability.Wallslide;
                WallSlide.HandleVertical(ref velocity);
            }
            else
                velocity = new Vector2(velocity.x, 0);



        }


        private void HandleHorizontalMovement(ref Vector2 velocity)
        {
            if (!_dashEnded && !(Dash && Dash.HorizontalActive))
                _dashEnded = true;

            LastUsedHorizontalAbility = Ability.None;
            var horizontal = App.C.PlayerActions.Horizontal;
            if (horizontal > 0)
                LastHorizontalDirection = 1;
            else if (horizontal < 0)
                LastHorizontalDirection = -1;
            Flip(horizontal);

            if (CollisionCheck.Sides.Left && horizontal < 0)
                horizontal = 0;

            if (CollisionCheck.Sides.Right && horizontal > 0)
                horizontal = 0;

            velocity += new Vector2(_horizontalSpeed*horizontal, 0);
            
            if (Dash && Dash.HorizontalActive)
            {
                LastUsedHorizontalAbility = Ability.Dash;
                Dash.HandleHorizontal(ref velocity);           
            }
            else if (WallJump && WallJump.HorizontalActive && !(LedgeHanging && LedgeHanging.VerticalActive))
            {
                LastUsedHorizontalAbility = Ability.WallJump;
                WallJump.HandleHorizontal(ref velocity);                
            }
            else if (WallSlide && WallSlide.HorizontalActive)
            {
                LastUsedHorizontalAbility = Ability.Wallslide;
                WallSlide.HandleHorizontal(ref velocity);
            }
        }

    }
}