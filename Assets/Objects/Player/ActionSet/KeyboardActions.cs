﻿using InControl;
using UnityEngine;

namespace RogueLiteInput
{
    /// <summary>
    /// Purpose: To set keyboard keys.
    /// Creator:Mikkel Nielsen.
    /// </summary>
    public class KeyboardActions : InputActions
    {
        public KeyboardActions()
        {
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