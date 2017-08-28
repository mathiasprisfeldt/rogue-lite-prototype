﻿using System.Collections;
using System.Linq;
using Abilitys;
using Health;
using UnityEngine;
using Knockbacks;

namespace Controllers
{
    public enum CharacterState
    {
        None, Idle, Moving, InAir
    }

    /// <summary>
    /// Purpose: Base class for all characters, enemies, player etc.
    /// Creator: MB
    /// </summary>
    public class Character : MonoBehaviour
    {
        private static readonly float BUMPING_RAY_LENGTH = 0.05f;
        private static readonly Vector2 BUMPING_ORIGIN_OFFSET = new Vector2(.5f, 0);

        private Collider2D _bumpingCollider2D; //Collider used for any collisions when bumping.
        private readonly RaycastHit2D[] _bumpingResults = new RaycastHit2D[1]; //Used to store if the player bumped into something platformy

        [Header("References:")]
        [SerializeField]
        private Transform _origin;

        [SerializeField]
        private Animator _mainAnimator;

        [SerializeField]
        private HealthController _healthController;

        [SerializeField]
        protected Rigidbody2D _rigidbody;

        [SerializeField]
        protected GameObject _model;

        [SerializeField]
        private CollisionCheck _groundCollisionCheck;

        [SerializeField]
        private CollisionCheck _physicialCollisionCheck;

        [SerializeField]
        private CollisionCheck _hitbox;

        [SerializeField]
        public KnockbackHandler _knockbackHandler;

        [SerializeField]
        private AbilityHandler _abilityHandler;

        [Header("Settings:")]
        [SerializeField, Tooltip("Which way is it facing at start?")]
        private int _startDirection = -1;

        [SerializeField]
        private float _damage;

        [SerializeField]
        private float _movementSpeed;

        [SerializeField]
        private float _flySpeed;

        [SerializeField]
        private bool _flipWithVelocity;

        [SerializeField, Tooltip("If on it will flip with rotation instead of scaling.")]
        private bool _doFlipWithRotation = true;

        public CharacterState State { get; set; }

        /// <summary>
        /// A normalized value direction value.
        /// -1 = left
        /// 0 = none
        /// 1 = right
        /// </summary>
        public int LookDirection { get; set; }

        /// <summary>
        /// Changes to which direction the controller bumped into an obstacle.
        /// -1 = Left
        /// 0 = None
        /// 1 = Right
        /// </summary>
        public int BumpingDirection { get; set; }

        public bool LockMovement { get; set; }

        public Vector2 Origin
        {
            get { return _origin.position; }
        }

        public Animator MainAnimator
        {
            get { return _mainAnimator; }
            set { _mainAnimator = value; }
        }

        public HealthController HealthController
        {
            get { return _healthController; }
            set { _healthController = value; }
        }

        public virtual Rigidbody2D Rigidbody
        {
            get { return _rigidbody; }
            set { _rigidbody = value; }
        }

        public bool OnGround
        {
            get { return GroundCollisionCheck && GroundCollisionCheck.Bottom; }
        }

        public CollisionCheck GroundCollisionCheck
        {
            get { return _groundCollisionCheck; }
            set { _groundCollisionCheck = value; }
        }

        public CollisionCheck PhysicialCollisionCheck
        { 
            get { return _physicialCollisionCheck; }
            set { _physicialCollisionCheck = value; }
        }

        public CollisionCheck Hitbox
        {
            get { return _hitbox; }
            set { _hitbox = value; }
        }

        public KnockbackHandler KnockbackHandler
        {
            get { return _knockbackHandler; }
            set { _knockbackHandler = value; }
        }

        public float Damage
        {
            get { return _damage; }
            set { _damage = value; }
        }

        public float MovementSpeed
        {
            get { return _movementSpeed; }
            set { _movementSpeed = value; }
        }

        public float FlySpeed
        {
            get { return _flySpeed; }
            set { _flySpeed = value; }
        }

        public AbilityHandler AbilityHandler
        {
            get { return _abilityHandler; }
            set { _abilityHandler = value; }
        }

        public bool IsFlying
        {
            get { return _flySpeed > 0; }
        }

