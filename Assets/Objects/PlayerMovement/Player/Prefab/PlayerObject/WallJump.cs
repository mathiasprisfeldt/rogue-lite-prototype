using UnityEngine;

namespace CharacterController
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(PlayerMovement)), ExecuteInEditMode]
    public class WallJump : MonoBehaviour
    {
        private float _wallJumpTimer;
        private float _wallJumpDirection;
        private PlayerMovement _playerMovement;

        [SerializeField]
        private float _verticalForce;

        [SerializeField]
        private float _horizontalForce;

        [SerializeField]
        private float _duration;

        public float HorizontalForce
        {
            get
            {
                    return _horizontalForce / _duration;
            }
        }

        public float VerticalForce
        {
            get { return _verticalForce; }
        }

        public bool Active { get; set; }
        public float Direction { get; private set; }

        public void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerMovement.WallJump = this;
        }

        public virtual void Update()
        {
            if (_wallJumpTimer > 0)
            {
                Active = true;
                _wallJumpTimer -= Time.deltaTime;
            }
            else if (Active)
                Active = false;
        }

        public virtual void StartWallJump(float direction)
        {
            Direction = direction;
            _wallJumpTimer = _duration;
        }

        public void OnDisable()
        {
            _playerMovement.WallJump = null;
        }
    }
}