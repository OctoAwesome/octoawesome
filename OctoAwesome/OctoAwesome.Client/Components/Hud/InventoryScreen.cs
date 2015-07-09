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
        private Texture2D panelTexture;

        private PanelControl panel;
        private LabelControl headline;

        public InventoryScreen(IScreenManager screenManager) : base(screenManager)
        {

        }

        public override void LoadContent()
        {
            panelTexture = ScreenManager.Content.Load<Texture2D>("Textures/panel");

            Controls.Add(panel = new PanelControl(ScreenManager));

            panel.Background = new SolidColorBrush(ScreenManager) { Color = new Color(211, 191, 143) };
            // panel.Background = new NineTileBrush(panelTexture, 30);
            panel.Position = new Index2(
                (ScreenManager.ScreenSize.X - 600) / 2,
                (ScreenManager.ScreenSize.Y - 400) / 2);
            panel.Size = new Index2(600, 400);

            headline = new LabelControl(ScreenManager) 
            {
                Font = ScreenManager.NormalText,
                Text = "Inventory",
                Color = Color.Black,
                Position = new Index2(
                    ((ScreenManager.ScreenSize.X - 600) / 2) + 100,
                    ((ScreenManager.ScreenSize.Y - 400) / 2) + 40),
            };
            Controls.Add(headline);

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
