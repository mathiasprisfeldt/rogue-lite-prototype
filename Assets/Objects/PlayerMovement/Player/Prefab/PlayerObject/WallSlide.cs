
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof (PlayerMovement)), ExecuteInEditMode]
    public class WallSlide : MonoBehaviour
    {
        [SerializeField] private float _wallSlideForce;

        [SerializeField] private bool _active;

        private PlayerMovement _playerMovement;
        private float _oldGrav;

        public void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerMovement.WallSlide = this;
        }

        public void ApplyWallSlide(ref float force)
        {
            if (_active)
                force = _playerMovement.Rigidbody.gravityScale*-Physics2D.gravity.y + _wallSlideForce;
        }

        public void OnDisable()
        {
            _playerMovement.WallSlide = null;
        }
    }
}
