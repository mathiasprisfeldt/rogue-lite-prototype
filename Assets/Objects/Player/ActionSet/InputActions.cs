﻿using UnityEngine;
using InControl;

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
        public PlayerAction Special { get; set; }
        public PlayerAction Sprint { get; set; }

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
            get{ return DeadZoneVertical(_verticalDeadZone); }
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
      
            LeftInput = CreatePlayerAction("Left");
            RightInput = CreatePlayerAction("Right");
            Jump = CreatePlayerAction("Jump");
            DownInput = CreatePlayerAction("Down");
            Dash = CreatePlayerAction("Dash");
            UpInput = CreatePlayerAction("Up");
            Attack = CreatePlayerAction("Attack");
            Special = CreatePlayerAction("Special");
            Sprint = CreatePlayerAction("Sprint");

            RawHorizontal = CreateOneAxisPlayerAction(LeftInput, RightInput);
            RawVertical = CreateOneAxisPlayerAction(DownInput, UpInput);
            ProxyInputActions = new ProxyInputActions();

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
            Jump.AddDefaultBinding(Key.S);
            Jump.AddDefaultBinding(Key.None);

            //Dash
            Dash.AddDefaultBinding(Key.Shift);
            Dash.AddDefaultBinding(Key.None);

            //Attack
            Attack.AddDefaultBinding(Key.D);
            Attack.AddDefaultBinding(Key.None);

            //Special
            Special.AddDefaultBinding(Key.W);
            Special.AddDefaultBinding(Key.None);

            //Sprint
            Sprint.AddDefaultBinding(Key.None);
            Sprint.AddDefaultBinding(Key.None);

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
            Special.AddDefaultBinding(InputControlType.Action4);
            Attack.AddDefaultBinding(Key.None);

            //Sprint
            Sprint.AddDefaultBinding(InputControlType.None);
            Sprint.AddDefaultBinding(InputControlType.None);

        }

        public void UpdateProxy()
        {
            ProxyInputActions.UpdateData(this);
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

        public void UpdateData(PlayerAction action)
        {
            WasPressed = WasPressed || action.WasPressed;
            WasReleased = WasReleased || action.WasReleased;
            IsPressed = action.IsPressed;
            WasRepeated = WasRepeated || action.WasRepeated;
            _wasreset = false;
        }

        public void Reset()
        {
            if(_wasreset)
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
        public ProxyPlayerAction Special { get; set; }
        public ProxyPlayerAction Up { get; set; }
        public ProxyPlayerAction Down { get; set; }
        public ProxyPlayerAction Left { get; set; }
        public ProxyPlayerAction Right { get; set; }
        public ProxyPlayerAction Sprint { get; set; }


        public ProxyInputActions()
        {
            Jump = new ProxyPlayerAction();
            Dash = new ProxyPlayerAction();
            Attack = new ProxyPlayerAction();
            Special = new ProxyPlayerAction();
            Up = new ProxyPlayerAction();
            Down = new ProxyPlayerAction();
            Left = new ProxyPlayerAction();
            Right = new ProxyPlayerAction();
            Sprint = new ProxyPlayerAction();
        }

        public void UpdateData(InputActions actions)
        {
            Jump.UpdateData(actions.Jump);
            Dash.UpdateData(actions.Dash);
            Attack.UpdateData(actions.Attack);
            Special.UpdateData(actions.Special);
            Up.UpdateData(actions.UpInput);
            Down.UpdateData(actions.DownInput);
            Left.UpdateData(actions.LeftInput);
            Right.UpdateData(actions.RightInput);
            Sprint.UpdateData(actions.Sprint);
        }

        public void Reset()
        {
            Jump.Reset();
            Dash.Reset();
            Attack.Reset();
            Special.Reset();
            Up.Reset();
            Down.Reset();
            Left.Reset();
            Right.Reset();
            Sprint.Reset();
        }
    }
}