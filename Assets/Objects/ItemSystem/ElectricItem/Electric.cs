using System;
using AcrylecSkeleton.ModificationSystem;
using AcrylecSkeleton.Utilities;
using Archon.SwissArmyLib.Events;
using Archon.SwissArmyLib.Utils;
using CharacterController;
using Controllers;
using Enemy;
using Health;
using UnityEngine;

namespace ItemSystem.Items
{
    /// <summary>
    /// Purpose:
    /// Creator: MP
    /// </summary>
    public class Electric : Item, TellMeWhen.ITimerCallback
    {
        [SerializeField]
        private float _stunTime = .5f;

        [SerializeField]
        private float _shieldCooldown = 2f;

        [SerializeField]
        private float _trapImmuneDuration = 1f;

        [SerializeField]
        private GameObject _shieldPrefab;

        [SerializeField]
        private Vector2 _shieldSpawnOffset;

        private float _shieldCooldownTimer;
        private GameObject _shieldObject;

        protected override void DoubleUp()
        {
            base.DoubleUp();
            ItemHandler.Owner.HealthController.OnNonDamage.AddListener(OnNonDamage);
            SpawnShield();
        }

        protected override void DoubleDown()
        {
            base.DoubleDown();
            ItemHandler.Owner.HealthController.OnNonDamage.RemoveListener(OnNonDamage);
            ItemHandler.Owner.HealthController.IsInvurnable = false;
            DestroyShield();
        }

        private void OnNonDamage(Character arg0)
        {
            ItemHandler.Owner.HealthController.IsInvurnable = false;
            ItemHandler.Owner.HealthController.TrapImmune = true;
            TellMeWhen.Seconds(_trapImmuneDuration,this);
            _shieldCooldownTimer = _shieldCooldown;
            DestroyShield();
        }

        private void Update()
        {
            if (Other && _slaveState == ItemSlave.Master)
            {
                if (_shieldCooldownTimer > 0)
                    _shieldCooldownTimer -= BetterTime.DeltaTime;
                else if (!ItemHandler.Owner.HealthController.IsInvurnable)
                    SpawnShield();
            }
        }

        public override void OnHit(HealthController victim)
        {
            base.OnHit(victim);

            if (!(victim.Character is ActionsController))
                victim.Character.GetComponent<EnemyController>().Stun(_stunTime);

            //Cannot stun player
        }

        private void SpawnShield()
        {
            ItemHandler.Owner.HealthController.IsInvurnable = true;
            _shieldObject = Instantiate(_shieldPrefab, ItemHandler.Owner.transform);
            _shieldObject.transform.localPosition = _shieldSpawnOffset;
        }

        private void DestroyShield()
        {
            if(_shieldObject == null)
                return;

            Animator shieldAnimator = _shieldObject.GetComponent<Animator>();
            if (shieldAnimator)
                shieldAnimator.SetTrigger("Destroy");

        }

        public void OnTimesUp(int id, object args)
        {
            ItemHandler.Owner.HealthController.TrapImmune = false;
        }
    }
}