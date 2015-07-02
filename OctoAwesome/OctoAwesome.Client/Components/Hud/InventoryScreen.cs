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
        private Texture2D panelTexture;

        private PanelControl panel;

        public InventoryScreen(HudComponent hud) : base(hud)
        {

        }

        public override void LoadContent()
        {
            pix = Hud.Game.Content.Load<Texture2D>("Textures/pix");
            panelTexture = Hud.Game.Content.Load<Texture2D>("Textures/panel");

            Controls.Add(panel = new PanelControl(Hud));

            panel.BackgroundTexture = panelTexture;
            panel.Position = new Index2(
                (Hud.GraphicsDevice.Viewport.Width - 600) / 2, 
                (Hud.GraphicsDevice.Viewport.Height - 400) / 2);
            panel.Size = new Index2(600, 400);

            foreach (var control in Controls)
                control.LoadContent();
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            foreach (var control in Controls)
                control.Draw(batch, gameTime);
        }
    }
}
