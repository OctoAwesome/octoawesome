using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Statische Klasse die das Laden von Bilder erleichtert
    /// TODO: In den Client umziehen?
    /// </summary>
    public static class ContentHelper
    {
        /// <summary>
        /// Lädt ein Bild am angegebenen Pfad und konvertiert es in eine Texture2D
        /// </summary>
        /// <param name="man">[Bitte ergänzen]</param>
        /// <param name="path">Der Pfad des Bilds das geladen werden soll</param>
        /// <param name="device">Das Graphicsdevice</param>
        /// <returns>Das Bild am angegebenen Pfad als Texture2D</returns>
        public static Texture2D LoadTexture2DFromFile(this ContentManager man, string path, GraphicsDevice device)
        {
            using (Stream stream = File.OpenRead(path))
            {
                return Texture2D.FromStream(device, stream);
            }
        }
    }
}
