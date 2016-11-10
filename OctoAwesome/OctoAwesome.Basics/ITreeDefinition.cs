using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Basisschnittstelle für Baumdefinitionen, die vom TreePopulator verarbeitet werden sollen.
    /// </summary>
    public interface ITreeDefinition : IDefinition
    {
        /// <summary>
        /// Gibt die Reihenfolge dieser ITreeDefinition in der Abarbeitung beim bepflanzen der Chunks an.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Maximaltemperatur für diese Art von Bäumen.
        /// </summary>
        float MaxTemperature { get; }

        /// <summary>
        /// Minimaltemperatur für diese Art von Bäumen.
        /// </summary>
        float MinTemperature { get; }

        /// <summary>
        /// Initialisiert die Treedefinition.
        /// </summary>
        /// <param name="definitionManager">Der verwendete <see cref="IDefinitionManager"/>.</param>
        void Init(IDefinitionManager definitionManager);

        /// <summary>
        /// Gibt die Anzahl der Bäume zurück, die in einem Chunk gepflanzt werden sollen.
        /// </summary>
        /// <param name="planet">Der aktuelle Planet</param>
        /// <param name="index">Der Index des Chunks in absoluten Blockkoordinaten.</param>
        /// <returns></returns>
        int GetDensity(IPlanet planet, Index3 index);

        /// <summary>
        /// Pflanzt einen Baum.
        /// </summary>
        /// <param name="definitionManager">Der verwendete <see cref="IDefinitionManager"/>.</param>
        /// <param name="planet">Der aktuelle Planet.</param>
        /// <param name="index">Die Position des Baums. X, Y in lokalen Chunk-Koordinaten, Z in absoluten Koordinaten.</param>
        /// <param name="builder">Der <see cref="LocalBuilder"/> zum Setzen des Baums.</param>
        /// <param name="seed">Seed für das zufälligere Pflanzen.</param>
        void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed);
    }
}
