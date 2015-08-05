using Microsoft.Xna.Framework;

namespace OctoAwesome
{
    /// <summary>
    /// Zeigt an, dass ein Spielobjet (z.B. Chunk, Block) geupdatet werden soll
    /// </summary>
    public interface IUpdateable
    {
        void Update(GameTime gameTime);
    }
}
