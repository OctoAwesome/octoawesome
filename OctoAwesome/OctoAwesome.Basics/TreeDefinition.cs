using OctoAwesome.Definitions;
using OctoAwesome.Location;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Basisklasse für Baumdefinitionen, die vom TreePopulator verarbeitet werden sollen.
    /// </summary>
    public abstract class TreeDefinition : ITreeDefinition
    {
        /// <summary>
        /// Returns the not existing Name of the Tree Definition.
        /// </summary>
        public string Name => "";

        /// <summary>
        /// Returns the not existing Resource Name of the Definition Icon.
        /// </summary>
        public string Icon => "";

        /// <summary>
        /// Gibt die Reihenfolge dieser TreeDefinition in der Abarbeitung beim bepflanzen der Chunks an.
        /// </summary>
        public abstract int Order { get; }

        /// <summary>
        /// Maximaltemperatur für diese Art von Bäumen.
        /// </summary>
        public abstract float MaxTemperature { get; }

        /// <summary>
        /// Minimaltemperatur für diese Art von Bäumen.
        /// </summary>
        public abstract float MinTemperature { get; }

        /// <summary>
        /// Gibt die Anzahl der Bäume zurück, die in einem Chunk gepflanzt werden sollen.
        /// </summary>
        /// <param name="planet">Der aktuelle Planet</param>
        /// <param name="index">Der Index des Chunks in absoluten Blockkoordinaten.</param>
        /// <returns></returns>
        public abstract int GetDensity(IPlanet planet, Index3 index);

        /// <summary>
        /// Initialisiert die Treedefinition.
        /// </summary>
        /// <param name="definitionManager">Der verwendete <see cref="IDefinitionManager"/>.</param>
        public abstract void Init(IDefinitionManager definitionManager);

        /// <summary>
        /// Pflanzt einen Baum.
        /// </summary>
        /// <param name="planet">Der aktuelle Planet.</param>
        /// <param name="index">Die Position des Baums. X, Y in lokalen Chunk-Koordinaten, Z in absoluten Koordinaten.</param>
        /// <param name="builder">Der <see cref="LocalBuilder"/> zum Setzen des Baums.</param>
        /// <param name="seed">Seed für das zufälligere Pflanzen.</param>
        public abstract void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed);
    }
}
