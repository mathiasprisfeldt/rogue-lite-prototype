using Abilitys;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;
using AcrylecSkeleton.ModificationSystem;
using Archon.SwissArmyLib.Utils;
using Controllers;

namespace CharacterController
{
    public enum MoveAbility
    {
        None, DoubleJump, WallJump, Wallslide, LedgeHanging, Dash, Jump, Climbing
    }

    public enum CombatAbility
    {
        None, Melee, Throw, DownMelee
    }

    public class ActionsController : Character
    {
        [Header("Component References"), SerializeField]
        private PlayerApplication _app;

        [SerializeField]
        private AbilityReferences _abilityReferences;

        [SerializeField]
        private ModificationHandler _modificationHandler;

        [SerializeField]
        private CollisionCheck _triggerCheck;

        [SerializeField]
        private CollisionCheck _nonPlatformTriggerCheck;

        [SerializeField]
        private CollisionCheck _collisionCheck;

        [SerializeField]
        private CollisionCheck _wallSlideCheck;

        [SerializeField]
        private CollisionCheck _onewayCheck;

        [SerializeField]
        private CollisionCheck _downCheck;

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
        private MoveAbility _lastUsedVerticalMoveAbility;
        private MoveAbility _lastUsedHorizontalMoveAbility;
        private CombatAbility _lastUsedCombatAbility;
        private MoveAbility _savedValue;
        private bool _dashEnded;

        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
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

        public GameObject Model
        {
            get { return _model; }
        }

        public Animator Animator
        {
            get { return _animator; }
            set { _animator = value; }
        }

        public MoveAbility LastUsedHorizontalMoveAbility
        {
            get { return _lastUsedHorizontalMoveAbility; }
            set { _lastUsedHorizontalMoveAbility = value; }
        }

        public MoveAbility LastUsedVerticalMoveAbility
        {
            get { return _lastUsedVerticalMoveAbility; }
            set { _lastUsedVerticalMoveAbility = value; }
        }

        public CombatAbility LastUsedCombatAbility
        {
            get { return _lastUsedCombatAbility; }
            set { _lastUsedCombatAbility = value; }
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
        public bool ClimbEnd { get; set; }
        public Trigger StartJump { get; set; }
        public Trigger StartDash { get; set; }
        public Trigger StartGrab { get; set; }
        public Trigger StartCombat { get; set; }
        public Trigger StartThrow { get; set; }
        public Trigger StartMelee { get; set; }
        public Trigger StartClimbing { get; set; }
        public Trigger StartDownMeele { get; set; }

        public float LastHorizontalDirection { get; set; }
        public float Horizontal { get; set; }
        public float Vertical { get; set; }
        public float InAirTImer { get; set; }

        public AbilityReferences AbilityReferences
        {
            get { return _abilityReferences; }
            set { _abilityReferences = value; }
        }

        public CollisionCheck NonPlatformTriggerCheck
        {
            get { return _nonPlatformTriggerCheck; }
            set { _nonPlatformTriggerCheck = value; }
        }

        public CollisionCheck OnewayCheck
        {
            get { return _onewayCheck; }
            set { _onewayCheck = value; }
        }

        public CollisionCheck DownCheck
        {
            get { return _downCheck; }
            set { _downCheck = value; }
        }


        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            _animator.SetFloat("Health", HealthController.HealthAmount);

            if (App.C.PlayerActions != null)
                App.C.PlayerActions.UpdateProxy();
            if(LastUsedVerticalMoveAbility != _savedValue)
                Debug.Log(LastUsedVerticalMoveAbility);
            _savedValue = LastUsedVerticalMoveAbility;
            
        }

        protected override void Awake()
        {
            base.Awake();
            LastHorizontalDirection = LookDirection;
            ClimbEnd = true;
            StartThrow = new Trigger();
            StartCombat = new Trigger();
            StartDash = new Trigger();
            StartGrab = new Trigger();
            StartJump = new Trigger();
            StartMelee = new Trigger();
            StartClimbing = new Trigger();
            StartDownMeele = new Trigger();
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

            x = _velocity.x * BetterTime.FixedDeltaTime;
            y = Velocity.y != 0 || _knockbackHandler.Active ? _velocity.y * BetterTime.FixedDeltaTime : Rigidbody.velocity.y;

            SetVelocity(new Vector2(x, y));

            if (_dashTimer > 0)
                _dashTimer -= BetterTime.FixedDeltaTime;

            HandleMaxSpeed();
            if (App.C.PlayerActions != null)
                App.C.PlayerActions.ResetProxy();
            _animator.SetBool("MovingUp", Rigidbody.velocity.y > 0);
            _animator.SetFloat("Speed", Mathf.Clamp01(new Vector2(Horizontal, Vertical).magnitude));

        }

