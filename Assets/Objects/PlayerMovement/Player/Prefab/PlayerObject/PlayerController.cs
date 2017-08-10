using System;
using AcrylecSkeleton.MVC;
using BindingsExample;
using UnityEngine;
using RogueLiteInput;

namespace Assets.Objects.PlayerMovement.Player.Prefab.Player
{
    /// <summary>
    /// Controller class for Player MVC object.
    /// Created by: Mikkel Nielsen
    /// Data: Monday, August 07, 2017
    /// </summary>
    public class PlayerController : Controller<PlayerApplication>
    {
        [SerializeField]
        private ActionsSetType _activeActionSettype;

        private ActionsSetType _oldActionSetType;
        private KeyboardActions _keyboardActions;

        public InputActions PlayerActions { get; set; }

        public void Awake()
        {
            _keyboardActions = new KeyboardActions();
            ChangeActionSet(_activeActionSettype);
        }

        public void Update()
        {
            if(_activeActionSettype != _oldActionSetType)
                ChangeActionSet(_activeActionSettype);

            _oldActionSetType = _activeActionSettype;
        }

        private void ChangeActionSet(ActionsSetType type)
        {
            switch (type)
            {
                case ActionsSetType.Keyboard:
                    PlayerActions = _keyboardActions;
                    break;
                case ActionsSetType.Controller:
                    PlayerActions = _keyboardActions;
                    break;
                default:
                    PlayerActions = _keyboardActions;
                    break;
            }
        }
    }
}