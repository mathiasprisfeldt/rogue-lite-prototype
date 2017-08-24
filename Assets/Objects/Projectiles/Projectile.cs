using System.Collections.Generic;
using Archon.SwissArmyLib.Events;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Projectiles
{
    /// <summary>
    /// Purpose: Base class for all projectiles.
    /// Creator: MB
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour, TellMeWhen.ITimerCallback
    {
        [Header("Settings:"), SerializeField]
        private float _damage;

        [SerializeField, Tooltip("Amount of time in seconds it takes before it kills itself.")]
        private float _timeToLive = 15;

        [SerializeField, Tooltip("Should force be set constantly, or with AddForce.")]
        private bool _useConstantForce;

        [SerializeField, Tooltip("Amount of force to add.")]
        private float _force;

        [SerializeField]
        private LayerMask _layersToHit;

        [SerializeField]
        private LayerMask _layerToDestoryOn;

        [SerializeField]
        private List<string> _tagsToHit = new List<string>();

        [SerializeField]
        private bool _animHandlesDestruction;

        [SerializeField]
        private UnityEvent _onDestroy;

        public Rigidbody2D RigidBody { get; set; }

        public Vector2 Direction { get; set; }

        public void Awake()
        {
            RigidBody = GetComponent<Rigidbody2D>();

            //When times up kill itself.
            TellMeWhen.Seconds(_timeToLive, this);
        }

        public void Shoot()
        {
            if (!_useConstantForce)
                RigidBody.AddForce(Direction * _force, ForceMode2D.Impulse);
        }

        public void Update()
        {
            if (Direction * _force != Vector2.zero)
                RigidBody.velocity = Direction * _force;

            if(RigidBody.velocity.x > 0 && transform.localScale.x < 0)
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

            if (_layerToDestoryOn == (_layerToDestoryOn | (1 << collision.gameObject.layer)))
                Kill();
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            TargetCheck(collision.gameObject);
            Kill();
        }

        /// <summary>
        /// Used to trigger onDestroy and destroy this gameobject.
        /// </summary>
        void Kill()
        {
            RigidBody.simulated = false;
            _onDestroy.Invoke();

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
            var targetHit = _layersToHit == (_layersToHit | (1 << target.layer))
                && !(_tagsToHit.Count > 0 && !_tagsToHit.Contains(target.tag));

            if (targetHit)
            {
                CollisionCheck cc = target.GetComponent<CollisionCheck>();
                if (cc != null)
                {
                    if (!cc.Character.HealthController.IsDead)
                        cc.Character.HealthController.Damage(_damage, pos: transform.position);
                    else
                        targetHit = false;
                }
            }

            return targetHit;
        }

        public void OnTimesUp(int id, object args)
        {
            if (this)
                Kill();
        }
    }
}