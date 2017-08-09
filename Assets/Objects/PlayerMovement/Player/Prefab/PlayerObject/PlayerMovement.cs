using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;
using AcrylecSkeleton.ModificationSystem;

namespace CharacterController
{
    public class PlayerMovement : CharacterController
    {
        [SerializeField]
        private WallJump _wallJump;

        [SerializeField]
        private DoubleJump _doubleJump;

        [SerializeField]
        private WallSlide _wallSlide;

        [SerializeField]
        private LedgeHanging _ledgeHanging;

        [Header("Component References"),SerializeField]
        private PlayerApplication _app;

        [SerializeField]
        private ModificationHandler _modificationHandler;

        [SerializeField]
        private CollisionCheck _triggerCheck;

        [SerializeField]
        private float _horizontalSpeed;

        [SerializeField]
        private float _jumpForce;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private float _maxFallSpeed;

        [SerializeField]
        private float _dashCooldown;

        private float _dashForce;

        private Vector2 _velocity;
        private CollisionSides _collisionSides;
        private CollisionSides _triggerSides;
        private bool _shouldDash;
        private float _dashTimer;
        private Vector2 _savedVelocity;
        private bool _shouldHang;

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
        public PlayerApplication App
        {
            get { return _app; }
            set { _app = value; }
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

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            HandleState();
            TriggerCheck.IsColliding(out _triggerSides);
            CollisionCheck.IsColliding(out _collisionSides);

            _shouldHang = LedgeHanging && LedgeHanging.VerticalActive;

            if (App.C.PlayerActions != null && App.C.PlayerActions.Dash.WasPressed && _dashTimer <= 0)
                _shouldDash = true;

            if (_dashTimer > 0)
                _dashTimer -= Time.deltaTime;
        }

        void FixedUpdate()
        {
            _velocity = new Vector2(0,0);

            HandleVerticalMovement(ref _velocity);
            HandleHorizontalMovement(ref _velocity);

            SetVelocity(new Vector2(_velocity.x * Time.fixedDeltaTime, _rigidbody.velocity.y));
            if(Velocity.y != 0)
                SetVelocity(new Vector2(_rigidbody.velocity.x, _velocity.y * Time.fixedDeltaTime));
        }

        void LateUpdate()
        {
            
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x,
                Mathf.Clamp(Rigidbody.velocity.y, -_maxFallSpeed, float.MaxValue));          
        }

        private void HandleState()
        {
            switch (State)
            {
                case CharacterState.Idle:
                    _animator.SetInteger("State", 0);
                    break;
                case CharacterState.Moving:
                    _animator.SetInteger("State", 1);
                    break;
                case CharacterState.InAir:
                    _animator.SetInteger("State", 2);
                    break;
                case CharacterState.OnWall:
                    _animator.SetInteger("State", 2);
                    break;
                case CharacterState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleVerticalMovement(ref Vector2 velocity)
        {
            if (_shouldDash)
            {
                _dashTimer = _dashCooldown;
                _shouldDash = false;

            }
            else if (WallJump && WallJump.VerticalActive)
            {
                WallJump.HandleVertical(ref velocity);
            }
            else if (DoubleJump && DoubleJump.VerticalActive)
            {
                DoubleJump.HandleVertical(ref velocity);
            }
            else if (App.C.PlayerActions != null && App.C.PlayerActions.Jump.WasPressed && TriggerSides.Bottom && Sides.Bottom)
            {
                List<Collider2D> col = Sides.BottomColliders.FindAll(x => x.gameObject.tag == "OneWayCollider").ToList();
                velocity += new Vector2(0, _jumpForce);

                if (col.Count > 0 && App.C.PlayerActions.Down.IsPressed)
                {
                    velocity = new Vector2(velocity.x,0);
                    foreach (var c in col)
                    {
                        _modificationHandler.AddModification(new TemporaryLayerChange(0.4f, "ChangeLayerOf" + c.gameObject.name, "NonPlayerCollision", c.gameObject));
                    }
                }
            }
            else if (_shouldHang)
            {
                LedgeHanging.HandleHorizontal(ref velocity);
            }
            else if (_wallSlide && _wallSlide.VerticalActive)
            {
                WallSlide.HandleVertical(ref velocity);
            }
            else
                velocity = new Vector2(velocity.x, 0);

        }


        private void HandleHorizontalMovement(ref Vector2 velocity)
        {
            var horizontal = App.C.PlayerActions.HorizontalDirection.RawValue;

            if (Sides.Left && horizontal < 0)
                horizontal = 0;

            if (Sides.Right && horizontal > 0)
                horizontal = 0;

            velocity += new Vector2(_horizontalSpeed * horizontal, 0);

            if (WallJump && WallJump.HorizontalActive)
            {
                WallJump.HandleHorizontal(ref velocity);
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