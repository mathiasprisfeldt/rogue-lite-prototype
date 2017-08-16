using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Properties"), SerializeField]
        private float _damage;

        [SerializeField]
        private LayerMask _layersToHit;

        [SerializeField]
        private LayerMask _layerToDestoryOn;

        [SerializeField]
        private List<string> _tagsToHit = new List<string>();


        public Rigidbody2D RigidBody { get; set; }

        public void Awake()
        {
            RigidBody = GetComponent<Rigidbody2D>();
        }

        public void Update()
        {
            if(RigidBody.velocity.x > 0 && transform.localScale.x < 0)
                transform.localScale = new Vector3(1, transform.localScale.y);

            if (RigidBody.velocity.x < 0 && transform.localScale.x > 0)
                transform.localScale = new Vector3(-1, transform.localScale.y);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (TargetCheck(collision.gameObject))
            {
                Destroy(gameObject);
                return;
            }
                
            if(_layerToDestoryOn == (_layerToDestoryOn | (1 << collision.gameObject.layer)))
                Destroy(gameObject);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            TargetCheck(collision.gameObject);
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
                    cc.Character.HealthController.Damage(_damage);
            }
            return targetHit;
        }
    }
}