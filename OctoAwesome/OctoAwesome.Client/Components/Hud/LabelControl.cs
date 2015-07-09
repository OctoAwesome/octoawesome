using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class LabelControl : Control
    {
        public string Text { get; set; }

        public SpriteFont Font { get; set; }

        public Color Color { get; set; }

        public LabelControl(IScreenManager screenManager) : base(screenManager) { }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            batch.Begin();
            batch.DrawString(Font, Text, new Vector2(Position.X, Position.Y), Color);
            batch.End();
        }
    }
}
