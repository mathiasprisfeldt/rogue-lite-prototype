﻿using System;
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


    public class PlayerMovement : CharacterController
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
        private ModificationHandler _modificationHandler;

        [SerializeField]
        private CollisionCheck _triggerCheck;

        [SerializeField]
        protected CollisionCheck _collisionCheck;

        public CollisionCheck CollisionCheck
        {
            get { return _collisionCheck; }
            set { _collisionCheck = value; }
        }

        [SerializeField]
        private float _horizontalSpeed;

        [SerializeField]
        private float _jumpForce;

        [SerializeField]
        private float _jumpDuration;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private float _maxFallSpeed;

        private float _dashForce;

        private Vector2 _velocity;
        private CollisionSides _collisionSides;
        private CollisionSides _triggerSides;
        private bool _shouldDash;
        private float _dashTimer;
        private Vector2 _savedVelocity;
        private bool _shouldHang;
        private float _jumpTimer;
        private Ability _lastUsedVerticalAbility;
        private Ability _lastUsedHorizontalAbility;
        private bool _dashEnded;

        public PlayerState PlayerState { get; set; }

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

        public CollisionSides Sides
        {
            get { return _collisionSides; }
            set { _collisionSides = value; }
        }

        public CollisionSides TriggerSides
        {
            get { return _triggerSides; }
            set { _triggerSides = value; }
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

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            if(App.C.PlayerActions != null && App.C.PlayerActions.Attack.WasPressed)
                Animator.SetTrigger("Attack");

            HandleState();
            TriggerCheck.IsColliding(out _triggerSides);
            CollisionCheck.IsColliding(out _collisionSides);
            if(App.C.PlayerActions != null)
                _shouldHang = LedgeHanging && LedgeHanging.VerticalActive;
        }

        void FixedUpdate()
        {
            _velocity = new Vector2(0,0);

            if (App.C.PlayerActions != null && (App.C.PlayerActions.Left || App.C.PlayerActions.Right))
            {
                float dir = App.C.PlayerActions.Left ? -1 : 1;
                Flip(dir);
            }

            HandleVerticalMovement(ref _velocity);
            HandleHorizontalMovement(ref _velocity);

            SetVelocity(new Vector2(_velocity.x * Time.fixedDeltaTime, _rigidbody.velocity.y));
            if(Velocity.y != 0)
                SetVelocity(new Vector2(_rigidbody.velocity.x, _velocity.y * Time.fixedDeltaTime));

            if (_jumpTimer > 0)
                _jumpTimer -= Time.fixedDeltaTime;

            if (_dashTimer > 0)
                _dashTimer -= Time.fixedDeltaTime;

            var predictGravity = Rigidbody.velocity.y + Physics2D.gravity.y * Rigidbody.gravityScale;
            if (predictGravity <= -_maxFallSpeed)
            {
                Rigidbody.velocity -= new Vector2(0,Rigidbody.CounterGravity(-Mathf.Abs(predictGravity - _maxFallSpeed)) * Time.fixedDeltaTime);
            }

        }

        void LateUpdate()
        {
            
            

        }

        protected override void UpdateState()
        {
            if (OnGround && (App.C.PlayerActions != null && (App.C.PlayerActions.Left.IsPressed || App.C.PlayerActions.Right.IsPressed)))
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
        
            if (LastUsedVerticalAbility == Ability.LedgeHanging)
                Animator.SetInteger("State", 3);
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
            if(Sides.BottomColliders != null)
                col = Sides.BottomColliders.FindAll(x => x.gameObject.tag == "OneWayCollider").ToList();
                if (!(Dash && Dash.HorizontalActive))
            {
                if (_shouldHang)
                {
                    LastUsedVerticalAbility = Ability.LedgeHanging;
                    LedgeHanging.HandleVertical(ref velocity);                    
                }
                else if (WallJump && WallJump.VerticalActive && !App.C.PlayerActions.Down.IsPressed && !(LedgeHanging && LedgeHanging.VerticalActive))
                {
                    LastUsedVerticalAbility = Ability.WallJump;
                    WallJump.HandleVertical(ref velocity);                    
                }
                else if (DoubleJump && DoubleJump.VerticalActive && !App.C.PlayerActions.Down.IsPressed )
                {
                    LastUsedVerticalAbility = Ability.DoubleJump;
                    DoubleJump.HandleVertical(ref velocity);                    
                }
                else if ((App.C.PlayerActions != null && App.C.PlayerActions.Jump.WasPressed && Sides.Bottom &&
                    !App.C.PlayerActions.Down.IsPressed) || _jumpTimer > 0 
                    || (col.Count > 0 && App.C.PlayerActions != null && App.C.PlayerActions.Down.IsPressed && App.C.PlayerActions.Jump.WasPressed))
                {
                    if (_jumpTimer <= 0)
                        _jumpTimer = _jumpDuration;
                    velocity = new Vector2(0, Rigidbody.CalculateVerticalSpeed(_jumpForce / _jumpDuration));

                    if (col.Count > 0 && App.C.PlayerActions.Down.IsPressed)
                    {
                        velocity = new Vector2(velocity.x, 0);
                        foreach (var c in col)
                        {
                            _modificationHandler.AddModification(new TemporaryLayerChange(0.4f, "ChangeLayerOf" + c.gameObject.name, "NonPlayerCollision", c.gameObject));
                        }
                    }
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
            

        }


        private void HandleHorizontalMovement(ref Vector2 velocity)
        {
            if (!_dashEnded && !(Dash && Dash.HorizontalActive))
                _dashEnded = true;

            LastUsedHorizontalAbility = Ability.None;
            var horizontal = App.C.PlayerActions.HorizontalDirection.RawValue;

            if (Sides.Left && horizontal < 0)
                horizontal = 0;

            if (Sides.Right && horizontal > 0)
                horizontal = 0;

            velocity += new Vector2(_horizontalSpeed * horizontal, 0);


            if (Dash && Dash.HorizontalActive)
            {
                Dash.HandleHorizontal(ref velocity);
                LastUsedHorizontalAbility = Ability.Dash;
            }
            else if (WallJump && WallJump.HorizontalActive && !(LedgeHanging && LedgeHanging.VerticalActive))
            {
                WallJump.HandleHorizontal(ref velocity);
                LastUsedHorizontalAbility = Ability.WallJump;
            }
        }


        public class TemporaryLayerChange : Modification
        {
            private LayerMask _targetLayer;
            private LayerMask _oldLayer;
            private GameObject _targetObject;

            public TemporaryLayerChange(float time, string name, string targetLayer, GameObject targetObject) : base(time, name)
            {
                _targetLayer = LayerMask.NameToLayer(targetLayer);
                if (_targetLayer == -1)
                {
                    Debug.LogWarning(targetLayer + " layer dose not exist!");
                    RemoveModificaiton();
                }

                else
                {
                    _oldLayer = targetObject.layer;
                    targetObject.layer = _targetLayer;
                    _targetObject = targetObject;
                }
            }

            public override void ApplyModificaiton()
            {
            }

            public override void RemoveModificaiton()
            {
                if (_targetObject)
                    _targetObject.layer = _oldLayer;
            }

            public override void UpdateModificaiton()
            {
            }

            public override void FixedUpdateModificaiton()
            {
            }
        }
    }
}