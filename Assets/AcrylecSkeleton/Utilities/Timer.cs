using System;
using UnityEngine;
using UnityEngine.Events;

namespace AcrylecSkeleton.Utilities
{
    public class Timer : MonoBehaviour
    {
        private float _intervalTimer;

        [Space, SerializeField]
        private bool _isScaled = true;

        [SerializeField, Tooltip("Can StartTimer interrupt current timer.")]
        private bool _canOverride;

        [SerializeField, Tooltip("In Seconds.")]
        private float _duration = 3;

        [SerializeField, Tooltip("In Seconds.")]
        private float _interval =  1;

#pragma warning disable 649
        [Space, SerializeField]
        private UnityEvent _elapsedEvent;

        [SerializeField]
        private UnityEvent _finishedEvent;
#pragma warning restore 649

        public TimeSpan Clock { get; private set; }

        public bool IsRunning { get; private set; }

        public TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromSeconds(_duration);
            }
        }

        /// <summary>
        /// Invoked when timer hits the interval.
        /// </summary>
        public UnityEvent ElapsedEvent
        {
            get { return _elapsedEvent; }
        }

        /// <summary>
        /// Invoked when timer is finished with its duration.
        /// </summary>
        public UnityEvent FinishedEvent
        {
            get { return _finishedEvent; }
        }

        void Update()
        {
            if (!IsRunning)
                return;

            float time = _isScaled ? Time.deltaTime : Time.unscaledDeltaTime;

            Clock += TimeSpan.FromSeconds(time);
            _intervalTimer += time;

            if (!_interval.FastApproximately(0) && _intervalTimer >= _interval)
            {
                _intervalTimer = 0;

                ElapsedEvent.Invoke();
            }

            if (Clock >= TimeSpan.FromSeconds(_duration))
            {
                IsRunning = false;
                Clock = TimeSpan.Zero;

                FinishedEvent.Invoke();
            }
        }

        /// <summary>
        /// Starts the timer with new duration or interval
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="interval"></param>
        public void StartTimer(float duration, float interval)
        {
            if (!_canOverride && IsRunning)
                return;

            IsRunning = true;
            Clock = TimeSpan.Zero;
        }

        public void StartTimer()
        {
            StartTimer(_duration, _interval);   
        }
    }
}