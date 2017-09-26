using System.Diagnostics;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;

namespace AcrylecSkeleton.Utilities
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private float _duration;

        [SerializeField]
        private float _interval;

        [SerializeField]
        private UnityEvent _elapsedEvent;

        [SerializeField]
        private UnityEvent _finishedEvent;

        public UnityEvent ElapsedEvent
        {
            get { return _elapsedEvent; }
        }

        public UnityEvent FinishedEvent
        {
            get { return _finishedEvent; }
        }

        void Start()
        {
        }

        void Update()
        {
        }

        public void StartTimer()
        {
        }

        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            ElapsedEvent.Invoke();
        }

        void OnDestroy()
        {
        }
    }
}