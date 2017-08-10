using Assets.Objects.PlayerMovement.Player.Prefab.Player;
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
        private GameObject _model;

        [SerializeField]
        private CollisionCheck _groundCollisionCheck;

        public CharacterState State { get; set; }

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
        }

        protected virtual void Setup()
        {
            _runnedSetup = true;
            if (!_rigidbody)
                _rigidbody = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();

            if (!GroundCollisionCheck)
                GroundCollisionCheck = GetComponent<CollisionCheck>() ?? gameObject.AddComponent<CollisionCheck>();

            Collider2D col = GetComponent<Collider2D>() ?? gameObject.AddComponent<BoxCollider2D>();

            GroundCollisionCheck.Colliders.Add(col);
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
    }
}