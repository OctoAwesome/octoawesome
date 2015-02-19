using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class KeyboardInput : IInputSet
    {
        public float MoveX { get; private set; }
        public float MoveY { get; private set; }
        public float HeadX { get; private set; }
        public float HeadY { get; private set; }
        public bool InteractTrigger { get; private set; }
        public bool JumpTrigger { get; private set; }


        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            MoveX = 0f;
            MoveY = 0f;
            HeadX = 0f;
            HeadY = 0f;
            InteractTrigger = keyboardState.IsKeyDown(Keys.E);
            JumpTrigger = keyboardState.IsKeyDown(Keys.Space);
            MoveX -= (keyboardState.IsKeyDown(Keys.A) ? 1 : 0);
            MoveX += (keyboardState.IsKeyDown(Keys.D) ? 1 : 0);
            MoveY -= (keyboardState.IsKeyDown(Keys.S) ? 1 : 0);
            MoveY += (keyboardState.IsKeyDown(Keys.W) ? 1 : 0);
            HeadX -= (keyboardState.IsKeyDown(Keys.Left) ? 1 : 0);
            HeadX += (keyboardState.IsKeyDown(Keys.Right) ? 1 : 0);
            HeadY -= (keyboardState.IsKeyDown(Keys.Up) ? 1 : 0);
            HeadY += (keyboardState.IsKeyDown(Keys.Down) ? 1 : 0);
        }
    }
}
