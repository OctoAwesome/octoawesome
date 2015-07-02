using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class PanelControl : Control
    {
        public bool Tiles { get; set; }

        public Texture2D BackgroundTexture { get; set; }

        public PanelControl(HudComponent hud)
            : base(hud)
        {
            Tiles = false;
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            if (BackgroundTexture == null)
                return;

            if (Tiles)
            {
                batch.Begin(samplerState: SamplerState.LinearWrap);
                batch.Draw(BackgroundTexture, 
                    new Rectangle(Position.X, Position.Y, Size.X, Size.Y), 
                    new Rectangle(Position.X, Position.Y, Size.X, Size.Y), 
                    Color.White);
            }
            else
            {
                batch.Begin();
                batch.Draw(BackgroundTexture,
                    new Rectangle(Position.X, Position.Y, Size.X, Size.Y),
                    new Rectangle(0, 0, BackgroundTexture.Width, BackgroundTexture.Height),
                    Color.White);
            }
            batch.End();
        }
    }
}
