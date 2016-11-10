using System.Collections.Generic;

namespace OctoAwesome
{
    public interface IExtensionResolver
    {
        void ExtendSimulation(Simulation simulation);

        void ExtendEntity(Entity entity);

        IEnumerable<T> GetDefinitions<T>() where T : IDefinition;
    }
}
