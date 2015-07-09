using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class ButtonControl : Control
    {
        public Brush Background { get; set; }

        public Brush Hovered { get; set; }

        public string Text { get; set; }

        public SpriteFont Font { get; set; }

        public Color Color { get; set; }

        public ButtonControl(IScreenManager screenManager) : base(screenManager) { }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            if (IsHovered)
            {
                if (Hovered != null)
                    Hovered.Draw(batch, new Rectangle(Position.X, Position.Y, Size.X, Size.Y));
                else if (Background != null)
                    Background.Draw(batch, new Rectangle(Position.X, Position.Y, Size.X, Size.Y));
            }
            else
            {
                if (Background != null)
                    Background.Draw(batch, new Rectangle(Position.X, Position.Y, Size.X, Size.Y));
            }

            batch.Begin();
            Vector2 textSize = Font.MeasureString(Text);
            batch.DrawString(Font, Text, new Vector2(
                Position.X + ((Size.X - textSize.X) / 2),
                Position.Y + ((Size.Y - textSize.Y) / 2)), Color);
            batch.End();
        }
    }
}
