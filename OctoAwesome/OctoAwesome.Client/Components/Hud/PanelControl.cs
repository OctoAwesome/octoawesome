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
        public Brush Background { get; set; }

        public PanelControl(IScreenManager screenManager)
            : base(screenManager)
        {
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            if (Background != null)
                Background.Draw(batch, new Rectangle(Position.X, Position.Y, Size.X, Size.Y));

            //if (BackgroundTexture == null)
            //    return;

            //if (dirty)
            //{
            //    dirty = false;
            //}

            //batch.Begin(samplerState: SamplerState.LinearWrap);

            //int cut = 33;

            //batch.Draw(BackgroundTexture,
            //        new Rectangle(Position.X, Position.Y, Size.X, Size.Y),
            //        new Rectangle(cut, cut, cut, cut),
            //        Color.White);

            //batch.Draw(BackgroundTexture,
            //        new Rectangle(Position.X, Position.Y, Size.X, cut),
            //        new Rectangle(cut, 0, cut, cut),
            //        Color.White);

            //batch.Draw(BackgroundTexture,
            //    new Rectangle(Position.X, Position.Y + Size.Y - cut, Size.X, cut),
            //    new Rectangle(cut, cut + cut, cut, cut),
            //    Color.White);

            //batch.Draw(BackgroundTexture,
            //    new Rectangle(Position.X, Position.Y, cut, Size.Y),
            //    new Rectangle(0, cut, cut, cut),
            //    Color.White);

            //batch.Draw(BackgroundTexture,
            //    new Rectangle(Position.X + Size.X - cut, Position.Y, cut, Size.Y),
            //    new Rectangle(cut + cut, cut, cut, cut),
            //    Color.White);

            //batch.Draw(BackgroundTexture,
            //    new Rectangle(Position.X, Position.Y, cut, cut),
            //    new Rectangle(0, 0, cut, cut),
            //    Color.White);

            //batch.Draw(BackgroundTexture,
            //    new Rectangle(Position.X + Size.X - cut, Position.Y, cut, cut),
            //    new Rectangle(cut + cut, 0, cut, cut),
            //    Color.White);

            //batch.Draw(BackgroundTexture,
            //    new Rectangle(Position.X, Position.Y + Size.Y - cut, cut, cut),
            //    new Rectangle(0, cut + cut, cut, cut),
            //    Color.White);

            //batch.Draw(BackgroundTexture,
            //    new Rectangle(Position.X + Size.X - cut, Position.Y + Size.Y - cut, cut, cut),
            //    new Rectangle(cut + cut, cut + cut, cut, cut),
            //    Color.White);

            //if (Tiles)
            //{
            //    batch.Begin(samplerState: SamplerState.LinearWrap);
            //    batch.Draw(BackgroundTexture,
            //        new Rectangle(Position.X, Position.Y, Size.X, Size.Y),
            //        new Rectangle(Position.X, Position.Y, Size.X, Size.Y),
            //        Color.White);
            //}
            //else
            //{
            //    batch.Begin();
            //    batch.Draw(BackgroundTexture,
            //        new Rectangle(Position.X, Position.Y, Size.X, Size.Y),
            //        new Rectangle(0, 0, BackgroundTexture.Width, BackgroundTexture.Height),
            //        Color.White);
            //}
            // batch.End();
        }
    }
}
