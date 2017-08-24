using UnityEngine;
using UnityEngine.Events;

namespace AnimationEventProxy
{
    /// <summary>
    /// Purpose: Used as a proxy for animators when wanting an animation event
    /// to trigger an external method.
    /// Creator: MP
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class AnimationEventProxy : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _event;

        public void Invoke()
        {
            _event.Invoke();
        }
    }
}