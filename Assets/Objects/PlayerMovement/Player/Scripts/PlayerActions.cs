using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;
using AcrylecSkeleton.ModificationSystem;

namespace CharacterController
{
    public enum Ability
    {
        None, DoubleJump, WallJump, Wallslide, LedgeHanging, Dash, Jump
    }


    public class PlayerActions : CharacterController
    {

        [Header("Component References"), SerializeField] private PlayerApplication _app;

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

        private Vector2 _velocity;

        private bool _shouldDash;
        private float _dashTimer;
        private Vector2 _savedVelocity;
        private bool _shouldHang;
        private Ability _lastUsedVerticalAbility;
        private Ability _lastUsedHorizontalAbility;
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


        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            HandleState();
            if (App.C.PlayerActions != null)
                _shouldHang = LedgeHanging && LedgeHanging.VerticalActive;
        }

        void FixedUpdate()
        {
            _velocity = new Vector2(0, 0);

            HandleHorizontalMovement(ref _velocity);
            HandleVerticalMovement(ref _velocity);           

            SetVelocity(new Vector2(_velocity.x*Time.fixedDeltaTime, _rigidbody.velocity.y));
            if (Velocity.y != 0)
                SetVelocity(new Vector2(_rigidbody.velocity.x, _velocity.y*Time.fixedDeltaTime));

            if (_dashTimer > 0)
                _dashTimer -= Time.fixedDeltaTime;

            var predictGravity = Rigidbody.velocity.y + Physics2D.gravity.y*Rigidbody.gravityScale;
            if (predictGravity <= -_maxFallSpeed)
            {
                Rigidbody.velocity -= new Vector2(0,
                    Rigidbody.CounterGravity(-Mathf.Abs(predictGravity - _maxFallSpeed))*Time.fixedDeltaTime);
            }

        }


        protected override void UpdateState()
        {
            if (OnGround && (App.C.PlayerActions != null && App.C.PlayerActions.Horizontal != 0))
            {
                State = CharacterState.Moving;
            }
            else if (OnGround)
            {
                State = CharacterState.Idle;
            }
            else
            {
                State = CharacterState.InAir;
            }


        }


        private void HandleState()
        {

            if (LastUsedHorizontalAbility == Ability.Dash)
                Animator.SetInteger("State", 5);
            else if (LastUsedVerticalAbility == Ability.LedgeHanging)
                Animator.SetInteger("State", 3);
            else if (LastUsedVerticalAbility == Ability.Wallslide)
                Animator.SetInteger("State", 4);
            else
            {
                switch (State)
                {
                    case CharacterState.Idle:
                        Animator.SetInteger("State", 0);
                        break;
                    case CharacterState.Moving:
                        Animator.SetInteger("State", 1);
                        break;
                    case CharacterState.InAir:
                        Animator.SetInteger("State", 2);
                           
                        break;
                    case CharacterState.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


        }

        private void HandleVerticalMovement(ref Vector2 velocity)
        {
            LastUsedVerticalAbility = Ability.None;
            List<Collider2D> col = new List<Collider2D>();
            if (CollisionCheck.Sides.BottomColliders != null)
                col = CollisionCheck.Sides.BottomColliders.FindAll(x => x.gameObject.tag == "OneWayCollider").ToList();
            if (_shouldHang)
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