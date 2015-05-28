using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal sealed class InputComponent : GameComponent, IInputSet
    {
        private bool lastInteract = false;
        private bool lastJump = false;
        private bool lastApply = false;
        private GamePadInput gamepad;
        private KeyboardInput keyboard;
        private MouseInput mouse;

        public float MoveX { get; private set; }
        public float MoveY { get; private set; }
        public float HeadX { get; private set; }
        public float HeadY { get; private set; }
        public bool InteractTrigger { get; private set; }
        public bool ApplyTrigger { get; private set; }
        public bool JumpTrigger { get; private set; }

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
            bool nextJump = false;
            bool nextApply = false;
            MoveX = 0f;
            MoveY = 0f;
            HeadX = 0f;
            HeadY = 0f;

            gamepad.Update();
            nextInteract = gamepad.InteractTrigger;
            nextApply = gamepad.ApplyTrigger;
            nextJump = gamepad.JumpTrigger;

            MoveX += gamepad.MoveX;
            MoveY += gamepad.MoveY;
            HeadX += gamepad.HeadX;
            HeadY += gamepad.HeadY;

            keyboard.Update();
            nextInteract |= keyboard.InteractTrigger;
            nextApply |= keyboard.ApplyTrigger;
            nextJump |= keyboard.JumpTrigger;

            MoveX += keyboard.MoveX;
            MoveY += keyboard.MoveY;
            HeadX += keyboard.HeadX;
            HeadY += keyboard.HeadY;

            // Mouse
            //if (Game.IsActive)
            //    mouse.Update();
            nextInteract |= mouse.InteractTrigger;
            nextApply |= mouse.ApplyTrigger;
            nextJump |= mouse.JumpTrigger;

            MoveX += mouse.MoveX;
            MoveY += mouse.MoveY;
            HeadX += mouse.HeadX;
            HeadY += mouse.HeadY;

            MoveX = Math.Min(1, Math.Max(-1, MoveX));
            MoveY = Math.Min(1, Math.Max(-1, MoveY));
            //HeadX = Math.Min(1, Math.Max(-1, HeadX));
            //HeadY = Math.Min(1, Math.Max(-1, HeadY));

            if (nextInteract && !lastInteract)
                InteractTrigger = true;
            else
                InteractTrigger = false;
            lastInteract = nextInteract;

            if (nextApply && !lastApply)
                ApplyTrigger = true;
            else
                ApplyTrigger = false;
            lastApply = nextApply;

            if (nextJump && !lastJump)
                JumpTrigger = true;
            else
                JumpTrigger = false;
            lastJump = nextJump;
        }
    }
}
