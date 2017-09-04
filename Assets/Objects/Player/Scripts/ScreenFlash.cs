using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Controllers;
using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Flash
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class ScreenFlash : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        
        [SerializeField]
        private Color _damageColor;

        [SerializeField]
        private Color _healColor;

        private Color _targetColor;
        private PlayerView _playerView;
        private float _fadeInTimer;
        private float _fadeOutTimer;
        private float _fadeInDuration;
        private float _fadeOutDuration;

        public UnityEvent OnStartEvent;
        public UnityEvent OnEndEvent;

        public Color DamageColor
        {
            get { return _damageColor; }
        }

        public Color HealColor
        {
            get { return _healColor; }
        }

        public void StartFlash(Color targetColor, float time, float fadeinTime = .5f)
        {
            _targetColor = targetColor;
            Color newColor = targetColor;
            newColor.a = 0;
            OnStartEvent.Invoke();
            _fadeInTimer = Mathf.Clamp01(fadeinTime) * time;
            _fadeOutTimer = time - _fadeInTimer;
            _fadeInDuration = _fadeInTimer;
            _fadeOutDuration = _fadeOutTimer;
        }


        public void Update()
        {
            if (_playerView == null && GameManager.Instance && GameManager.Instance.Player)
            {
                _playerView = GameManager.Instance.Player.V;
                _playerView.ScreenFlash = this;
            }

            if (_fadeInTimer > 0 || _fadeOutTimer > 0)
            {
                if (_fadeInTimer > 0)
                {
                    Color color = _targetColor;
                    color.a = Mathf.Abs(_fadeInTimer/_fadeInDuration - 1)*_targetColor.a;
                    _image.color = color;
                    _fadeInTimer -= Time.deltaTime;
                }
                else if (_fadeOutTimer > 0)
                {
                    Color color = _targetColor;
                    color.a = _fadeOutTimer / _fadeOutDuration * _targetColor.a;
                    _image.color = color;
                    _fadeOutTimer -= Time.deltaTime;
                }                  
            }
        }

        public void OnDestroy()
        {
            OnEndEvent.RemoveAllListeners();
            OnStartEvent.RemoveAllListeners();
        }
    }
}