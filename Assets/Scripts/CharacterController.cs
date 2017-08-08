﻿using UnityEngine;

namespace CharacterController
{
    public enum CharacterState
    {
        Idle, Moving, InAir, OnWall
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

        [Header("Auto Setup"), SerializeField]
        private bool _setup;

        [SerializeField]
        private bool _flipWithVelocity;

        [SerializeField]
        protected Rigidbody2D _rigidbody;

        [SerializeField]
        protected CollisionCheck _collisionCheck;

        [SerializeField]
        private GameObject _model;

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
                if (!(_collisionCheck && _collisionCheck.Bottom))
                {
                    
                }
                return _collisionCheck && _collisionCheck.Bottom;
            }
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
                
            if(!_collisionCheck)
                _collisionCheck = GetComponent<CollisionCheck>() ?? gameObject.AddComponent<CollisionCheck>();

            Collider2D col = GetComponent<Collider2D>() ?? gameObject.AddComponent<BoxCollider2D>();

            _collisionCheck.Colliders.Add(col);
            _collisionCheck.CollisionLayers = ~0;

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
            else if (OnGround)
            {
                if (State != CharacterState.Idle && _idleTimer < 0.2f)
                {
                    _idleTimer += Time.deltaTime;
                    State = CharacterState.Moving;
                }
                else   
                    State = CharacterState.Idle;
            }

            else if (_collisionCheck && (_collisionCheck.Right || _collisionCheck.Left))
                State = CharacterState.OnWall;
            else
            {
                if (_inAirTimer < 0.2f && State != CharacterState.InAir)
                    _inAirTimer += Time.deltaTime;
                else
                {
                    _inAirTimer = 0;
                    State = CharacterState.InAir;
                }
                    
            }
                
        }


        private void Flip(float dir)
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