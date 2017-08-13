using UnityEngine;
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

        public PlayerOneAxisAction RawHorizontal { get; set; }
        public PlayerOneAxisAction RawVertical { get; set; }

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

            RawHorizontal = CreateOneAxisPlayerAction(LeftInput, RightInput);
            RawVertical = CreateOneAxisPlayerAction(DownInput, UpInput);

            //Keyboard inputs
            //Left keyboard
            LeftInput.AddDefaultBinding(Key.A);
            LeftInput.AddDefaultBinding(Key.None);

            //Right
            RightInput.AddDefaultBinding(Key.D);
            RightInput.AddDefaultBinding(Key.None);

            //Down
            DownInput.AddDefaultBinding(Key.S);
            DownInput.AddDefaultBinding(Key.None);

            //Up
            UpInput.AddDefaultBinding(Key.W);
            UpInput.AddDefaultBinding(Key.None);

            //Jump
            Jump.AddDefaultBinding(Key.Space);
            Jump.AddDefaultBinding(Key.None);

            //Dash
            Dash.AddDefaultBinding(Key.LeftShift);
            Dash.AddDefaultBinding(Key.None);

            //Attack
            Attack.AddDefaultBinding(Key.J);
            Attack.AddDefaultBinding(Key.None);


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
            Dash.AddDefaultBinding(InputControlType.RightBumper);
            Dash.AddDefaultBinding(InputControlType.RightTrigger);

            //Attack
            Attack.AddDefaultBinding(InputControlType.Action2);
            Attack.AddDefaultBinding(Key.None);

        }

    }
}