using System.Collections;
using System.Collections.Generic;
using InControl;
using RogueLiteInput;
using UnityEngine;

public class GamepadActions : InputActions {

    public GamepadActions()
    {

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
