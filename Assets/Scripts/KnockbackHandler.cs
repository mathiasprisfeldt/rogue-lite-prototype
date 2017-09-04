using System;
using System.Collections.Generic;
using System.Linq;
using AcrylecSkeleton.Extensions;
using Archon.SwissArmyLib.Events;
using Controllers;
using UnityEditor;
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

        [SerializeField, Tooltip("If it uses default force dir, it will use given " +
                                 "force to find out which direction to apply the force.")]
        private bool _flipDefaultDir;

        [SerializeField]
        private bool _forceDefaultValues;

        [SerializeField]
        private float _defaultForce;

        [SerializeField]
        private Vector2 _defaultKnockbackDir;

        [SerializeField]
        private AnimationCurve _velCurve =
            new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        [SerializeField]
        private float _defaultDuration;

        public Vector2 Velocity { get; private set; }

        public bool Active
        {
            get { return _knocksBacks.Count > 0; }
        }

        private readonly List<KnockBack> _knocksBacks = new List<KnockBack>();

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
                _character.LockMovement = false;
        }

        private void OnStarted()
        {
            if (_lockMovement)
                _character.LockMovement = true;
        }

        public void Update()
        {
            for (int i = _knocksBacks.Count - 1; i >= 0; i--)
            {
                if (_knocksBacks[i].Time > 0)
                {
                    _knocksBacks[i].Time = Mathf.Clamp01(_knocksBacks[i].Time - Time.deltaTime / _knocksBacks[i].Duration);

                    float time = _knocksBacks[i].Time;
                    Vector2 targetVel = _knocksBacks[i].OriginalVelocity.Vector2Multiply(new Vector2(time, _velCurve.Evaluate(1 - time)));
                    _knocksBacks[i].Velocity = targetVel;
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
            _character.Rigidbody.velocity = _character.Rigidbody.CounterGravity(ApplyKnockback()) * Time.fixedDeltaTime;
        }

        public Vector2 ApplyKnockback()
        {
            Vector2 temp = new Vector2();
            return ApplyKnockback(ref temp);
        }

        public Vector2 ApplyKnockback(ref Vector2 velocity)
        {
            for (int i = _knocksBacks.Count - 1; i >= 0; i--)
                velocity += _knocksBacks[i].Velocity;

            Velocity = velocity;
            return velocity;
        }

        public void AddForce(Vector2 force, float duration)
        {
            if (_forceDefaultValues)
            {
                Vector2 dir = _defaultKnockbackDir;

                //If we use the force parameter to detect where to direct the force do so.
                if (_flipDefaultDir)
                {
                    if (force.x > 0)
                        dir.x *= -1;
                    if (force.x > 0 && _character.PhysicialCollisionCheck.Right)
                        dir.x *= -1;
                    else if (force.x < 0 && _character.PhysicialCollisionCheck.Left)
                        dir.x *= -1;
                }
                    

                force = dir * _defaultForce;
                duration = _defaultDuration;
            }

            if (Started != null && !_knocksBacks.Any())
                Started.Invoke();

            _knocksBacks.Add(new KnockBack(force, duration));
        }

        public void AddForce()
        {
            AddForce(_defaultKnockbackDir * _defaultForce, _defaultDuration);
        }

        public void Clear()
        {
            for (int i = _knocksBacks.Count - 1; i >= 0; i--)
            {
                _knocksBacks.RemoveAt(i);

                if (Done != null && i == 0)
                    Done.Invoke();
            }
        }
    }
}

public class KnockBack
{
    public Vector2 OriginalVelocity { get; set; }
    public Vector2 Velocity { get; set; }
    public float Time { get; set; }
    public float Duration { get; private set; }

    public KnockBack(Vector2 velocity, float time)
    {
        OriginalVelocity = velocity;
        Velocity = Vector2.zero;
        Time = 1;
        Duration = time;
    }
}