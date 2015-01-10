using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    public class Input2 : GameComponent
    {
        public bool Left { get; private set; }

        public bool Right { get; private set; }

        public bool Up { get; private set; }

        public bool Down { get; private set; }

        public bool Interact { get; set; }

        public Input2(Game game) : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            Interact = gamePadState.Buttons.A == ButtonState.Pressed;
            Left = gamePadState.ThumbSticks.Left.X < -0.5f;
            Right = gamePadState.ThumbSticks.Left.X > 0.5f;
            Down = gamePadState.ThumbSticks.Left.Y < -0.5f;
            Up = gamePadState.ThumbSticks.Left.Y > 0.5f;

            KeyboardState keyboardState = Keyboard.GetState();

            Interact |= keyboardState.IsKeyDown(Keys.Space);
            Left |= keyboardState.IsKeyDown(Keys.Left);
            Right |= keyboardState.IsKeyDown(Keys.Right);
            Up |= keyboardState.IsKeyDown(Keys.Up);
            Down |= keyboardState.IsKeyDown(Keys.Down);
        }
    }
}
