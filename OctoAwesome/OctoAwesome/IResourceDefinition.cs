using System;
using System.Drawing;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für die Definition einer Ressource
    /// </summary>
    public interface IResourceDefinition : IItemDefinition
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