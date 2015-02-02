using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class MouseInput : IInputSet
    {
        private float mouseSpeed = 5f;

        private Game game;

        public float MoveX { get; private set; }
        public float MoveY { get; private set; }
        public float HeadX { get; private set; }
        public float HeadY { get; private set; }
        public bool Interact { get; private set; }

        public MouseInput(Game game)
        {
            this.game = game;

            //int centerX = game.GraphicsDevice.Viewport.Width / 2;
            //int centerY = game.GraphicsDevice.Viewport.Height / 2;
            //Mouse.SetPosition(centerX, centerY);
        }

        public void Update()
        {
            MouseState state = Mouse.GetState();

            int centerX = game.GraphicsDevice.Viewport.Width / 2;
            int centerY = game.GraphicsDevice.Viewport.Height / 2;

            int deltaX = state.Position.X - centerX;
            int deltaY = state.Position.Y - centerY;

            HeadX = (float)deltaX / mouseSpeed;
            HeadY = (float)-deltaY / mouseSpeed;

            Mouse.SetPosition(centerX, centerY);
        }
    }
}
