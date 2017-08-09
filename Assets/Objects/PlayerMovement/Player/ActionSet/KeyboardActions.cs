using InControl;
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
            //Left
            Left.AddDefaultBinding(Key.A);
            Left.AddDefaultBinding(Key.None);

            //Right
            Right.AddDefaultBinding(Key.D);
            Right.AddDefaultBinding(Key.None);

            //Down
            Down.AddDefaultBinding(Key.S);
            Down.AddDefaultBinding(Key.None);

            //Down
            Up.AddDefaultBinding(Key.W);
            Up.AddDefaultBinding(Key.None);

            //Jump
            Jump.AddDefaultBinding(Key.Space);
            Jump.AddDefaultBinding(Key.None);

            //Dash
            Dash.AddDefaultBinding(Key.LeftShift);
            Dash.AddDefaultBinding(Key.None);

        }
    }
}