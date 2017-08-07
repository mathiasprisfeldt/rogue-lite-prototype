using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using RogueLiteInput;
using UnityEngine;
using AcrylecSkeleton;
using AcrylecSkeleton.ModificationSystem;

namespace RogueLiteMovement
{

    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private WallJump _wallJump;

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
        private CollisionSides _triggerSides;
        private bool _shouldJump;
        private bool _shouldWallJump;
        private Vector2 _velocity;
        private float _runTimer;

        public WallJump WallJump
        {
            get { return _wallJump; }
            set { _wallJump = value; }
        }


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            _triggerCheck.IsColliding(out _triggerSides);
            _collisionCheck.IsColliding(out _collisionSides);
            if (_app.C.PlayerActions.Jump.WasPressed && _triggerSides.Bottom)
                _shouldJump = true;

            if (_app.C.PlayerActions.Jump.WasPressed && (_collisionSides.Right || _collisionSides.Left))
                _shouldWallJump = true;
        }

        void FixedUpdate()
        {
            HandleVerticalMovement(ref _velocity, _jumpForce);
            HandleHorizontalMovement(ref _velocity, _horizontalSpeed);
        }

        void LateUpdate()
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x,
                Mathf.Clamp(_rigidbody.velocity.y, -_maxFallSpeed, float.MaxValue));
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

                if (col.Count > 0 && _app.C.PlayerActions.Down.IsPressed)
                {
                    force = 0;
                    foreach (var c in col)
                    {
                        _modificationHandler.AddModification(new TemporaryLayerChange(0.15f,
                            "ChangeLayerOf" + c.gameObject.name, "NonPlayerCollision", c.gameObject));
                    }
                }
                _shouldWallJump = false;
                _shouldJump = false;
            }
            else
                force = 0;

            if (force != 0)
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, force);
        }


        private void HandleHorizontalMovement(ref Vector2 velocity, float speed)
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var wallDir = 0f;
            Flip(horizontal);

            var run = horizontal != 0;
            if (run)
                _runTimer = 0;
            else
                _runTimer += Time.deltaTime;

            if (!_collisionSides.Bottom && !_triggerSides.Bottom)
                _animator.SetInteger("State", 2);
            else if (_runTimer < .1f)
                _animator.SetInteger("State", 1);
            else
                _animator.SetInteger("State", 0);

            if (_collisionSides.Left && horizontal < 0)
                horizontal = 0;

            if (_collisionSides.Right && horizontal > 0)
                horizontal = 0;

            var dir = Mathf.Clamp(horizontal + wallDir, -1, 1);

            if (WallJump && WallJump.Active)
            {
                speed = WallJump.HorizontalForce;
                dir = WallJump.Direction;
            }

            _rigidbody.velocity = new Vector2(dir*speed*Time.fixedDeltaTime, _rigidbody.velocity.y);
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