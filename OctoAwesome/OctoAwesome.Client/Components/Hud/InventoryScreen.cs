using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class InventoryScreen : Screen
    {
        private Texture2D pix;

        public InventoryScreen(HudComponent hud) : base(hud)
        {

        }

        public override void LoadContent()
        {
            pix = Hud.Game.Content.Load<Texture2D>("Textures/pix");
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            batch.Begin();
            batch.Draw(pix, new Rectangle(0, 0, Hud.Game.GraphicsDevice.Viewport.Width, Hud.Game.GraphicsDevice.Viewport.Height), Color.Red);
            batch.End();
        }
    }
}
