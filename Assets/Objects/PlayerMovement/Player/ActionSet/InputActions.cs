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
        private float _deadZone = .4f;

        protected ActionsSetType _activeActionSetType;

        //PlayerActions
        public PlayerAction LeftInput { get; set; }
        public PlayerAction RightInput { get; set; }
        public PlayerAction Jump { get; set; }
        public PlayerAction DownInput { get; set; }
        public PlayerAction Dash { get; set; }
        public PlayerAction UpInput { get; set; }
        public PlayerAction Attack { get; set; }

        public PlayerOneAxisAction Horizontal { get; set; }
        public PlayerOneAxisAction Vertical { get; set; }

        public bool Up
        {
            get
            {
                return Vertical.Value > _deadZone;
            }
        }
        public bool Down
        {
            get
            {
                return Vertical.Value < -_deadZone;
            }
        }
        public bool Right
        {
            get { return Horizontal.Value > _deadZone; }
        }
        public bool Left
        {
            get { return Horizontal.Value < -_deadZone; }
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

            Horizontal = CreateOneAxisPlayerAction(LeftInput, RightInput);
            Vertical = CreateOneAxisPlayerAction(DownInput, UpInput);

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