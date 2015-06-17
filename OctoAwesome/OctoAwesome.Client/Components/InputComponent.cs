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
        private const int SlotTriggerLength = 10;

        private bool lastInteract = false;
        private bool lastJump = false;
        private bool[] lastSlotTrigger = new bool[SlotTriggerLength];
        private bool lastApply = false;
        private bool lastSlotLeftTrigger = false;
        private bool lastSlotRightTrigger = false;
        private GamePadInput gamepad;
        private KeyboardInput keyboard;
        private MouseInput mouse;

        private bool[] slotTriggers = new bool[SlotTriggerLength];

        public float MoveX { get; private set; }
        public float MoveY { get; private set; }
        public float HeadX { get; private set; }
        public float HeadY { get; private set; }
        public bool InteractTrigger { get; private set; }
        public bool ApplyTrigger { get; private set; }
        public bool JumpTrigger { get; private set; }

        public bool[] SlotTrigger { get { return slotTriggers; } }

        public bool SlotLeftTrigger { get; private set; }

        public bool SlotRightTrigger { get; private set; }

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
            bool[] nextSlot = new bool[SlotTriggerLength];
            bool nextSlotLeft = false;
            bool nextSlotRight = false;
            MoveX = 0f;
            MoveY = 0f;
            HeadX = 0f;
            HeadY = 0f;

            gamepad.Update();
            nextInteract = gamepad.InteractTrigger;
            nextApply = gamepad.ApplyTrigger;
            nextJump = gamepad.JumpTrigger;
            nextSlotLeft = gamepad.SlotLeftTrigger;
            nextSlotRight = gamepad.SlotRightTrigger;
            if (gamepad.SlotTrigger != null)
                for (int i = 0; i < Math.Min(gamepad.SlotTrigger.Length, SlotTriggerLength); i++)
                    nextSlot[i] = gamepad.SlotTrigger[i];

            MoveX += gamepad.MoveX;
            MoveY += gamepad.MoveY;
            HeadX += gamepad.HeadX;
            HeadY += gamepad.HeadY;

            keyboard.Update();
            nextInteract |= keyboard.InteractTrigger;
            nextApply |= keyboard.ApplyTrigger;
            nextJump |= keyboard.JumpTrigger;
            nextSlotLeft |= keyboard.SlotLeftTrigger;
            nextSlotRight |= keyboard.SlotRightTrigger;
            if (keyboard.SlotTrigger != null)
                for (int i = 0; i < Math.Min(keyboard.SlotTrigger.Length, SlotTriggerLength); i++)
                    nextSlot[i] |= keyboard.SlotTrigger[i];


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
            nextSlotLeft |= mouse.SlotLeftTrigger;
            nextSlotRight |= mouse.SlotRightTrigger;
            if (mouse.SlotTrigger != null)
                for (int i = 0; i < Math.Min(mouse.SlotTrigger.Length, SlotTriggerLength); i++)
                    nextSlot[i] |= mouse.SlotTrigger[i];

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

            SlotLeftTrigger = nextSlotLeft && !lastSlotLeftTrigger;
            lastSlotLeftTrigger = nextSlotLeft;

            SlotRightTrigger = nextSlotRight && !lastSlotRightTrigger;
            lastSlotRightTrigger = nextSlotRight;

            for (int i = 0; i < SlotTriggerLength; i++)
            {
                if (nextSlot[i] && lastSlotTrigger[i])
                    slotTriggers[i] = true;
                else
                    slotTriggers[i] = false;
                lastSlotTrigger[i] = nextSlot[i];
            }
        }
    }
}
