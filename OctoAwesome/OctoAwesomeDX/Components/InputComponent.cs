using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class InputComponent : GameComponent, IInputSet
    {
        private bool lastInteract = false;
        private GamePadInput gamepad;
        private KeyboardInput keyboard;

        public bool Left { get; private set; }

        public bool Right { get; private set; }

        public bool Up { get; private set; }

        public bool Down { get; private set; }

        public bool Interact { get; private set; }

        public InputComponent(Game game)
            : base(game)
        {
            gamepad = new GamePadInput();
            keyboard = new KeyboardInput()
        }

        public override void Update(GameTime gameTime)
        {

            bool nextInteract = false;

            gamepad.Update();
            nextInteract = gamepad.Interact;
            Left = gamepad.Left;
            Right = gamepad.Right;
            Down = gamepad.Down;
            Up = gamepad.Up;

            keyboard.Update();
            nextInteract |= gamepad.Interact;
            Left |= gamepad.Left;
            Right |= gamepad.Right;
            Down |= gamepad.Down;
            Up |= gamepad.Up;

            if (nextInteract && !lastInteract)
                Interact = true;
            else
                Interact = false;
            lastInteract = nextInteract;
        }
    }
}
