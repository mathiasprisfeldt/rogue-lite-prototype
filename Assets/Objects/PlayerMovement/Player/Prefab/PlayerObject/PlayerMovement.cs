using System.Collections.Generic;
using System.Linq;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;
using AcrylecSkeleton.ModificationSystem;

namespace RogueLiteMovement
{
    public enum PlayerState
    {
        Idle, Walking, InAir, OnWall
    }

    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private WallJump _wallJump;

        [SerializeField]
        private DoubleJump _doubleJump;

        [SerializeField]
        private WallSlide _wallSlide;

        [Header("Component References"),SerializeField]
        private PlayerApplication _app;

        [SerializeField]
        private ModificationHandler _modificationHandler;

        [SerializeField]
        private CollisionCheck _triggerCheck;

        [SerializeField]
        private CollisionCheck _collisionCheck;

        [SerializeField]
        private float _horizontalSpeed;

        [SerializeField]
        private float _jumpForce;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private Rigidbody2D _rigidbody;

        [SerializeField]
        private GameObject _model;

        [SerializeField]
        private float _maxFallSpeed;

        private CollisionSides _collisionSides;
        private CollisionSides _predictSides;
        private bool _shouldJump;
        private bool _shouldWallJump;
        private Vector2 _velocity;
        private float _runTimer;

        public PlayerState PlayerState { get; set; }

        public WallJump WallJump
        {
            get { return _wallJump; }
            set { _wallJump = value; }
        }     

        public DoubleJump DoubleJump
        {
            get { return _doubleJump; }
            set { _doubleJump = value; }
        }

        public PlayerApplication App
        {
            get { return _app; }
            set { _app = value; }
        }

        public Rigidbody2D Rigidbody
        {
            get { return _rigidbody; }
            set { _rigidbody = value; }
        }

        public WallSlide WallSlide
        {
            get { return _wallSlide; }
            set { _wallSlide = value; }
        }

        // Update is called once per frame
        void Update()
        {
            HandleState();
            _triggerCheck.IsColliding(out _predictSides);
            _collisionCheck.IsColliding(out _collisionSides);
            if (App.C.PlayerActions.Jump.WasPressed && _predictSides.Bottom)
                _shouldJump = true;

            if (App.C.PlayerActions.Jump.WasPressed && (_collisionSides.Right || _collisionSides.Left))
                _shouldWallJump = true;
        }

        void FixedUpdate()
        {
            HandleVerticalMovement(ref _velocity, _jumpForce);
            HandleHorizontalMovement(ref _velocity, _horizontalSpeed);
        }

        void LateUpdate()
        {
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x,
                Mathf.Clamp(Rigidbody.velocity.y, -_maxFallSpeed, float.MaxValue));

           
        }

        private void HandleState()
        {
            if ((_collisionSides.Left || _collisionSides.Right) && !_collisionSides.Bottom)
            {
                _animator.SetInteger("State", 2);
                PlayerState = PlayerState.OnWall;
            }
            else if (!_collisionSides.Bottom)
            {
                _animator.SetInteger("State", 2);
                PlayerState = PlayerState.InAir;
            }
            else if (_runTimer < .1f)
            {
                _animator.SetInteger("State", 1);
                PlayerState = PlayerState.Walking;
            }
            else
            {
                _animator.SetInteger("State", 0);
                PlayerState = PlayerState.Idle;
            }
        }

        private void HandleVerticalMovement(ref Vector2 velocity, float force)
        {
            if (_shouldWallJump && (_collisionSides.Left || _collisionSides.Right) && WallJump)
            {
                _shouldWallJump = false;
                _shouldJump = false;
                float dir = _collisionSides.Left ? 1 : -1;
                WallJump.StartWallJump(dir);
                force = WallJump.VerticalForce;
            }
            else if (_shouldJump && _collisionSides.Bottom)
            {
                List<Collider2D> col = _collisionSides.BottomColliders.FindAll(x => x.gameObject.tag == "OneWayCollider").ToList();

                if (col.Count > 0 && App.C.PlayerActions.Down.IsPressed)
                {
                    force = 0;
                    foreach (var c in col)
                    {
                        _modificationHandler.AddModification(new TemporaryLayerChange(0.4f,
                            "ChangeLayerOf" + c.gameObject.name, "NonPlayerCollision", c.gameObject));
                    }
                }
                _shouldWallJump = false;
                _shouldJump = false;
            }
            else
                force = 0;

            if (_wallSlide)
                _wallSlide.ApplyWallSlide(ref force);

            if (force != 0)
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, force * Time.fixedDeltaTime);
        }


        private void HandleHorizontalMovement(ref Vector2 velocity, float speed)
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var wallDir = 0f;
            

            var run = horizontal != 0;
            if (run)
                _runTimer = 0;
            else
                _runTimer += Time.deltaTime;                

            if (_collisionSides.Left && horizontal < 0)
                horizontal = 0;

            if (_collisionSides.Right && horizontal > 0)
                horizontal = 0;

            var dir = horizontal;
            Debug.Log(horizontal);

            if (WallJump && WallJump.Active)
            {
                speed = WallJump.HorizontalForce;
                dir = WallJump.Direction;
            }
            Flip(dir);
            Rigidbody.velocity = new Vector2(dir*speed*Time.fixedDeltaTime, Rigidbody.velocity.y);
        }

        private void Flip(float dir)
        {
            if (dir > 0)
                _model.transform.localScale = new Vector3(1, transform.localScale.y);
            if (dir < 0)
                _model.transform.localScale = new Vector3(-1, transform.localScale.y);
        }

        public class TemporaryLayerChange : Modification
        {
            private LayerMask _targetLayer;
            private LayerMask _oldLayer;
            private GameObject _targetObject;

            public TemporaryLayerChange(float time, string name, string targetLayer, GameObject targetObject)
                : base(time, name)
            {
                _targetLayer = LayerMask.NameToLayer(targetLayer);
                if (_targetLayer == -1)
                {
                    Debug.LogWarning(targetLayer + " layer dose not exist!");
                    RemoveModificaiton();
                }

                else
                {
                    _oldLayer = targetObject.layer;
                    targetObject.layer = _targetLayer;
                    _targetObject = targetObject;
                }

            }

            public override void ApplyModificaiton()
            {
            }

            public override void RemoveModificaiton()
            {
                if (_targetObject)
                    _targetObject.layer = _oldLayer;
            }

            public override void UpdateModificaiton()
            {

            }

            public override void FixedUpdateModificaiton()
            {

            }
        }
    }
}