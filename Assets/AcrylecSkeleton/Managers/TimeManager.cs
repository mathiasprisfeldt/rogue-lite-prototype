using System;
using AcrylecSkeleton.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace AcrylecSkeleton.Managers
{
    public class TimeManager : Singleton<TimeManager>
    {
        private bool _resetTimeOnSceneChange = true;

        /*Slowmotion fields*/
        private float _savedTimeScale;
        private float _targetTime;
        private float _duration;
        private AnimationCurve _animationCurve;
        private Action _finishAction;
        private float _slowmationAnimationTimer; //Used for evaluation of AnimationCurves.
        private bool _slowmotionAnimationIsRunning;
        private float _currentAnimTime;
        private bool _isPaused;

#pragma warning disable 649
        [SerializeField] private AnimationCurve _defaultTimeAnimationCurve; //Evaluated when starting slowmotion.
#pragma warning restore 649

        [SerializeField]
        private UnityEvent _timeAnimationDone = new UnityEvent();

        public static event Action<bool> Paused; 

        public UnityEvent TimeAnimationDone
        {
            get { return _timeAnimationDone; }
            set { _timeAnimationDone = value; }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                _isPaused = value; 
                SetTime(value ? 0 : _slowmotionAnimationIsRunning ? _currentAnimTime : 1, false);
                if (Paused != null) Paused(value);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        
        void Update()
        {
            if (IsPaused)
                return;

            if (_slowmotionAnimationIsRunning)
            {
                _slowmationAnimationTimer += Time.unscaledDeltaTime / _duration;

                var curveTime = _animationCurve.Evaluate(_slowmationAnimationTimer);

                _currentAnimTime = Mathf.Abs(((_savedTimeScale - _targetTime) * curveTime) - _savedTimeScale);
                SetTime(_currentAnimTime, false);

                if (_slowmationAnimationTimer >= 1)
                {
                    _slowmotionAnimationIsRunning = false;
                    _slowmationAnimationTimer = 0;
                    _timeAnimationDone.Invoke();

                    if (_finishAction != null)
                        _finishAction();
                }
            }
        }

        /// <summary>
        /// Resets current time animation then sets the time.
        /// </summary>
        /// <param name="time"></param>
        public void SetTime(float time, bool interuptAnim = true)
        {
            Time.timeScale = time;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        
            if (interuptAnim)
                ResetAnimation();
        }

        /// <summary>
        /// Resets current time animation then sets the time to 1.
        /// </summary>
        public void ResetTime()
        {
            SetTime(1);
        }

        private void OnSceneUnloaded(Scene arg0)
        {
            if (_resetTimeOnSceneChange)
            {
                ResetTime();
            }
        }

        /// <summary>
        /// Animates with default animation curve from current to given TimeScale, at a pace of given speed to run.
        /// </summary>
        /// <param name="time">How long it takes from a to b</param>
        /// <param name="target">Target TimeScale</param>
        /// <param name="curve">Self given animation curve, the animation follows</param>
        /// <returns></returns>
        public void AnimateTime(float duration, float target, AnimationCurve curve)
        {
            ResetAnimation();

            //Setup
            _slowmotionAnimationIsRunning = true;
            _savedTimeScale = Time.timeScale;
            _duration = duration;
            _targetTime = target;
            _animationCurve = curve;
        }

        /// <summary>
        /// Animates with default animation curve from current to given TimeScale, at a pace of given speed to run.
        /// </summary>
        /// <param name="time">How long it takes from a to b</param>
        /// <param name="target">Target TimeScale</param>
        /// <param name="action">Invoked action when done animating</param>
        /// <returns></returns>
        public void AnimateTime(float time, float target, Action action)
        {
            AnimateTime(time, target, _defaultTimeAnimationCurve);
            _finishAction = action;
        }

        public void ResetAnimation()
        {
            _finishAction = null;
            _slowmationAnimationTimer = 0;
            _slowmotionAnimationIsRunning = false;
        }
    }
}