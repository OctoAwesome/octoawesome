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
        private MouseInput mouse;

        public float MoveX { get; private set; }
        public float MoveY { get; private set; }
        public float HeadX { get; private set; }
        public float HeadY { get; private set; }
        public bool Interact { get; private set; }

        public InputComponent(Game game)
            : base(game)
        {
            gamepad = new GamePadInput();
            keyboard = new KeyboardInput();
            mouse = new MouseInput(game);
        }

        public override void Update(GameTime gameTime)
        {

            bool nextInteract = false;
            MoveX = 0f;
            MoveY = 0f;
            HeadX = 0f;
            HeadY = 0f;

            gamepad.Update();
            nextInteract = gamepad.Interact;

            MoveX += gamepad.MoveX;
            MoveY += gamepad.MoveY;
            HeadX += gamepad.HeadX;
            HeadY += gamepad.HeadY;

            keyboard.Update();
            nextInteract |= keyboard.Interact;

            MoveX += keyboard.MoveX;
            MoveY += keyboard.MoveY;
            HeadX += keyboard.HeadX;
            HeadY += keyboard.HeadY;

            // TODO: Mouse
            mouse.Update();
            nextInteract |= mouse.Interact;

            MoveX += mouse.MoveX;
            MoveY += mouse.MoveY;
            HeadX += mouse.HeadX;
            HeadY += mouse.HeadY;

            MoveX = Math.Min(1, Math.Max(-1, MoveX));
            MoveY = Math.Min(1, Math.Max(-1, MoveY));
            HeadX = Math.Min(1, Math.Max(-1, HeadX));
            HeadY = Math.Min(1, Math.Max(-1, HeadY));

            if (nextInteract && !lastInteract)
                Interact = true;
            else
                Interact = false;
            lastInteract = nextInteract;
        }
    }
}
