using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal class GamePadInput : IInputSet
    {
        public bool Left { get; private set; }

        public bool Right { get; private set; }

        public bool Up { get; private set; }

        public bool Down { get; private set; }

        public bool Interact { get; private set; }

        public void Update()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            Interact = gamePadState.Buttons.A == ButtonState.Pressed;
            Left = gamePadState.ThumbSticks.Left.X < -0.5f;
            Right = gamePadState.ThumbSticks.Left.X > 0.5f;
            Down = gamePadState.ThumbSticks.Left.Y < -0.5f;
            Up = gamePadState.ThumbSticks.Left.Y > 0.5f;
        }
    }
}
