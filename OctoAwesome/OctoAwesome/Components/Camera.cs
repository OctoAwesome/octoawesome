using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Components
{
    internal sealed class Camera
    {
        private Game game;
        private Input input;
        private Vector2 renderSize;

        public readonly float MAXSPEED = 1000f;

        // public readonly float SCALE = 10f;

        public Camera(Game game, Input input)
        {
            this.game = game;
            this.input = input;
        }

        public void SetRenderSize(Vector2 renderSize)
        {
            this.renderSize = renderSize;
            RecalcViewPort();
        }

        public void Update(TimeSpan frameTime)
        {
            //Vector2 velocity = new Vector2(
            //    (input.CamLeft ? -1f : 0f) + (input.CamRight ? 1f : 0f),
            //    (input.CamUp ? -1f : 0f) + (input.CamDown ? 1f : 0f));

            //velocity = velocity.Normalized();

            //Center += (velocity * MAXSPEED * (float)frameTime.TotalSeconds);

            float posX = game.Player.Position.X - ViewPort.Left;
            float posY = game.Player.Position.Y - ViewPort.Top;

            float frameX = ViewPort.Width / 4;
            float frameY = ViewPort.Height / 4;

            if (posX < frameX)
                Center = new Vector2(Center.X - (frameX - posX), Center.Y);

            if (posX > ViewPort.Width - frameX)
                Center = new Vector2(Center.X + (posX - (ViewPort.Width - frameX)), Center.Y);

            if (posY < frameY)
                Center = new Vector2(Center.X, Center.Y - (frameY - posY));

            if (posY > ViewPort.Height - frameY)
                Center = new Vector2(Center.X, Center.Y + (posY - (ViewPort.Height - frameY)));


            if (Center.X < (ViewPort.Width / 2) - 100)
                Center = new Vector2((ViewPort.Width / 2) - 100, Center.Y);

            if (Center.Y < (ViewPort.Height / 2) - 100)
                Center = new Vector2(Center.X, (ViewPort.Height / 2) - 100);

            if (Center.X > game.PlaygroundSize.X - (ViewPort.Width / 2) + 100)
                Center = new Vector2(game.PlaygroundSize.X - (ViewPort.Width / 2) + 100, Center.Y);

            if (Center.Y > game.PlaygroundSize.Y - (ViewPort.Height / 2) + 100)
                Center = new Vector2(Center.X, game.PlaygroundSize.Y - (ViewPort.Height / 2) + 100);

            RecalcViewPort();
        }

        private void RecalcViewPort()
        {
            float offsetX = game.Camera.Center.X - (this.renderSize.X / 2);
            float offsetY = game.Camera.Center.Y - (this.renderSize.Y / 2);

            ViewPort = new RectangleF(offsetX, offsetY, renderSize.X, renderSize.Y);
        }

        public Vector2 Center { get; private set; }

        public RectangleF ViewPort { get; set; }
    }
}
