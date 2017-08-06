using UnityEngine;

namespace AcrylecSkeleton.Utilities
{
    /// <summary>
    /// Resizes camera to fit everything places & scaled relative to virtual resolution.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ResponsiveCamera : MonoBehaviour
    {
        private Resolution _prevResolution; //Resolution in previous update loop.

        [SerializeField] private Camera _cam; //Targeted camera

        [SerializeField]
        private float _virtualWidth = 1280, _virtualHeight = 720;

        public float OriginalSize { get; private set; } //Default size

        void Awake()
        {
            UpdateCameraSize();
            OriginalSize = _cam.orthographicSize;
        }

        /// <summary>
        /// Called when changing resolution.
        /// </summary>
        public void OnResolutionChanged()
        {
            UpdateCameraSize();
            OriginalSize = _cam.orthographicSize;
        }

        /// <summary>
        /// Resets camera size to calculated size.
        /// </summary>
        public void UpdateCameraSize()
        {
            float verRatio = _virtualWidth / _virtualHeight;
            float horRatio = (float)Screen.width / (float)Screen.height;
        
            _cam.orthographicSize *= (verRatio / horRatio);
        }
    }
}