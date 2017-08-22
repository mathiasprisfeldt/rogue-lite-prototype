using UnityEngine;

namespace ObjectDetacher
{
    /// <summary>
    /// Purpose: Used to detach a particle system when its method gets called.
    /// Creator: MP
    /// </summary>
    public class ParticleDetacher : MonoBehaviour
    {
        private bool _isDetached;

        [SerializeField]
        private ParticleSystem _particleSystem;

        public void Detach()
        {
            transform.parent = null;
            _particleSystem.Stop();
            _isDetached = true;
        }

        void Update()
        {
            if (_isDetached && _particleSystem.particleCount <= 0)
                Destroy(gameObject);
        }

    }
}