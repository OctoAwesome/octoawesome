using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using engenious.Graphics;
using engenious;
namespace OctoAwesome.Entities
{
    public interface IDrawable
    {
        /// <summary>
        /// TODO: remove
        /// </summary>
        #region Temporary
        string Name { get; }
        string ModelName { get; }
        string TextureName { get; }
        float BaseRotationZ { get; }
        #endregion

        bool DrawUpdate { get; }
        float Azimuth { get; }
        float Height { get; }
        float Radius { get; }
        Coordinate Position { get; }
        void Initialize(IGraphicsDevice device);
        void Draw(IGraphicsDevice graphicsDevice, GameTime gameTime);
    }    
}
