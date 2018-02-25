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
        #region Temporary
        string Name { get; }
        string ModelName { get; }
        string TextureName { get; }
        float BaseRotationZ { get; }
        #endregion

        float Azimuth { get; }
        Vector3 Body { get; }
        Coordinate Position { get; }
        bool NeedUpdate { get; }
        void Initialize(IGraphicsDevice device);
        void Draw(IGraphicsDevice graphicsDevice, GameTime gameTime);
    }    
}
