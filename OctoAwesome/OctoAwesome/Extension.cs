using engenious;
using OctoAwesome.EntityComponents;

namespace OctoAwesome
{
    //TODO: Perhaps outsource

    /// <summary>
    /// The base extension implementation.
    /// </summary>
    public sealed class Extension : IExtension
    {
        /// <inheritdoc />
        public string Description => "OctoAwesome";

        /// <inheritdoc />
        public string Name => "OctoAwesome";

        /// <inheritdoc />
        public void Register(IExtensionLoader extensionLoader, ITypeContainer typeContainer)
        {
            extensionLoader.RegisterEntityExtender<Player>((player) =>
            {
                var p = (Player)player;
                p.Components.AddComponent(new ControllableComponent());
                p.Components.AddComponent(new HeadComponent() { Offset = new Vector3(0, 0, 3.2f) });
                p.Components.AddComponent(new InventoryComponent());
                p.Components.AddComponent(new ToolBarComponent());
            });
        }

        /// <inheritdoc />
        public void Register(ITypeContainer typeContainer) { }
    }
}
