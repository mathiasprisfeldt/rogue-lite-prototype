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
        private enum TellMeWhemType
        {
            SpawnShield, Invurnable
        }

        [SerializeField]
        private float _stunTime = .5f;

        [SerializeField]
        private float _shieldCooldown = 2f;

        [SerializeField]
        private float _immuneDuration = 1f;

        [SerializeField]
        private GameObject _shieldPrefab;

        [SerializeField]
        private Vector2 _shieldSpawnOffset;

        private float _shieldCooldownTimer;
        private GameObject _shieldObject;
        private bool _onCooldown;

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
            if(_onCooldown)
                return;
            TellMeWhen.Seconds(_immuneDuration,this, (int)TellMeWhemType.Invurnable);
            TellMeWhen.Seconds(_shieldCooldown,this, (int)TellMeWhemType.SpawnShield);
            DestroyShield();
            _onCooldown = true;
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
            ShieldBehavior shieldBehavior = _shieldObject.GetComponent<ShieldBehavior>();
            if (shieldBehavior != null)
                shieldBehavior.Owner = this;
        }

        private void DestroyShield()
        {
            if(_shieldObject == null)
                return;

            //Animator shieldAnimator = _shieldObject.GetComponent<Animator>();
            //if (shieldAnimator)
            //    shieldAnimator.SetTrigger("Destroy");

        }

        public void OnTimesUp(int id, object args)
        {
            if (!(Other && _slaveState == ItemSlave.Master))
                return;
                switch (id)
            {
                case (int)TellMeWhemType.SpawnShield:
                    SpawnShield();
                    _onCooldown = false;
                    break;
                case (int)TellMeWhemType.Invurnable:
                    ItemHandler.Owner.HealthController.IsInvurnable = false;
                    break;
            }
        }
    }
}