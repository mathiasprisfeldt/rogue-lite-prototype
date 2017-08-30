using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Controllers;
using Managers;
using UnityEditor;
using UnityEngine;
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
        private float _currentFlashDuration;
        private float _flashTimer;
        private PlayerView _playerView;

        public Color DamageColor
        {
            get { return _damageColor; }
        }

        public Color HealColor
        {
            get { return _healColor; }
        }

        public void StartFlash(Color targetColor, float time)
        {
            _targetColor = targetColor;
            Color newColor = targetColor;
            newColor.a = 0;
            _currentFlashDuration = time;
            _flashTimer = time;
        }


        public void Update()
        {
            if (_playerView == null && GameManager.Instance && GameManager.Instance.Player)
            {
                _playerView = GameManager.Instance.Player.V;
                _playerView.ScreenFlash = this;
            }

            if (_flashTimer > 0)
            {
                if (_flashTimer > _currentFlashDuration/2)
                {
                    Color color = _targetColor;
                    color.a = Mathf.Abs(((_flashTimer/2) / (_currentFlashDuration / 2))) * _targetColor.a;
                    _image.color = color;
                }
                else
                {
                    Color color = _targetColor;
                    color.a = ((_flashTimer/2) / (_currentFlashDuration / 2)) * _targetColor.a;
                    _image.color = color;
                }
                _flashTimer -= Time.deltaTime;
            }
        }
    }
}