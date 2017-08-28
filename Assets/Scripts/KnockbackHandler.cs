using System;
using System.Collections.Generic;
using System.Linq;
using Archon.SwissArmyLib.Events;
using Controllers;
using UnityEngine;

namespace Knockbacks
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(Character))]
    public class KnockbackHandler : MonoBehaviour
    {
        private Character _character;

        [SerializeField]
        private bool _lockMovement;

        public Vector2 Velocity { get; private set; }

        public bool Active
        {
            get { return _knocksBacks.Count > 0; }
        }

        private List<KnockBack> _knocksBacks = new List<KnockBack>();

        public event Action Started; 
        public event Action Done;

        void Awake()
        {
            _character = GetComponent<Character>();

            Started += OnStarted;
            Done += OnDone;
        }

        private void OnDone()
        {
            if (_lockMovement)
                _character.LockMovement = true;
        }

        private void OnStarted()
        {
            if (_lockMovement)
                _character.LockMovement = false;
        }

        public void Update()
        {
            for (int i = _knocksBacks.Count - 1; i >= 0; i--)
            {
                if (_knocksBacks[i].Time > 0)
                {
                    _knocksBacks[i].Time -= Time.deltaTime;
                }
                else
                {
                    _knocksBacks.RemoveAt(i);

                    if (Done != null && i == 0)
                        Done.Invoke();
                }
            }            
        }

        void FixedUpdate()
        {
            if (!Active)
                return;

            Vector2 knockbackForce = Vector2.zero;
            _character.Rigidbody.velocity = _character.Rigidbody.CounterGravity(ApplyKnockback()) * Time.fixedDeltaTime + knockbackForce; 
        }

        public Vector2 ApplyKnockback()
        {
            Vector2 temp = new Vector2();
            return ApplyKnockback(ref temp);
        }

        public Vector2 ApplyKnockback(ref Vector2 velocity)
        {
            for (int i = _knocksBacks.Count - 1; i >= 0; i--)
            {
                if (_knocksBacks[i].Time > 0)
                {
                    velocity += _knocksBacks[i].Velocity;
                }
                else
                    _knocksBacks.RemoveAt(i);
            }
            Velocity = velocity;
            return velocity;
        }

        public void AddForce(Vector2 force, float duration)
        {
            if (Started != null && !_knocksBacks.Any())
                Started.Invoke();

            _knocksBacks.Add(new KnockBack(force, duration));
        }


    }
}

public class KnockBack
    {
        public Vector2 Velocity { get; set; }
        public float Time { get; set; }
        public float Duration { get; private set; }

        public KnockBack(Vector2 velocity, float time)
        {
            Velocity = velocity;
            Time = time;
            Duration = time;
        }
    }