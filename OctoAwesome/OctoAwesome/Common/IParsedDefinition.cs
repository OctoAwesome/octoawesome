using System;
namespace OctoAwesome.Common
{
    /// <summary>
    /// Interface for parsed definitions
    /// </summary>
    public interface IParsedDefinition
    {
        /// <summary>
        /// Associated type with this definition
        /// </summary>
        Type AssociatedType { get; }
        /// <summary>
        /// Indicates that the definition is sealed
        /// </summary>
        bool IsSealed { get; }
        /// <summary>
        /// Set a property
        /// </summary>
        /// <param name="propertyname">Name of the property</param>
        /// <param name="value">Value of the property to parse</param>
        void Set(string propertyname, string value);
        /// <summary>
        /// Sealing the definition
        /// </summary>
        void Seal();
    }
}
