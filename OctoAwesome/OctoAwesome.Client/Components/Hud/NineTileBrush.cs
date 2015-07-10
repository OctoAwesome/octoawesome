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
        public Texture2D CenterTexture { get; set; }

        public Texture2D LeftTexture { get; set; }

        public Texture2D RightTexture { get; set; }

        public Texture2D TopTexture { get; set; }

        public Texture2D BottomTexture { get; set; }

        public Texture2D UpperLeftTexture { get; set; }

        public Texture2D UpperRightTexture { get; set; }

        public Texture2D LowerLeftTexture { get; set; }

        public Texture2D LowerRightTexture { get; set; }

        public override void Draw(SpriteBatch batch, Rectangle rectangle)
        {
            batch.Begin(samplerState: SamplerState.LinearWrap);

            // Center
            batch.Draw(CenterTexture,
                new Rectangle(rectangle.X + UpperLeftTexture.Width, rectangle.Y + UpperLeftTexture.Height, rectangle.Width - UpperLeftTexture.Width - LowerRightTexture.Width, rectangle.Height - UpperLeftTexture.Height - LowerRightTexture.Height),
                new Rectangle(0, 0, rectangle.Width - UpperLeftTexture.Width - LowerRightTexture.Width, rectangle.Height - UpperLeftTexture.Height - LowerRightTexture.Height), Color.White);

            // Borders
            batch.Draw(TopTexture,
                new Rectangle(rectangle.X + UpperLeftTexture.Width, rectangle.Y, rectangle.Width - UpperLeftTexture.Width - UpperRightTexture.Width, TopTexture.Height),
                new Rectangle(0, 0, rectangle.Width - UpperLeftTexture.Width - UpperRightTexture.Width, TopTexture.Height), Color.White);

            batch.Draw(BottomTexture,
                new Rectangle(rectangle.X + UpperLeftTexture.Width, rectangle.Y + rectangle.Height - BottomTexture.Height, rectangle.Width - LowerLeftTexture.Width - LowerRightTexture.Width, BottomTexture.Height),
                new Rectangle(0, 0, rectangle.Width - LowerLeftTexture.Width - LowerRightTexture.Width, BottomTexture.Height), Color.White);

            batch.Draw(LeftTexture,
                new Rectangle(rectangle.X, rectangle.Y + UpperLeftTexture.Height, LeftTexture.Width, rectangle.Height - UpperLeftTexture.Height - LowerLeftTexture.Height),
                new Rectangle(0, 0, LeftTexture.Width, rectangle.Height - UpperLeftTexture.Height - LowerLeftTexture.Height), Color.White);

            batch.Draw(RightTexture,
                new Rectangle(rectangle.X + rectangle.Width - RightTexture.Width, rectangle.Y + UpperRightTexture.Height, RightTexture.Width, rectangle.Height - UpperRightTexture.Height - LowerRightTexture.Height),
                new Rectangle(0, 0, RightTexture.Width, rectangle.Height - UpperRightTexture.Height - LowerRightTexture.Height), Color.White);

            // Corners
            batch.Draw(UpperLeftTexture, new Rectangle(rectangle.X, rectangle.Y, UpperLeftTexture.Width, UpperLeftTexture.Height), Color.White);
            batch.Draw(UpperRightTexture, new Rectangle(rectangle.X + rectangle.Size.X - UpperRightTexture.Width, rectangle.Y, UpperRightTexture.Width, UpperRightTexture.Height), Color.White);
            batch.Draw(LowerLeftTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Size.Y - LowerLeftTexture.Height, LowerLeftTexture.Width, LowerLeftTexture.Height), Color.White);
            batch.Draw(LowerRightTexture, new Rectangle(rectangle.X + rectangle.Size.X - LowerRightTexture.Width, rectangle.Y + rectangle.Size.Y - LowerRightTexture.Height, LowerRightTexture.Width, LowerRightTexture.Height), Color.White);

            batch.End();
        }

        public static NineTileBrush FromFullTexture(Texture2D texture, int cutX, int cutY)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");

            if (cutX <= 0)
                throw new ArgumentException("cutX is too small");

            if (cutY <= 0)
                throw new ArgumentException("cutY is too small");

            if (2 * cutX > texture.Width)
                throw new ArgumentException("cutX is too large.");

            if (2 * cutY > texture.Width)
                throw new ArgumentException("cutY is too large.");

            NineTileBrush brush = new NineTileBrush();

            #region Ecken

            // Eck-Buffer
            uint[] buffer1 = new uint[cutX * cutY];

            // Upper Left
            brush.UpperLeftTexture = new Texture2D(texture.GraphicsDevice, cutX, cutY);
            texture.GetData<uint>(0, new Rectangle(0, 0, cutX, cutY), buffer1, 0, cutX * cutY);
            brush.UpperLeftTexture.SetData<uint>(buffer1);

            // Upper Right
            brush.UpperRightTexture = new Texture2D(texture.GraphicsDevice, cutX, cutY);
            texture.GetData<uint>(0, new Rectangle(texture.Width - cutX, 0, cutX, cutY), buffer1, 0, cutX * cutY);
            brush.UpperRightTexture.SetData<uint>(buffer1);

            // Lower Left
            brush.LowerLeftTexture = new Texture2D(texture.GraphicsDevice, cutX, cutY);
            texture.GetData<uint>(0, new Rectangle(0, texture.Height - cutY, cutX, cutY), buffer1, 0, cutX * cutY);
            brush.LowerLeftTexture.SetData<uint>(buffer1);

            // Lower Right
            brush.LowerRightTexture = new Texture2D(texture.GraphicsDevice, cutX, cutY);
            texture.GetData<uint>(0, new Rectangle(texture.Width - cutX, texture.Height - cutY, cutX, cutY), buffer1, 0, cutX * cutY);
            brush.LowerRightTexture.SetData<uint>(buffer1);

            #endregion

            #region Horizontale Kanten

            int buffer2SizeX = (texture.Width - cutX - cutX);
            int buffer2SizeY = cutY;
            uint[] buffer2 = new uint[buffer2SizeX * buffer2SizeY];

            // Upper Border
            brush.TopTexture = new Texture2D(texture.GraphicsDevice, buffer2SizeX, buffer2SizeY);
            texture.GetData<uint>(0, new Rectangle(cutX, 0, buffer2SizeX, buffer2SizeY), buffer2, 0, buffer2SizeX * buffer2SizeY);
            brush.TopTexture.SetData<uint>(buffer2);

            // Lower Border
            brush.BottomTexture = new Texture2D(texture.GraphicsDevice, buffer2SizeX, buffer2SizeY);
            texture.GetData<uint>(0, new Rectangle(cutX, texture.Height - cutY, buffer2SizeX, buffer2SizeY), buffer2, 0, buffer2SizeX * buffer2SizeY);
            brush.BottomTexture.SetData<uint>(buffer2);

            #endregion

            #region Vertikale Kanten

            int buffer3SizeX = cutX;
            int buffer3SizeY = (texture.Height - cutY - cutY);
            uint[] buffer3 = new uint[buffer3SizeX * buffer3SizeY];

            // Left Border
            brush.LeftTexture = new Texture2D(texture.GraphicsDevice, buffer3SizeX, buffer3SizeY);
            texture.GetData<uint>(0, new Rectangle(0, cutY, buffer3SizeX, buffer3SizeY), buffer3, 0, buffer3SizeX * buffer3SizeY);
            brush.LeftTexture.SetData<uint>(buffer3);

            // Right Border
            brush.RightTexture = new Texture2D(texture.GraphicsDevice, buffer3SizeX, buffer3SizeY);
            texture.GetData<uint>(0, new Rectangle(texture.Width - cutX, cutY, buffer3SizeX, buffer3SizeY), buffer3, 0, buffer3SizeX * buffer3SizeY);
            brush.RightTexture.SetData<uint>(buffer3);

            #endregion

            #region Zentrum

            int buffer4SizeX = (texture.Width - cutX - cutX);
            int buffer4SizeY = (texture.Height - cutY - cutY);
            uint[] buffer4 = new uint[buffer4SizeX * buffer4SizeY];

            // Left Border
            brush.CenterTexture = new Texture2D(texture.GraphicsDevice, buffer4SizeX, buffer4SizeY);
            texture.GetData<uint>(0, new Rectangle(cutX, cutY, buffer4SizeX, buffer4SizeY), buffer4, 0, buffer4SizeX * buffer4SizeY);
            brush.CenterTexture.SetData<uint>(buffer4);

            #endregion

            return brush;
        }
    }
}
