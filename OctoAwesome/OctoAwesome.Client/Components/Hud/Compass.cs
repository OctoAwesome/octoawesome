using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class Compass : Control
    {
        private Texture2D compassTexture;

        public Compass(HudComponent hud) : base(hud)
        {

        }

        public override void LoadContent()
        {
            compassTexture = Hud.Game.Content.Load<Texture2D>("Textures/compass");
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            batch.Begin();

            batch.Draw(compassTexture, new Vector2(0, 0), Color.White);

            batch.End();
        }
    }
}
