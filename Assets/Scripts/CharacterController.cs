using System.Linq;
using UnityEngine;

namespace CharacterController
{
    public enum CharacterState
    {
        None, Idle, Moving, InAir
    }

    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [ExecuteInEditMode]
    public class CharacterController : MonoBehaviour
    {
        private bool _runnedSetup;

        [Header("Auto Setup"), SerializeField]
        private bool _setup;

        [SerializeField]
        private bool _flipWithVelocity;

        [SerializeField]
        protected Rigidbody2D _rigidbody;

        [SerializeField]
        protected GameObject _model;

        [SerializeField]
        private CollisionCheck _groundCollisionCheck;

        [SerializeField]
        private Collider2D _physicalCollider;

        [SerializeField]
        private CollisionCheck _physicialCollisionCheck;

        public CharacterState State { get; set; }

        /// <summary>
        /// Changes to which direction the controller bumped into an obstacle.
        /// -1 = Left
        /// 0 = None
        /// 1 = Right
        /// </summary>
        public int BumpingDirection { get; set; }

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

        public virtual void Update()
        {
            if (_setup && !_runnedSetup)
                Setup();
            UpdateState();

            if (_flipWithVelocity && _rigidbody)
                Flip(_rigidbody.velocity.x);

            CheckSideBumping();
        }

        protected virtual void Setup()
        {
            _runnedSetup = true;
            if (!_rigidbody)
                _rigidbody = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();

            if (!GroundCollisionCheck)
                GroundCollisionCheck = GetComponent<CollisionCheck>() ?? gameObject.AddComponent<CollisionCheck>();

            Collider2D col = GetComponent<Collider2D>() ?? gameObject.AddComponent<BoxCollider2D>();

            GroundCollisionCheck.CollidersToCheck.Add(col);
            GroundCollisionCheck.CollisionLayers = ~0;
            
            if (!_model)
                _model = gameObject;
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
            if (dir > 0)
                _model.transform.localScale = new Vector3(1, transform.localScale.y);
            if (dir < 0)
                _model.transform.localScale = new Vector3(-1, transform.localScale.y);
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
            if (!_physicalCollider)
            {
                Debug.LogWarning("CheckSideBumping requires physical collider reference to function properly.", transform);
            }

            //Offsets for raycasts
            Vector2 xOffset = new Vector2(0.01f, 0);
            Vector2 yOffset = new Vector2(0, 0.01f);

            Vector2 pos = _physicalCollider.transform.position;
            Vector2 extents = _physicalCollider.bounds.extents;
            Vector2 origin = new Vector2(pos.x, pos.y + _physicalCollider.bounds.size.y / 2 + _physicalCollider.offset.y); //Root origin for raycasts

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
            if (_physicialCollisionCheck)
            {
                leftBump = _physicialCollisionCheck.Left;
                rightBump = _physicialCollisionCheck.Right;
            }
            else
                Debug.LogWarning("CheckSideBumping requires physical collision check to function properly.", transform);

            if (!leftHit || leftBump)
                BumpingDirection = -1;
            else if (!rightHit)
                BumpingDirection = 1;
            else
                BumpingDirection = 0;
        }
    }
}