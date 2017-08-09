using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;

namespace CharacterController
{
    public enum CharacterState
    {
        None,Idle, Moving, InAir
    }

    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [ExecuteInEditMode]
    public class CharacterController : MonoBehaviour
    {
        private bool _runnedSetup;
        private float _idleTimer;
        private float _inAirTimer;
        private float _onGroundTimer;

        [Header("Auto Setup"), SerializeField]
        private bool _setup;

        [Header("Component References"), SerializeField]
        private PlayerApplication _app;

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
            get
            {
                if (_onGroundTimer > 0)
                    return true;
                return GroundCollisionCheck && GroundCollisionCheck.Bottom;
            }
        }

        public CollisionCheck GroundCollisionCheck
        {
            get { return _groundCollisionCheck; }
            set { _groundCollisionCheck = value; }
        }

        public PlayerApplication App
        {
            get { return _app; }
            set { _app = value; }
        }

        public virtual void Update()
        {
            if (_setup && !_runnedSetup)
                Setup();
            UpdateState();

            if (_flipWithVelocity && _rigidbody)
                Flip(_rigidbody.velocity.x);

            if (!GroundCollisionCheck.Bottom)
                _onGroundTimer -= Time.deltaTime;
            else 
                _onGroundTimer = 0.05f;
        }

        protected virtual void Setup()
        {
            _runnedSetup = true;
            if (!_rigidbody)
                _rigidbody = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
                
            if(!GroundCollisionCheck)
                GroundCollisionCheck = GetComponent<CollisionCheck>() ?? gameObject.AddComponent<CollisionCheck>();

            Collider2D col = GetComponent<Collider2D>() ?? gameObject.AddComponent<BoxCollider2D>();

            GroundCollisionCheck.Colliders.Add(col);
            GroundCollisionCheck.CollisionLayers = ~0;

            if(!_model)
            _model = gameObject;
        }

        protected virtual void UpdateState()
        {
            if (OnGround && _rigidbody.velocity.x != 0)
            {
                _idleTimer = 0;
                State = CharacterState.Moving;
            }                
            else if (OnGround && (!App.C.PlayerActions.Left || !App.C.PlayerActions.Right))
            {
                    State = CharacterState.Idle;
            }
            else
            {
                    State = CharacterState.InAir;
  
            }
            Debug.Log(State);
                
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