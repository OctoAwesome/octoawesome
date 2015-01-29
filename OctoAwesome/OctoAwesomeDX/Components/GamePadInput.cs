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

        public bool HeadLeft { get; private set; }

        public bool HeadRight { get; private set; }

        public bool HeadUp { get; private set; }

        public bool HeadDown { get; private set; }

        public bool Interact { get; private set; }

        public void Update()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            Interact = gamePadState.Buttons.A == ButtonState.Pressed;
            Left = gamePadState.ThumbSticks.Left.X < -0.5f;
            Right = gamePadState.ThumbSticks.Left.X > 0.5f;
            Down = gamePadState.ThumbSticks.Left.Y < -0.5f;
            Up = gamePadState.ThumbSticks.Left.Y > 0.5f;
            HeadLeft = gamePadState.ThumbSticks.Right.X < -0.5f;
            HeadRight = gamePadState.ThumbSticks.Right.X > 0.5f;
            HeadDown = gamePadState.ThumbSticks.Right.Y < -0.5f;
            HeadUp = gamePadState.ThumbSticks.Right.Y > 0.5f;

        }
    }
}
