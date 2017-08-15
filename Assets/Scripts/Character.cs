using System.Linq;
using Health;
using UnityEngine;

namespace Controllers
{
    public enum CharacterState
    {
        None, Idle, Moving, InAir
    }

    public enum LookDirection
    {
        Right,
        Left
    }

    /// <summary>
    /// Purpose: Base class for all characters, enemies, player etc.
    /// Creator: MB
    /// </summary>
    public class Character : MonoBehaviour
    {
        [Header("References:")]
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

        [Header("Settings:")]
        [SerializeField]
        private float _damage;

        [SerializeField]
        private bool _flipWithVelocity;

        [SerializeField, Tooltip("If on it will flip with rotation instead of scaling.")]
        private bool _doFlipWithRotation = true;

        public CharacterState State { get; set; }

        public LookDirection LookDirection { get; set; }

        /// <summary>
        /// Changes to which direction the controller bumped into an obstacle.
        /// -1 = Left
        /// 0 = None
        /// 1 = Right
        /// </summary>
        public int BumpingDirection { get; set; }

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

        public float Damage
        {
            get { return _damage; }
            set { _damage = value; }
        }

        public virtual void Update()
        {
            UpdateState();

            if (_flipWithVelocity && _rigidbody)
                Flip(Mathf.Round(_rigidbody.velocity.x));

            CheckSideBumping();
        }

        protected virtual void UpdateState()
        {
            if (OnGround && !_rigidbody.IsSleeping())
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

        public void Flip(float dir)
        {
            if (dir == 0)
                return;

            if (_doFlipWithRotation)
            {
                float rot = dir < 0 ? 180 : 0;
                _model.transform.rotation = Quaternion.Euler(0, rot, 0);
            }
            else
                _model.transform.localScale = new Vector2(dir < 0 ? -1 : 1, transform.localScale.y);

            LookDirection = dir < 0 ? LookDirection.Left : LookDirection.Right;
        }

        public virtual void AddVelocity(Vector2 velocity)
        {
            _rigidbody.velocity += velocity;
        }

        public virtual void SetVelocity(Vector2 velocity)
        {
            _rigidbody.velocity = velocity;
        }

        /// <summary>
        /// Checks if the controller is bumping into a wall or is at the end of a platform.
        /// Usage. Use the property.
        /// </summary>
        public void CheckSideBumping()
        {
            if (!PhysicialCollisionCheck || !PhysicialCollisionCheck.CollidersToCheck.Any())
            {
                Debug.LogWarning("CheckSideBumping requires physical collider reference to function properly.", transform);
                return;
            }

            Collider2D collider = PhysicialCollisionCheck.CollidersToCheck.FirstOrDefault();

            //Offsets for raycasts
            Vector2 xOffset = new Vector2(0.01f, 0);
            Vector2 yOffset = new Vector2(0, 0.01f);

            Vector2 pos = collider.transform.position;
            Vector2 extents = collider.bounds.extents;
            Vector2 origin = new Vector2(pos.x, pos.y + collider.bounds.size.y / 2 + collider.offset.y); //Root origin for raycasts

            //Calculating needed direction + length & left and right origins.
            Vector2 direction = Vector2.down * extents.y * 2 - yOffset;
            Vector2 leftOrigin = origin - xOffset - new Vector2(extents.x, 0);
            Vector2 rightOrigin = origin + xOffset + new Vector2(extents.x, 0);

            //Raycasting to check if we're near end of platform.
            bool leftHit = Physics2D.RaycastAll(leftOrigin, direction, LayerMask.GetMask("Platform")).Any();
            bool rightHit = Physics2D.RaycastAll(rightOrigin, direction, LayerMask.GetMask("Platform")).Any();

            bool leftBump = false;
            bool rightBump = false;

            //If we cant find physical collider checker we log it and move on.
            if (PhysicialCollisionCheck)
            {
                leftBump = PhysicialCollisionCheck.Left;
                rightBump = PhysicialCollisionCheck.Right;
            }
            else
                Debug.LogWarning("CheckSideBumping requires physical collision check to function properly.", transform);

            if (!leftHit || leftBump)
                BumpingDirection = -1;
            else if (!rightHit || rightBump)
                BumpingDirection = 1;
            else
                BumpingDirection = 0;
        }
    }
}