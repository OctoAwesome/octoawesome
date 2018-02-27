using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using engenious.Graphics;
using engenious;
namespace OctoAwesome.Entities
{
    /// <summary>
    /// Interface for drawable elements
    /// </summary>
    public interface IDrawable
    {
        #region Temporary
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Modelname
        /// </summary>
        string ModelName { get; }
        /// <summary>
        /// Texturename
        /// </summary>
        string TextureName { get; }
        /// <summary>
        /// Base rotation of Z axis
        /// </summary>
        float BaseRotationZ { get; }
        #endregion

        /// <summary>
        /// Indicates if the element need draw updates
        /// </summary>
        bool DrawUpdate { get; }
        /// <summary>
        /// Horizontal angle
        /// </summary>
        float Azimuth { get; }
        /// <summary>
        /// Height of the element
        /// </summary>
        float Height { get; }
        /// <summary>
        /// body radius of the element
        /// </summary>
        float Radius { get; }
        /// <summary>
        /// position of the element
        /// </summary>
        Coordinate Position { get; }
        //void Initialize(IGraphicsDevice device);
        //void Draw(IGraphicsDevice graphicsDevice, GameTime gameTime);
    }    
}
