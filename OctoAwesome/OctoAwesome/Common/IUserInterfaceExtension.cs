using OctoAwesome.Entities;

namespace OctoAwesome.Common
{
    /// <summary>
    /// Interface for UI dependend <see cref="EntityComponent"/>
    /// </summary>
    public interface IUserInterfaceExtension
    {
        /// <summary>
        /// Register the extending UI.
        /// </summary>
        /// <param name="manager">UI-Manager</param>
        void Register(IUserInterfaceExtensionManager manager);
    }
}
