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
    public abstract class InputActions : PlayerActionSet
    {
        protected ActionsSetType _activeActionSetType;

        //PlayerActions
        public PlayerAction Left { get; set; }
        public PlayerAction Right { get; set; }
        public PlayerAction Jump { get; set; }
        public PlayerAction Down { get; set; }
        public PlayerAction Dash { get; set; }
        public PlayerAction Up { get; set; }

        //Directions
        public PlayerOneAxisAction HorizontalDirection { get; set; }

        public InputActions()
        {         
            Left = CreatePlayerAction("Left");
            Right = CreatePlayerAction("Right");
            Jump = CreatePlayerAction("Jump");
            Down = CreatePlayerAction("Down");
            Dash = CreatePlayerAction("Dash");
            Up = CreatePlayerAction("Up");

            //Directions
            HorizontalDirection = CreateOneAxisPlayerAction(Left, Right);
        }

    }
}