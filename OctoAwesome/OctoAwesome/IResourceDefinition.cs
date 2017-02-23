using System;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für die Definition einer Ressource
    /// TODO: Das hier ist ja inzwischen etwas aus der Mode gekommen -> Siehe BlockDefinition
    /// </summary>
    public interface IResourceDefinition : IDefinition
    {
        /// <summary>
        /// Liefert eine Instanz der Ressource
        /// </summary>
        /// <returns>Eine Instanz der Ressource</returns>
        IResource GetInstance();

        /// <summary>
        /// Liefert den Typen der Ressource
        /// </summary>
        /// <returns>Den Typen</returns>
        Type GetResourceType();
    }
}
