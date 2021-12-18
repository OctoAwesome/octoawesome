using OctoAwesome.Definitions;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Interface for defining a specific tree type, used by the <see cref="TreePopulator"/>.
    /// </summary>
    public interface ITreeDefinition : IDefinition
    {
        /// <summary>
        /// Gets a value indicating the priority order in which the trees should be tried to be planted.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Gets the maximum temperature the tree type can grow in.
        /// </summary>
        float MaxTemperature { get; }

        /// <summary>
        /// Gets the minimal temperature the tree type can grow in.
        /// </summary>
        float MinTemperature { get; }

        /// <summary>
        /// Initializes the <see cref="ITreeDefinition"/>.
        /// </summary>
        /// <param name="definitionManager">The used <see cref="IDefinitionManager"/>.</param>
        void Init(IDefinitionManager definitionManager);

        /// <summary>
        /// Gets the number of trees that should be tried to be planted in a specific chunk.
        /// </summary>
        /// <param name="planet">The planet to get the tree density from.</param>
        /// <param name="index">The chunk position to get the tree density at.</param>
        /// <returns></returns>
        int GetDensity(IPlanet planet, Index3 index);

        /// <summary>
        /// Plants a tree at a given position.
        /// </summary>
        /// <param name="planet">The planet to plant the tree on.</param>
        /// <param name="index">The position of the tree. X, Y in local chunk coordinates, Z in absolute coordinates.</param>
        /// <param name="builder">The <see cref="LocalBuilder"/> for building the tree.</param>
        /// <param name="seed">Seeding value for generating unique trees.</param>
        void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed);
    }
}
