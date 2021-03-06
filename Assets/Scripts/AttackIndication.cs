using Archon.SwissArmyLib.Utils;
using UnityEngine;

namespace Indication
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class AttackIndication : MonoBehaviour
    {
        private float _currentTransitionDuration;
        private float _transitionTimer;
        private bool _show;

        public bool Show { get { return _show; } }

        public void Awake()
        {
            transform.localScale = new Vector3(0, 0);
        }

        public void ShowIndicator(float transitionTime)
        {
            _currentTransitionDuration = transitionTime;
            _transitionTimer = transitionTime;
            _show = true;
        }

        public void HideIndicator(float transitionTime)
        {
            if (!_show)
                return;

            _currentTransitionDuration = transitionTime;
            _transitionTimer = transitionTime;
            _show = false;
        }

        public void Update()
        {
            if (_transitionTimer > 0)
            {
                _transitionTimer -= BetterTime.DeltaTime;

                float transitionValue;
                if (_show)
                    transitionValue = (_currentTransitionDuration - _transitionTimer) / _currentTransitionDuration;
                else
                    transitionValue = _transitionTimer / _currentTransitionDuration;
                transitionValue = Mathf.Clamp01(transitionValue);
                transform.localScale = new Vector3(transitionValue, transitionValue);
            }
            
        }

    }
}