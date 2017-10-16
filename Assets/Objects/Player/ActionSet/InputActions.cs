using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using InControl;
using Managers;

namespace RogueLiteInput
{
    /// <summary>
    /// A enum to determine what kind of PlayerActionSet is active.
    /// </summary>
    public enum ActionsSetType
    {
        Keyboard, Controller
    }

    /// <summary>
    /// A enum to determine Action types.
    /// </summary>
    public enum ActionsType
    {
        Left, Right, Jump, Down
    }

    /// <summary>
    /// Purpose: A super class for the different playerActionsSets like keyboard- and controller PlayerActionsSets.
    /// Creator: Mikkel Nielsen
    /// </summary>
    public class InputActions : PlayerActionSet
    {
        private float _horizontalDeadZone = .4f;
        private float _verticalDeadZone = .3f;

        protected ActionsSetType _activeActionSetType;

        //PlayerActions
        public PlayerAction LeftInput { get; set; }
        public PlayerAction RightInput { get; set; }
        public PlayerAction DownInput { get; set; }
        public PlayerAction UpInput { get; set; }
        public PlayerAction Jump { get; set; }
        public PlayerAction Dash { get; set; }
        public PlayerAction Attack { get; set; }
        public PlayerAction Special1 { get; set; }
        public PlayerAction Special2 { get; set; }
        public PlayerAction Sprint { get; set; }
        public PlayerAction Interact { get; set; }

        public PlayerOneAxisAction RawHorizontal { get; set; }
        public PlayerOneAxisAction RawVertical { get; set; }
        public ProxyInputActions ProxyInputActions { get; set; }

        public float Horizontal
        {
            get
            {

                return DeadZoneHorizontal(_horizontalDeadZone);
            }
        }
        public float DeadZoneHorizontal(float deadZone)
        {
            if (Mathf.Abs(RawHorizontal.RawValue) > deadZone)
                return RawHorizontal.RawValue;
            return 0f;
        }
        public float Vertical
        {
            get { return DeadZoneVertical(_verticalDeadZone); }
        }
        public float DeadZoneVertical(float deadZone)
        {
            if (Mathf.Abs(RawVertical.RawValue) > deadZone)
                return RawVertical.RawValue;
            return 0f;
        }
        public bool Up
        {
            get { return DeadZoneUp(_verticalDeadZone); }
        }
        public bool DeadZoneUp(float deadZone)
        {
            return RawVertical.Value > deadZone;
        }
        public bool Down
        {
            get { return DeadZoneDown(_verticalDeadZone); }
        }
        public bool DeadZoneDown(float deadZone)
        {
            return RawVertical.Value < -deadZone;
        }
        public bool Right
        {
            get { return DeadZoneRight(_horizontalDeadZone); }
        }
        public bool DeadZoneRight(float deadZone)
        {
            return RawHorizontal.Value > deadZone;
        }
        public bool Left
        {
            get { return DeadZoneLeft(_horizontalDeadZone); }
        }
        public bool DeadZoneLeft(float deadZone)
        {
            return RawHorizontal.Value < -deadZone;
        }
        public InputActions()
        {
            OnLastInputTypeChanged += type =>
            {
                if(GameManager.Instance && GameManager.Instance.InputTypeChange != null)
                    GameManager.Instance.InputTypeChange.Invoke(type);
            };

            LeftInput = CreatePlayerAction("Left");
            RightInput = CreatePlayerAction("Right");
            Jump = CreatePlayerAction("Jump");
            DownInput = CreatePlayerAction("Down");
            Dash = CreatePlayerAction("Dash");
            UpInput = CreatePlayerAction("Up");
            Attack = CreatePlayerAction("Attack");
            Special1 = CreatePlayerAction("Special1");
            Special2 = CreatePlayerAction("Special2");
            Sprint = CreatePlayerAction("Sprint");
            Interact = CreatePlayerAction("Interact");

            RawHorizontal = CreateOneAxisPlayerAction(LeftInput, RightInput);
            RawVertical = CreateOneAxisPlayerAction(DownInput, UpInput);
            ProxyInputActions = new ProxyInputActions(this);

            //Keyboard inputs
            //Left keyboard
            LeftInput.AddDefaultBinding(Key.LeftArrow);
            LeftInput.AddDefaultBinding(Key.None);

            //Right
            RightInput.AddDefaultBinding(Key.RightArrow);
            RightInput.AddDefaultBinding(Key.None);

            //Down
            DownInput.AddDefaultBinding(Key.DownArrow);
            DownInput.AddDefaultBinding(Key.None);

            //Up
            UpInput.AddDefaultBinding(Key.UpArrow);
            UpInput.AddDefaultBinding(Key.None);

            //Jump
            Jump.AddDefaultBinding(Key.Space);
            Jump.AddDefaultBinding(Key.None);

            //Dash
            Dash.AddDefaultBinding(Key.Shift);
            Dash.AddDefaultBinding(Key.None);

            //Attack
            Attack.AddDefaultBinding(Key.D);
            Attack.AddDefaultBinding(Key.None);

            //Special1
            Special1.AddDefaultBinding(Key.A);
            Special1.AddDefaultBinding(Key.None);

            //Special2
            Special2.AddDefaultBinding(Key.S);
            Special2.AddDefaultBinding(Key.None);

            //Sprint
            Sprint.AddDefaultBinding(Key.None);
            Sprint.AddDefaultBinding(Key.None);

            //Interact
            Interact.AddDefaultBinding(Key.W);
            Interact.AddDefaultBinding(Key.None);

            //Gamepad inputs
            //Left
            LeftInput.AddDefaultBinding(InputControlType.LeftStickLeft);
            LeftInput.AddDefaultBinding(Key.None);

            //Right
            RightInput.AddDefaultBinding(InputControlType.LeftStickRight);
            RightInput.AddDefaultBinding(Key.None);

            //Down
            DownInput.AddDefaultBinding(InputControlType.LeftStickDown);
            DownInput.AddDefaultBinding(Key.None);

            //Up
            UpInput.AddDefaultBinding(InputControlType.LeftStickUp);
            UpInput.AddDefaultBinding(Key.None);

            //Jump
            Jump.AddDefaultBinding(InputControlType.Action1);
            Jump.AddDefaultBinding(Key.None);

            //Dash
            Dash.AddDefaultBinding(InputControlType.Action2);
            Dash.AddDefaultBinding(InputControlType.None);

            //Attack
            Attack.AddDefaultBinding(InputControlType.Action3);
            Attack.AddDefaultBinding(Key.None);

            //Special
            Special1.AddDefaultBinding(InputControlType.LeftTrigger);
            Special1.AddDefaultBinding(Key.None);

            //Specia2
            Special2.AddDefaultBinding(InputControlType.RightTrigger);
            Special2.AddDefaultBinding(Key.None);

            //Sprint
            Sprint.AddDefaultBinding(InputControlType.None);
            Sprint.AddDefaultBinding(InputControlType.None);

            //Interact
            Interact.AddDefaultBinding(InputControlType.Action4);
            Interact.AddDefaultBinding(InputControlType.None);

        }

