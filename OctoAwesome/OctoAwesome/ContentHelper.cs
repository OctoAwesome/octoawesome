using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OctoAwesome
{
    public static class ContentHelper
    {
        public static Texture2D LoadTexture2DFromFile(this ContentManager man, string path, GraphicsDevice device)
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile(path);

            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Bmp);
                stream.Seek(0, SeekOrigin.Begin);
                return Texture2D.FromStream(device, stream);
            }
        }
    }
}
