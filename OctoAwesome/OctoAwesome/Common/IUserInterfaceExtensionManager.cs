using engenious.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Common
{
    /// <summary>
    /// UserInterfaceManager
    /// </summary>
    public interface IUserInterfaceExtensionManager
    {
        /// <summary>
        /// Definitionmanager
        /// </summary>
        IDefinitionManager DefinitionManager { get; }
        /// <summary>
        /// Register an <see cref="IUserInterfaceExtension"/> to Gamescreen
        /// </summary>
        /// <param name="controltype">Type of the Control</param>
        /// <param name="args">contructor parameter, [reserved parameter] + args</param>
        /// <returns></returns>
        bool RegisterOnGameScreen(Type controltype, params object[] args);
        /// <summary>
        /// Register an <see cref="IUserInterfaceExtension"/> to InvertoryScreen
        /// </summary>
        /// <param name="controltype">Type of the Control</param>
        /// <param name="args">contructor parameter, [reserved parameter] + args</param>
        /// <returns></returns>
        bool RegisterOnInventoryScreen(Type controltype, params object[] args);
        /// <summary>
        /// Register an Debug label to the Game debug Control
        /// </summary>
        /// <param name="right"></param>
        /// <param name="updatefunc"></param>
        /// <returns></returns>
        bool RegisterOnDebugScreen(bool right, Func<string> updatefunc);
        /// <summary>
        /// Load Textures for UI
        /// </summary>
        /// <param name="type">Texture type</param>
        /// <param name="key">Texture key</param>
        /// <returns></returns>
        Texture2D LoadTextures(Type type, string key);
    }
}
