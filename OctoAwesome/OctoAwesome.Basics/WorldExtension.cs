using OctoAwesome.Basics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Extensions
{
    public sealed class WorldExtension : IExtension
    {
        public string Name => Languages.OctoBasics.WorldExtensionName;

        public string Description => Languages.OctoBasics.WorldExtensionDescription;

        public void LoadDefinitions(IExtensionLoader extensionloader)
        {
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes().Where(
                t => !t.IsAbstract && typeof(IDefinition).IsAssignableFrom(t)))
            {
                extensionloader.RegisterDefinition((IDefinition) Activator.CreateInstance(t));
            }
            //extensionloader.LoadDefinitionsFromResource("OctoAwesome.Basics.Assets.entitydefinitions.xml");
        }
        public void Extend(IExtensionLoader extensionloader)
        {
            extensionloader.RegisterMapGenerator(new ComplexPlanetGenerator());
            extensionloader.RegisterMapPopulator(new TreePopulator());
            //extensionloader.RegisterMapPopulator(new WauziPopulator());
            extensionloader.RegisterEntity<WauziEntity>();
        }
    }
}
