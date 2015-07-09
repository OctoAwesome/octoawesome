using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class NineTileBrush : Brush
    {
        private Texture2D[] textures;
        private int cut;

        public NineTileBrush(Texture2D texture, int cut)
        {
            this.cut = cut;

            textures = new Texture2D[9];

            // Links oben
            uint[] buffer = new uint[cut * cut];
            texture.GetData<uint>(0, new Rectangle(0, 0, cut, cut), buffer, 0, cut * cut);
            textures[0] = new Texture2D(texture.GraphicsDevice, cut, cut);
            textures[0].SetData<uint>(buffer);

            // Rechts oben
            texture.GetData<uint>(0, new Rectangle(texture.Width - cut, 0, cut, cut), buffer, 0, cut * cut);
            textures[2] = new Texture2D(texture.GraphicsDevice, cut, cut);
            textures[2].SetData<uint>(buffer);

            // Links unten
            texture.GetData<uint>(0, new Rectangle(0, texture.Height - cut, cut, cut), buffer, 0, cut * cut);
            textures[6] = new Texture2D(texture.GraphicsDevice, cut, cut);
            textures[6].SetData<uint>(buffer);

            // Rechts unten
            texture.GetData<uint>(0, new Rectangle(texture.Width - cut, texture.Height - cut, cut, cut), buffer, 0, cut * cut);
            textures[8] = new Texture2D(texture.GraphicsDevice, cut, cut);
            textures[8].SetData<uint>(buffer);
        }

        public override void Draw(SpriteBatch batch, Rectangle rectangle)
        {
            batch.Begin();
            batch.Draw(textures[0], new Rectangle(rectangle.X, rectangle.Y, cut, cut), Color.White);
            batch.Draw(textures[2], new Rectangle(rectangle.X + rectangle.Size.X - cut, rectangle.Y, cut, cut), Color.White);
            batch.Draw(textures[6], new Rectangle(rectangle.X, rectangle.Y + rectangle.Size.Y - cut, cut, cut), Color.White);
            batch.Draw(textures[8], new Rectangle(rectangle.X + rectangle.Size.X - cut, rectangle.Y + rectangle.Size.Y - cut, cut, cut), Color.White);
            batch.End();
        }
    }
}