        private bool HandleOnewayColliders()
        {
            var collisionHappened = false;

            if (GroundCollisionCheck.Sides.BottomColliders != null && GroundCollisionCheck.Sides.BottomColliders.Count > 0
            && App.C.PlayerActions.Down && App.C.PlayerActions.ProxyInputActions.Jump.WasPressed)
            {
                foreach (var c in GroundCollisionCheck.Sides.BottomColliders)
                {
                    if (c.gameObject.tag == "OneWayCollider")
                    {
                        ModificationHandler.AddModification(new TemporaryLayerChange("ChangeLayerOf" + c.gameObject.name, "NonPlayerCollision", c, OnewayCheck, 0));
                        collisionHappened = true;
                    }

                }
            }
            return collisionHappened;
        }

        private void HandleMaxSpeed()
        {
            var predictGravity = Rigidbody.velocity.y + Physics2D.gravity.y * Rigidbody.gravityScale;
            if (predictGravity <= -_maxFallSpeed)
            {
                Rigidbody.velocity -= new Vector2(0, Rigidbody.CounterGravity(-Mathf.Abs(predictGravity - _maxFallSpeed)) * BetterTime.FixedDeltaTime);
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

            if (State == CharacterState.InAir)
                InAirTImer += BetterTime.DeltaTime;
            else if (InAirTImer > 0)
                InAirTImer = 0;
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
            Animator.SetBool("LedgeGrabbed", LastUsedVerticalMoveAbility == MoveAbility.LedgeHanging);

            //Onwall
            Animator.SetBool("OnWall", LastUsedVerticalMoveAbility == MoveAbility.Wallslide);

            //Climbing
            Animator.SetBool("Climbing", LastUsedVerticalMoveAbility == MoveAbility.Climbing);

            //Dash
            if (LastUsedHorizontalMoveAbility == MoveAbility.Dash && StartDash.Value && !Combat)
                Animator.SetTrigger("Dash");


            //Melee
            if (StartMelee.Value)
                Animator.SetTrigger("Melee");


            //Throw
            if (StartThrow.Value)
                Animator.SetTrigger("Throw");

            //Start Jump
            if (StartJump.Value)
                Animator.SetTrigger("Jump");

            //GrabLedge
            if (StartGrab.Value)
                Animator.SetTrigger("GrabLedge");

            //GrabLedge
            if (StartCombat.Value)
                Animator.SetTrigger("Combat");

            //Climb
            if (StartClimbing.Value)
                Animator.SetTrigger("Climb");

            //SwordDown
            if (StartDownMeele.Value)
                Animator.SetTrigger("SwordDown");

        }

        private void HandleCombat()
        {
            if (_abilityReferences.Throw && _abilityReferences.Throw.KnifeActive)
            {
                BeginCombat(CombatAbility.Throw);
            }
            else if (_abilityReferences.DownMeele && _abilityReferences.DownMeele.Active)
            {
                BeginCombat(CombatAbility.DownMelee);
            }
            else if (_abilityReferences.Melee && _abilityReferences.Melee.Active)
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
                StartCombat.Value = true;
                LastUsedCombatAbility = combatAbility;

            }

            Combat = true;

        }

        private void HandleVerticalMovement(ref Vector2 velocity)
        {
            Vertical = App.C.PlayerActions.Vertical;
            LastUsedVerticalMoveAbility = MoveAbility.None;

            if (!HandleOnewayColliders())
            {
                if (_abilityReferences.LedgeHanging && _abilityReferences.LedgeHanging.VerticalActive)
                {
                    LastUsedVerticalMoveAbility = MoveAbility.LedgeHanging;
                    _abilityReferences.LedgeHanging.HandleVertical(ref velocity);
                }
                else if (_abilityReferences.WallJump && _abilityReferences.WallJump.VerticalActive &&
                    !(_abilityReferences.LedgeHanging && _abilityReferences.LedgeHanging.VerticalActive))
                {
                    LastUsedVerticalMoveAbility = MoveAbility.WallJump;
                    _abilityReferences.WallJump.HandleVertical(ref velocity);
                }
                else if (_abilityReferences.Jump && _abilityReferences.Jump.VerticalActive)
                {
                    _abilityReferences.Jump.HandleVertical(ref velocity);
                    LastUsedVerticalMoveAbility = MoveAbility.Jump;
                }
                else if (_abilityReferences.DoubleJump && _abilityReferences.DoubleJump.VerticalActive)
                {
                    LastUsedVerticalMoveAbility = MoveAbility.DoubleJump;
                    _abilityReferences.DoubleJump.HandleVertical(ref velocity);
                }
                else if (_abilityReferences.Dash && _abilityReferences.Dash.VerticalActive)
                {
                    LastUsedVerticalMoveAbility = MoveAbility.Dash;
                    _abilityReferences.Dash.HandleVertical(ref velocity);
                }
                else if (_abilityReferences.Climing.VerticalActive)
                {
                    LastUsedVerticalMoveAbility = MoveAbility.Climbing;
                    ClimbEnd = false;
                    Animator.SetBool("ClimbEnd", false);
                    _abilityReferences.Climing.HandleVertical(ref velocity);
                }
                else if (_abilityReferences.WallSlide && _abilityReferences.WallSlide.VerticalActive)
                {
                    LastUsedVerticalMoveAbility = MoveAbility.Wallslide;
                    _abilityReferences.WallSlide.HandleVertical(ref velocity);
                }
                else
                    velocity = new Vector2(velocity.x, 0);
            }
        }

