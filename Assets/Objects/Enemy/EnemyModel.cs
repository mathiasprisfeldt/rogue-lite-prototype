using System;
using AcrylecSkeleton.MVC;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Controllers;
using Indication;
using Managers;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Model class for Enemy MVC object.
    /// Created by: MP-L
    /// Data: Monday, August 14, 2017
    /// </summary>
    public class EnemyModel : Model<EnemyApplication>
    {
        [Header("References:")]
        [SerializeField]
        private Character _character;

        [SerializeField]
        private EnemyAttackAnim _enemyAttackAnim;

        [SerializeField]
        private AttackIndication _attackIndicator;

        [SerializeField]
        private AttackIndication _staggerIndicator;

        [SerializeField]
        private CollisionCheck _viewBox;

        [Space]
        [Header("Enemy Settings:")]
        [SerializeField]
        private float _attackCooldown;

        [SerializeField, Tooltip("How long it takes to do an action.")]
        private float _indicatorDuration;

        [SerializeField, Tooltip("Can the enemy target player if behind him?")]
        private bool _targetBehind;

        [SerializeField, Tooltip("Amount of time it takes the enemy to turn around.")]
        private float _turnSpeed;

        [SerializeField]
        private bool _canBackPaddle;

        [SerializeField, Tooltip("Should the enemy turn if hit from behind?")]
        private bool _turnOnBackstab;

        [SerializeField, Tooltip("Speed added to enemy when player is targeted.")]
        private float _engageSpeed;

        [SerializeField]
        private bool _hasWallHack;

        [SerializeField]
        private bool _isFlying;

        [SerializeField, Tooltip("The enemy never loses it target.")]
        private bool _neverForget;

        [SerializeField, Tooltip("How long it takes for the enemy to forget the player.")]
        private float _memoryDuration;

        public Character Character
        {
            get { return _character; }
            set { _character = value; }
        }

        public EnemyAttackAnim AttackAnim
        {
            get { return _enemyAttackAnim; }
        }

        public AttackIndication AttackIndicator
        {
            get { return _attackIndicator; }
            set { _attackIndicator = value; }
        }

        public AttackIndication StaggerIndicator
        {
            get { return _staggerIndicator; }
            set { _staggerIndicator = value; }
        }

        public CollisionCheck ViewBox
        {
            get { return _viewBox; }
            set { _viewBox = value; }
        }

        public float AttackCooldown
        {
            get { return _attackCooldown; }
            set { _attackCooldown = value; }
        }

        public float IndicatorDuration
        {
            get { return _indicatorDuration; }
            set { _indicatorDuration = value; }
        }

        public bool TargetBehind
        {
            get { return _targetBehind; }
            set { _targetBehind = value; }
        }

        public float TurnSpeed
        {
            get { return _turnSpeed; }
            set { _turnSpeed = value; }
        }

        public bool CanBackPaddle
        {
            get { return _canBackPaddle; }
            set { _canBackPaddle = value; }
        }

        public bool TurnOnBackstab
        {
            get { return _turnOnBackstab; }
            set { _turnOnBackstab = value; }
        }

        public float EngageSpeed
        {
            get { return _engageSpeed; }
            set { _engageSpeed = value; }
        }

        public bool IsFlying
        {
            get { return _isFlying; }
            set { _isFlying = value; }
        }

        public bool HasWallHack
        {
            get { return _hasWallHack; }
            set { _hasWallHack = value; }
        }

        public bool NeverForget
        {
            get { return _neverForget; }
            set { _neverForget = value; }
        }

        public float MemoryDuration
        {
            get { return _memoryDuration; }
            set { _memoryDuration = value; }
        }

    }
}