        public void UpdateProxy()
        {
            ProxyInputActions.UpdateData();
        }

        public void ResetProxy()
        {
            ProxyInputActions.Reset();
        }

        
    }

    public class ProxyPlayerAction
    {
        private bool _wasreset;

        public bool WasPressed { get; set; }
        public bool WasReleased { get; set; }
        public bool IsPressed { get; set; }
        public bool WasRepeated { get; set; }

        public PlayerAction Action { get; set; }

        public ProxyPlayerAction(PlayerAction action)
        {
            Action = action;
        }

        public void UpdateData()
        {
            WasPressed = WasPressed || Action.WasPressed;
            WasReleased = WasReleased || Action.WasReleased;
            IsPressed = Action.IsPressed;
            WasRepeated = WasRepeated || Action.WasRepeated;
            _wasreset = false;
        }

        public void Reset()
        {
            if (_wasreset)
                return;
            WasRepeated = false;
            WasPressed = false;
            WasReleased = false;
            IsPressed = false;
            _wasreset = true;
        }
    }

    public class ProxyInputActions
    {
        public ProxyPlayerAction Jump { get; set; }
        public ProxyPlayerAction Dash { get; set; }
        public ProxyPlayerAction Attack { get; set; }
        public ProxyPlayerAction Special1 { get; set; }
        public ProxyPlayerAction Special2 { get; set; }
        public ProxyPlayerAction Up { get; set; }
        public ProxyPlayerAction Down { get; set; }
        public ProxyPlayerAction Left { get; set; }
        public ProxyPlayerAction Right { get; set; }
        public ProxyPlayerAction Sprint { get; set; }
        public ProxyPlayerAction Interact { get; set; }

        private List<ProxyPlayerAction> _proxyActions;

        public bool AnyWasPressed
        {
            get{ return _proxyActions.Any(proxyPlayerAction => proxyPlayerAction.WasPressed); }
        }

        public ProxyInputActions(InputActions actions)
        {
            Jump = new ProxyPlayerAction(actions.Jump);
            Dash = new ProxyPlayerAction(actions.Dash);
            Attack = new ProxyPlayerAction(actions.Attack);
            Special1 = new ProxyPlayerAction(actions.Special1);
            Special2 = new ProxyPlayerAction(actions.Special2);
            Up = new ProxyPlayerAction(actions.UpInput);
            Down = new ProxyPlayerAction(actions.DownInput);
            Left = new ProxyPlayerAction(actions.LeftInput);
            Right = new ProxyPlayerAction(actions.RightInput);
            Sprint = new ProxyPlayerAction(actions.Sprint);
            Interact = new ProxyPlayerAction(actions.Interact);

            _proxyActions = new List<ProxyPlayerAction>()
            {
                Jump,
                Dash,
                Attack,
                Special1,
                Special2,
                Up,
                Down,
                Left,
                Right,
                Sprint,
                Interact
            };
        }

        public void UpdateData()
        {
            Jump.UpdateData();
            Dash.UpdateData();
            Attack.UpdateData();
            Special1.UpdateData();
            Special2.UpdateData();
            Up.UpdateData();
            Down.UpdateData();
            Left.UpdateData();
            Right.UpdateData();
            Sprint.UpdateData();
            Interact.UpdateData();
        }

        public void Reset()
        {
            Jump.Reset();
            Dash.Reset();
            Attack.Reset();
            Special1.Reset();
            Special2.Reset();
            Up.Reset();
            Down.Reset();
            Left.Reset();
            Right.Reset();
            Sprint.Reset();
            Interact.Reset();
        }
    }
}