using System;
using System.Collections.Generic;
using UnityEngine;
using Abilitys;
using Managers;

namespace Pickups
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class AbilityPickup : Pickup
    {
        [SerializeField]
        private List<HandledAbility> _autoAbilities = new List<HandledAbility>(); 

        [SerializeField]
        private List<Sprite> _autoSkins = new List<Sprite>();

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private HandledAbility _currentAbility;

        private static event Action AbilityPickupAction;

        public void Start()
        {
            _currentAbility = HandledAbility.None;
            AbilityHandler ah = GameObject.FindObjectOfType<AbilityHandler>();
            if (ah != null)
            {
                CheckAbility(ah);
            }

            AbilityPickupAction += OnAbilityPickupAction;
        }

        private void OnAbilityPickupAction()
        {
            _currentAbility = HandledAbility.None;
        }

        private void CheckAbility(AbilityHandler ah)
        {
            for (int i = 0; i < _autoAbilities.Count; i++)
            {
                if (!ah.GetAbility(_autoAbilities[i]).Active)
                {
                    _currentAbility = _autoAbilities[i];
                    if (i < _autoSkins.Count)
                    {
                        _spriteRenderer.sprite = _autoSkins[i];
                        break;
                    }

                }
            }
        }

        public void Update()
        {
            if(_currentAbility == HandledAbility.None && GameManager.Instance && GameManager.Instance.Player)
                if(GameManager.Instance.Player.C.Character.AbilityHandler != null)
                    CheckAbility(GameManager.Instance.Player.C.Character.AbilityHandler);
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            Check(collision.gameObject);
        }

        public override void Apply(GameObject go)
        {
            CollisionCheck cc = go.GetComponent<CollisionCheck>();
            if (cc != null)
            {
                if (cc.Character.AbilityHandler != null)
                {
                    cc.Character.AbilityHandler.UnlockAbility(_currentAbility);

                    AbilityPickupAction -= OnAbilityPickupAction;
                    if(AbilityPickupAction != null)
                        AbilityPickupAction.Invoke();

                    Destroy(gameObject);
                }
                    
            }
        }
    }
}