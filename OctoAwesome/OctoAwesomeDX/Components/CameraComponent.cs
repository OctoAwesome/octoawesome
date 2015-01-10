using Microsoft.Xna.Framework;
using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Components
{
    internal sealed class CameraComponent : DrawableGameComponent
    {
        private WorldComponent world;
        private InputComponent input;
        private Vector2 renderSize;

        public readonly float MAXSPEED = 1000f;

        public readonly float SCALE = 64f;

        public CameraComponent(Game game, WorldComponent world, InputComponent input)
            : base(game)
        {
            this.world = world;
            this.input = input;
        }

        public void SetRenderSize(Vector2 renderSize)
        {
            this.renderSize = renderSize;
            RecalcViewPort();
        }

        protected override void LoadContent()
        {
            SetRenderSize(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
        }

        public override void Update(GameTime frameTime)
        {
            //Vector2 velocity = new Vector2(
            //    (input.CamLeft ? -1f : 0f) + (input.CamRight ? 1f : 0f),
            //    (input.CamUp ? -1f : 0f) + (input.CamDown ? 1f : 0f));

            //velocity = velocity.Normalized();

            //Center += (velocity * MAXSPEED * (float)frameTime.TotalSeconds);

            float posX = (world.World.Player.Position.X * SCALE) - ViewPort.Left;
            float posY = (world.World.Player.Position.Y * SCALE) - ViewPort.Top;

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

            if (Center.X > (world.World.PlaygroundSize.X * SCALE) - (ViewPort.Width / 2) + 100)
                Center = new Vector2((world.World.PlaygroundSize.X * SCALE) - (ViewPort.Width / 2) + 100, Center.Y);

            if (Center.Y > (world.World.PlaygroundSize.Y * SCALE) - (ViewPort.Height / 2) + 100)
                Center = new Vector2(Center.X, (world.World.PlaygroundSize.Y * SCALE) - (ViewPort.Height / 2) + 100);

            RecalcViewPort();
        }

        private void RecalcViewPort()
        {
            float offsetX = (Center.X) - (this.renderSize.X / 2);
            float offsetY = (Center.Y) - (this.renderSize.Y / 2);

            ViewPort = new Rectangle((int)offsetX, (int)offsetY, (int)renderSize.X, (int)renderSize.Y);
        }

        /// <summary>
        /// Kameraposition (Render-Koordinate)
        /// </summary>
        public Vector2 Center { get; private set; }

        /// <summary>
        /// Sichtbarer Bereich (Render-Koordinate)
        /// </summary>
        public Rectangle ViewPort { get; set; }
    }
}