        public override void Flip(float dir)
        {
            if (dir != 0)
                LastHorizontalDirection = dir > 0 ? 1 : -1;
            base.Flip(dir);
        }

        private void HandleHorizontalMovement(ref Vector2 velocity)
        {
            if (!_dashEnded && !(_abilityReferences.Dash && _abilityReferences.Dash.HorizontalActive))
                _dashEnded = true;

            LastUsedHorizontalMoveAbility = MoveAbility.None;
            Horizontal = App.C.PlayerActions.Horizontal;
            if (!Combat)
                Flip(Horizontal);

            if (CollisionCheck.Sides.Left && Horizontal < 0)
                Horizontal = 0;

            if (CollisionCheck.Sides.Right && Horizontal > 0)
                Horizontal = 0;

            velocity += new Vector2(MovementSpeed * Horizontal, 0);

            if (_abilityReferences.Dash && _abilityReferences.Dash.HorizontalActive)
            {
                LastUsedHorizontalMoveAbility = MoveAbility.Dash;
                _abilityReferences.Dash.HandleHorizontal(ref velocity);
            }
            else if (_abilityReferences.WallJump && _abilityReferences.WallJump.HorizontalActive
                && !(_abilityReferences.LedgeHanging && _abilityReferences.LedgeHanging.VerticalActive))
            {
                LastUsedHorizontalMoveAbility = MoveAbility.WallJump;
                _abilityReferences.WallJump.HandleHorizontal(ref velocity);
            }
            else if (_abilityReferences.Climing && _abilityReferences.Climing.HorizontalActive)
            {
                LastUsedHorizontalMoveAbility = MoveAbility.Climbing;
                _abilityReferences.Climing.HandleHorizontal(ref velocity);
            }
            else if (_abilityReferences.WallSlide && _abilityReferences.WallSlide.HorizontalActive)
            {
                LastUsedHorizontalMoveAbility = MoveAbility.Wallslide;
                _abilityReferences.WallSlide.HandleHorizontal(ref velocity);
            }
        }

    }

    public struct LastSprint
    {
        public bool Active { get; private set; }
        public bool IsLeft { get; private set; }

        public LastSprint(bool active, bool isLeft) : this()
        {
            Active = active;
            IsLeft = isLeft;
        }

        public void SetActive(bool active, bool isLeft)
        {
            Active = active;
            IsLeft = isLeft;
        }
    }

    public class TemporaryLayerChange : Modification
    {
        private LayerMask _targetLayer;
        private LayerMask _oldLayer;
        private Collider2D _targetCollider;
        private CollisionCheck _collisonCheck;
        private float _timer;

        public TemporaryLayerChange(string name, string targetLayer, Collider2D targetCollider, CollisionCheck collisionCheck, float time) : base(name)
        {
            _timer = time;
            _targetLayer = LayerMask.NameToLayer(targetLayer);
            if (_targetLayer == -1)
            {
                Debug.LogWarning(targetLayer + " layer dose not exist!");
                ModificationHandler.RemoveModification(this);
            }
            else
            {
                _collisonCheck = collisionCheck;
                _oldLayer = targetCollider.gameObject.layer;
                targetCollider.gameObject.layer = _targetLayer;
                _targetCollider = targetCollider;
            }
        }

        public override void ApplyModificaiton()
        {
        }

        public override void RemoveModificaiton()
        {
            _targetCollider.gameObject.layer = _oldLayer;
        }

        public override void UpdateModificaiton()
        {
            if (_timer > 0)
                _timer -= BetterTime.DeltaTime;
            if (!_collisonCheck.Sides.TargetColliders.Contains(_targetCollider) && _timer <= 0)
                ModificationHandler.RemoveModification(this);
        }

        public override void FixedUpdateModificaiton()
        {
        }
    }
}