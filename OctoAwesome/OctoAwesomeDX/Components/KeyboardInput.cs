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

        public bool Interact { get; private set; }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            Interact = keyboardState.IsKeyDown(Keys.Space);
            Left = keyboardState.IsKeyDown(Keys.Left);
            Right = keyboardState.IsKeyDown(Keys.Right);
            Up = keyboardState.IsKeyDown(Keys.Up);
            Down = keyboardState.IsKeyDown(Keys.Down);
        }
    }
}
