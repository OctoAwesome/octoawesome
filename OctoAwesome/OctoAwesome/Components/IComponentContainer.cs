using System;

namespace OctoAwesome.Components
{
    /// <summary>
    /// Interface for component containers.
    /// </summary>
    public interface IComponentContainer
    {
        /// <summary>
        /// Gets the Id of the container.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the reference to the active simulation; or <c>null</c> when no simulation is active.
        /// </summary>
        Simulation? Simulation { get; }

        /// <summary>
        /// Gets a value indicating whether a component of the given type exists in this container.
        /// </summary>
        /// <typeparam name="T">The type of the component to check for.</typeparam>
        /// <returns>A value indicating whether a component of the given type exists in this container.</returns>
        bool ContainsComponent<T>();

        /// <summary>
        /// Gets a component of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the component to get.</typeparam>
        /// <returns>The matching component; or <c>null</c> when no matching component was found.</returns>
        T? GetComponent<T>();
        /// <summary>
        /// Gets a component of the specified type with a known id.
        /// </summary>
        /// <param name="id">The unique identifier for the component</param>
        /// <typeparam name="T">The type of the component to get.</typeparam>
        /// <returns>The matching component; or <c>null</c> when no matching component was found.</returns>
        T? GetComponent<T>(int id);
    }
}
