using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class MiniMap : Control
    {
        Texture2D pix;

        public MiniMap(HudComponent hud):base(hud)
        {

        }

        public override void LoadContent()
        {
            pix = Hud.Game.Content.Load<Texture2D>("Textures/pix");
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            batch.Begin();
            batch.Draw(pix, new Rectangle(Position.X - 2, Position.Y - 2, Size.X + 4, Size.Y + 4), Color.Black);
            batch.Draw(Hud.Scene.MiniMapTexture, new Rectangle(Position.X, Position.Y, Size.X, Size.Y), Color.White);

            Index2 center = (Size / 2) + Position;

            batch.Draw(pix, new Rectangle(center.X - 1, center.Y - 1, 2, 2), Color.White);
            batch.End();
        }
    }
}