        protected virtual void Awake()
        {
            Flip(_startDirection);

            if (PhysicialCollisionCheck)
                _bumpingCollider2D = PhysicialCollisionCheck.CollidersToCheck.FirstOrDefault();
        }

        public virtual void Update()
        {
            UpdateState();

            if (_flipWithVelocity && _rigidbody)
                Flip(Mathf.Round(_rigidbody.velocity.x));

            //Update animation states
            if (MainAnimator)
            {
                MainAnimator.SetBool("OnGround", OnGround);
                MainAnimator.SetBool("Moving", State == CharacterState.Moving);
            }
        }

        void FixedUpdate()
        {
            CheckSideBumping();
        }

        protected virtual void UpdateState()
        {
            //TODO: Maybe find a better way to check if the character is moving or not.
            if (OnGround && _rigidbody.velocity != Vector2.zero)
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

        /// <summary>
        /// Flips the characters visual direction and look direction.
        /// </summary>
        /// <param name="dir"></param>
        public void Flip(Vector2 dir)
        {
            Flip(Mathf.RoundToInt(dir.normalized.x));
        }

        /// <summary>
        /// Flips the characters visual direction and look direction.
        /// </summary>
        /// <param name="dir"></param>
        public void Flip(float dir)
        {
            if (dir == 0 || dir == LookDirection)
                return;

            if (_doFlipWithRotation)
            {
                float rot = dir < 0 ? 180 : 0;
                _model.transform.rotation = Quaternion.Euler(0, rot, 0);
            }
            else
                _model.transform.localScale = new Vector2(dir < 0 ? -1 : 1, transform.localScale.y);

            LookDirection = Mathf.RoundToInt(dir);
        }

        public virtual void SetVelocity(Vector2 velocity, bool respectMovementSpeed = false, float movementSpeedAddtion = 0, bool fly = false)
        {
            if (_healthController.IsDead || LockMovement)
                return;

            if (respectMovementSpeed)
            {               
                velocity.x *= MovementSpeed + movementSpeedAddtion;

                if (fly)
                    velocity.y *= FlySpeed + movementSpeedAddtion;
            }

            _rigidbody.velocity = velocity;
        }

        /// <summary>
        /// Forces the character to stand still.
        /// </summary>
        public void StandStill()
        {
            Rigidbody.velocity = !IsFlying ? new Vector2(0, Rigidbody.velocity.y) : Vector2.zero;
        }

        /// <summary>
        /// Checks if the controller is bumping into a wall or is at the end of a platform.
        /// Usage. Use the property.
        /// </summary>
        public void CheckSideBumping()
        {
            if (!OnGround)
                return;

            if (!PhysicialCollisionCheck || !PhysicialCollisionCheck.CollidersToCheck.Any())
            {
                Debug.LogWarning("CheckSideBumping requires physical collider reference to function properly.", transform);
                return;
            }

            Bounds bounds = _bumpingCollider2D.bounds;
            
            Vector2 offset = BUMPING_ORIGIN_OFFSET + new Vector2(Mathf.Abs(Rigidbody.velocity.x), 0) * Time.fixedDeltaTime;

            //Calculating needed direction + length & left and right origins.
            Vector2 direction = Vector2.down;
            float length = BUMPING_RAY_LENGTH;
            Vector2 leftOrigin = (Vector2) bounds.min - offset;
            Vector2 rightOrigin = (Vector2) bounds.min + offset + new Vector2(bounds.size.x, 0);
            
            //Raycasting to check if we're near end of platform.
            bool leftHit = Physics2D.RaycastNonAlloc(leftOrigin, direction * length, _bumpingResults, length, LayerMask.GetMask("Platform")) != 0;
            bool rightHit = Physics2D.RaycastNonAlloc(rightOrigin, direction * length, _bumpingResults, length, LayerMask.GetMask("Platform")) != 0;

            //If we cant find physical collider checker we log it and move on.
            bool leftBump = PhysicialCollisionCheck.Left;
            bool rightBump = PhysicialCollisionCheck.Right;

            if (!leftHit || leftBump)
                BumpingDirection = -1;
            else if (!rightHit || rightBump)
                BumpingDirection = 1;
            else
                BumpingDirection = 0;
        }
    }
}