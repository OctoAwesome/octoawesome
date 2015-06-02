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
        private bool lastSlot1 = false;
        private bool lastSlot2 = false;
        private bool lastSlot3 = false;
        private bool lastSlot4 = false;
        private bool lastSlot5 = false;
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

        public bool Slot1Trigger { get; private set; }

        public bool Slot2Trigger { get; private set; }

        public bool Slot3Trigger { get; private set; }

        public bool Slot4Trigger { get; private set; }

        public bool Slot5Trigger { get; private set; }

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
            bool nextSlot1 = false;
            bool nextSlot2 = false;
            bool nextSlot3 = false;
            bool nextSlot4 = false;
            bool nextSlot5 = false;
            MoveX = 0f;
            MoveY = 0f;
            HeadX = 0f;
            HeadY = 0f;

            gamepad.Update();
            nextInteract = gamepad.InteractTrigger;
            nextApply = gamepad.ApplyTrigger;
            nextJump = gamepad.JumpTrigger;
            nextSlot1 = gamepad.Slot1Trigger;
            nextSlot2 = gamepad.Slot2Trigger;
            nextSlot3 = gamepad.Slot3Trigger;
            nextSlot4 = gamepad.Slot4Trigger;
            nextSlot5 = gamepad.Slot5Trigger;

            MoveX += gamepad.MoveX;
            MoveY += gamepad.MoveY;
            HeadX += gamepad.HeadX;
            HeadY += gamepad.HeadY;

            keyboard.Update();
            nextInteract |= keyboard.InteractTrigger;
            nextApply |= keyboard.ApplyTrigger;
            nextJump |= keyboard.JumpTrigger;
            nextSlot1 |= keyboard.Slot1Trigger;
            nextSlot2 |= keyboard.Slot2Trigger;
            nextSlot3 |= keyboard.Slot3Trigger;
            nextSlot4 |= keyboard.Slot4Trigger;
            nextSlot5 |= keyboard.Slot5Trigger;

            MoveX += keyboard.MoveX;
            MoveY += keyboard.MoveY;
            HeadX += keyboard.HeadX;
            HeadY += keyboard.HeadY;

            // Mouse
            if (Game.IsActive)
                mouse.Update();
            nextInteract |= mouse.InteractTrigger;
            nextApply |= mouse.ApplyTrigger;
            nextJump |= mouse.JumpTrigger;
            nextSlot1 |= mouse.Slot1Trigger;
            nextSlot2 |= mouse.Slot2Trigger;
            nextSlot3 |= mouse.Slot3Trigger;
            nextSlot4 |= mouse.Slot4Trigger;
            nextSlot5 |= mouse.Slot5Trigger;


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

            if (nextSlot1 && !lastSlot1)
                Slot1Trigger = true;
            else
                Slot1Trigger = false;
            lastSlot1 = nextSlot1;

            if (nextSlot2 && !lastSlot2)
                Slot2Trigger = true;
            else
                Slot2Trigger = false;
            lastSlot2 = nextSlot2;

            if (nextSlot3 && !lastSlot3)
                Slot3Trigger = true;
            else
                Slot3Trigger = false;
            lastSlot3 = nextSlot3;

            if (nextSlot4 && !lastSlot4)
                Slot4Trigger = true;
            else
                Slot4Trigger = false;
            lastSlot4 = nextSlot4;

            if (nextSlot5 && !lastSlot5)
                Slot5Trigger = true;
            else
                Slot5Trigger = false;
            lastSlot5 = nextSlot5;
        }


        
    }
}
