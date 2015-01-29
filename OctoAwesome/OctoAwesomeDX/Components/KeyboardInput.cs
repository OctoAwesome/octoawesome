using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class KeyboardInput : IInputSet
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
            KeyboardState keyboardState = Keyboard.GetState();

            Interact = keyboardState.IsKeyDown(Keys.Space);
            Left = keyboardState.IsKeyDown(Keys.A);
            Right = keyboardState.IsKeyDown(Keys.D);
            Up = keyboardState.IsKeyDown(Keys.W);
            Down = keyboardState.IsKeyDown(Keys.S);
            HeadLeft = keyboardState.IsKeyDown(Keys.Left);
            HeadRight = keyboardState.IsKeyDown(Keys.Right);
            HeadUp = keyboardState.IsKeyDown(Keys.Up);
            HeadDown = keyboardState.IsKeyDown(Keys.Down);
        }
    }
}
