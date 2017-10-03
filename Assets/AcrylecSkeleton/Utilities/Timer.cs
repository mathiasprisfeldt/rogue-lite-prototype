using System;
using System.Collections.Generic;
using AcrylecSkeleton.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace AcrylecSkeleton.Utilities
{
    public enum DeltaTimeType
    {
        Scaled,
        UnScaled,
        Smoothed
    }

    public class TimerUpdater : Singleton<TimerUpdater>
    {
        private readonly List<Timer> _timers = new List<Timer>();

        /// <summary>
        /// Adds a new timer to the mix.
        /// </summary>
        /// <param name="timer"></param>
        public void Add(Timer timer)
        {
            _timers.Add(timer);
        }

        public void Remove(Timer timer)
        {
            _timers.Remove(timer);
        }

        /// <summary>
        /// Make certain that all registered timers gets updated.
        /// </summary>
        void Update()
        {
            foreach (Timer timer in _timers)
            {
                switch (timer.DeltaDeltaTimeType)
                {
                    case DeltaTimeType.Scaled:
                        timer.Update(Time.deltaTime);
                        break;
                    case DeltaTimeType.UnScaled:
                        timer.Update(Time.unscaledDeltaTime);
                        break;
                    case DeltaTimeType.Smoothed:
                        timer.Update(Time.smoothDeltaTime);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    [Serializable]
    public class Timer
    {
        private bool _isInitialized;
        private float _intervalTimer;

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        [SerializeField]
        private bool _canOverride;

        [SerializeField]
        private float _duration = 3;

        [SerializeField]
        private float _interval = 1;

        [SerializeField]
        private DeltaTimeType _deltaDeltaTimeType = DeltaTimeType.Scaled;

        [Space, SerializeField]
        private UnityEvent _elapsed = new UnityEvent();

        [SerializeField]
        private UnityEvent _started = new UnityEvent();

        [SerializeField]
        private UnityEvent _finished = new UnityEvent();
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        public TimeSpan Clock { get; private set; }
        public bool IsRunning { get; private set; }
        public double Normalized { get; set; }

        public DeltaTimeType DeltaDeltaTimeType
        {
            get { return _deltaDeltaTimeType; }
        }

        public TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromSeconds(_duration);
            }
        }

        public TimeSpan ReversedClock
        {
            get { return Duration.Subtract(Clock); }
        }

        public UnityEvent Elapsed
        {
            get { return _elapsed; }
            set { _elapsed = value; }
        }

        public UnityEvent Started
        {
            get { return _started; }
            set { _started = value; }
        }

        public UnityEvent Finished
        {
            get { return _finished; }
            set { _finished = value; }
        }

        public Timer(float duration, float interval = 1, bool canOverride = false, DeltaTimeType deltaTimeType = DeltaTimeType.Scaled)
        {
            _duration = duration;
            _interval = interval;
            _canOverride = canOverride;
            _deltaDeltaTimeType = deltaTimeType;
        }

        /// <summary>
        /// This must be run before the timer can work.
        /// It adds itself to Timer updater.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
                return;

            TimerUpdater.Instance.Add(this);
            _isInitialized = true;
        }

        public void Update(float deltaTime)
        {
            if (!IsRunning)
                return;

            Clock += TimeSpan.FromSeconds(deltaTime);
            Normalized = Clock.TotalSeconds / _duration;
            _intervalTimer += deltaTime;

            if (!_interval.FastApproximately(0) && _intervalTimer >= _interval)
            {
                _intervalTimer = 0;

                Elapsed.Invoke();
            }

            if (Clock >= TimeSpan.FromSeconds(_duration))
            {
                IsRunning = false;
                Clock = TimeSpan.Zero;

                Finished.Invoke();
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

            if (!_isInitialized)
                Initialize();

            IsRunning = true;
            Clock = TimeSpan.Zero;
            Started.Invoke();
        }

        public void StartTimer()
        {
            StartTimer(_duration, _interval);
        }

        public void Destroy()
        {
            if (TimeManager.CheckSanity())
                TimerUpdater.Instance.Remove(this);
        }
    }
}