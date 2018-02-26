using System;
using engenious.Graphics;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// UserInterfaceManager
    /// </summary>
    public interface IUserInterfaceManager
    {
        bool RegisterOnGameScreen(Type controltype, params object[] args);
        bool RegisterOnInventoryScreen(Type controltype, params object[] args);
        Texture2D LoadTextures(Type type, string key);
    }
    /// <summary>
    /// Interface for UI dependend <see cref="EntityComponent"/>
    /// </summary>
    public interface IUserInterfaceExtension
    {
        /// <summary>
        /// Register the extending UI.
        /// </summary>
        /// <param name="manager">UI-Manager</param>
        /// <param name="textures">UI-Textures</param>
        void Register(IUserInterfaceManager manager);
    }
}
