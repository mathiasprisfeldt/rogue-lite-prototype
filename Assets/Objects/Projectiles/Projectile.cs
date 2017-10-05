using System.Collections.Generic;
using AcrylecSkeleton.Extensions;
using AcrylecSkeleton.Utilities;
using Archon.SwissArmyLib.Events;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using CharacterController;

namespace Projectiles
{
    /// <summary>
    /// Purpose: Base class for all projectiles.
    /// Creator: MB
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour, TellMeWhen.ITimerCallback
    {
        [Header("Settings:"), SerializeField]
        protected float _damage;

        [SerializeField, Tooltip("Amount of time in seconds it takes before it kills itself.")]
        private float _timeToLive = 15;

        [SerializeField, Tooltip("Should force be set constantly, or with AddForce.")]
        private bool _useConstantForce;

        [SerializeField, Tooltip("Amount of force to add.")]
        private float _force;

        [SerializeField]
        private float _additionalYDirection;

        [SerializeField]
        protected LayerMask _layersToHit;

        [SerializeField]
        protected LayerMask _layerToDestoryOn;

        [SerializeField]
        protected List<string> _tagsToHit = new List<string>();

        [SerializeField]
        private bool _animHandlesDestruction;

        [SerializeField]
        private UnityEvent _onDestroy;

        [Header("References:")]
        [SerializeField]
        private GameObject _hitEffectPrefab;

        private Vector2 _direction;
        protected bool _used;

        public UnityEvent OnDestroy { get { return _onDestroy; } }
        public Rigidbody2D RigidBody { get; set; }

        public Vector2 Direction
        {
            get { return _direction; }
            set
            {
                if (value.y == 0)
                {
                    _direction = new Vector2(value.x, _additionalYDirection);
                }
                else
                    _direction = value;
            }
        }

        public Character Owner { get; set; }

        public virtual void Awake()
        {
            RigidBody = GetComponent<Rigidbody2D>();

            //When times up kill itself.
            if (_timeToLive > 0)
                TellMeWhen.Seconds(_timeToLive, this);
        }

        public virtual void Shoot()
        {
            if (Owner is ActionsController)
            {
                _tagsToHit = new List<string>() { "Enemy" };
            }
            else
            {
                _tagsToHit = new List<string>() { "Player" };
            }

            if (!_useConstantForce)
                RigidBody.AddForce(Direction * _force, ForceMode2D.Impulse);
        }

        public virtual void Update()
        {
            if (RigidBody.velocity.x > 0 && transform.localScale.x < 0)
                transform.localScale = new Vector3(1, transform.localScale.y);

            if (RigidBody.velocity.x < 0 && transform.localScale.x > 0)
                transform.localScale = new Vector3(-1, transform.localScale.y);
        }

        void FixedUpdate()
        {
            if (_useConstantForce)
                RigidBody.velocity = Direction * _force;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (TargetCheck(collision.gameObject))
            {
                Kill();
                return;
            }

            if (_layerToDestoryOn.Contains(collision.gameObject.layer))
            {
                if (_hitEffectPrefab)
                {
                    Instantiate(_hitEffectPrefab, collision.bounds.ClosestPoint(transform.position), Quaternion.identity);
                }

                Kill();
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (TargetCheck(collision.gameObject))
                Kill();
        }

        /// <summary>
        /// Used to trigger onDestroy and destroy this gameobject.
        /// </summary>
        protected virtual void Kill()
        {
            RigidBody.simulated = false;
            OnDestroy.Invoke();

            if (!_animHandlesDestruction)
                Remove();
        }

        /// <summary>
        /// Removes the gameobject
        /// </summary>
        public void Remove()
        {
            Destroy(gameObject);
        }

        private bool TargetCheck(GameObject target)
        {
            var targetHit = _layersToHit.Contains(target.layer)
                && !(_tagsToHit.Count > 0 && !_tagsToHit.Contains(target.tag));

            if (targetHit)
            {
                CollisionCheck cc = target.GetComponent<CollisionCheck>();
                if (cc != null)
                {
                    if (!cc.Character.HealthController.IsDead && !_used)
                    {
                        HandleDamage(cc);
                        _used = true;
                    }
                    else
                        targetHit = false;
                }
            }

            return targetHit;
        }

        protected virtual void HandleDamage(CollisionCheck cc)
        {
            cc.Character.HealthController.Damage(_damage, pos: transform.position, from: Owner);
        }

        public virtual void OnTimesUp(int id, object args)
        {
            if (this)
                Kill();
        }
    }
}