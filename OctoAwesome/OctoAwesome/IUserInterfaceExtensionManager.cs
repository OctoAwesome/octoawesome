using engenious.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// UserInterfaceManager
    /// </summary>
    public interface IUserInterfaceExtensionManager
    {
        /// <summary>
        /// Register an <see cref="IUserInterfaceExtension"/> to Gamescreen
        /// </summary>
        /// <param name="controltype">Type of the Control</param>
        /// <param name="args">contructor parameter</param>
        /// <returns></returns>
        bool RegisterOnGameScreen(Type controltype, params object[] args);
        /// <summary>
        /// Register an <see cref="IUserInterfaceExtension"/> to InvertoryScreen
        /// </summary>
        /// <param name="controltype">Type of the Control</param>
        /// <param name="args">contructor parameter</param>
        /// <returns></returns>
        bool RegisterOnInventoryScreen(Type controltype, params object[] args);
        /// <summary>
        /// Load Textures for UI
        /// </summary>
        /// <param name="type">Texture type</param>
        /// <param name="key">Texture key</param>
        /// <returns></returns>
        Texture2D LoadTextures(Type type, string key);
    }
}
