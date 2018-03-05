using System.Linq;
using engenious;
using OctoAwesome.Basics.Controls;
using OctoAwesome.Entities;
using OctoAwesome.Common;

namespace OctoAwesome.Basics.EntityComponents
{
    /// <summary>
    /// EntityComponent, die eine Werkzeug-Toolbar für den Spieler bereitstellt.
    /// </summary>
    public class ToolBarComponent : EntityComponent, IUserInterfaceExtension
    {
        /// <summary>
        /// Gibt die Anzahl der Tools in der Toolbar an.
        /// </summary>
        public const int TOOLCOUNT = 10;
        /// <summary>
        /// Auflistung der Werkzeuge die der Spieler in seiner Toolbar hat.
        /// </summary>
        public InventorySlot[] Tools { get; }
        /// <summary>
        /// Derzeit aktives Werkzeug des Spielers
        /// </summary>
        public InventorySlot ActiveTool { get; set; }
        /// <summary>
        /// Erzeugte eine neue ToolBarComponent
        /// </summary>
        public ToolBarComponent() : base(true)
        {
            Tools = new InventorySlot[TOOLCOUNT];
        }
        /// <summary>
        /// Entfernt einen InventorySlot aus der Toolbar
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveSlot(InventorySlot slot)
        {
            for (int i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == slot)
                    Tools[i] = null;
            }
            if (ActiveTool == slot)
                ActiveTool = null;
        }
        /// <summary>
        /// Setzt einen InventorySlot an eine Stelle in der Toolbar und löscht ggf. vorher den Slot aus alten Positionen.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="index"></param>
        public void SetTool(InventorySlot slot, int index)
        {
            RemoveSlot(slot);

            Tools[index] = slot;
        }
        /// <summary>
        /// Gibt den Index eines InventorySlots in der Toolbar zurück.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns>Den Index des Slots, falls nicht gefunden -1.</returns>
        public int GetSlotIndex(InventorySlot slot)
        {
            for (int j = 0; j < Tools.Length; j++)
                if (Tools[j] == slot)
                    return j;

            return -1;
        }
        /// <summary>
        /// Fügt einen neuen InventorySlot an der ersten freien Stelle hinzu.
        /// </summary>
        /// <param name="slot"></param>
        public void AddNewSlot(InventorySlot slot)
        {
            for (int i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == null)
                {
                    Tools[i] = slot;
                    break;
                }
            }
        }
        public override void Update(GameTime gameTime, IGameService service)
        {
            IEntityController controller = (Entity as IControllable)?.Controller;
            if (controller == null) return;
            else if(controller.InteractInput)
            {
                if (Entity.Components.TryGetComponent(out InventoryComponent inventory))
                {
                    service.TakeBlock(controller, Entity.Cache, inventory);
                }
                else if (service.TakeBlock(controller, Entity.Cache, out IInventoryableDefinition item))
                {
                    // TODO: und jetzt ?
                }
            }
            else if(controller.ApplyInput)
            {
                if (Entity.Components.TryGetComponent(out InventoryComponent inventory))
                    service.InteractBlock(Entity.Position, 0, 0, controller, Entity.Cache, ActiveTool, inventory);
            }
        }
        public void Register(IUserInterfaceExtensionManager manager)
        {
            manager.RegisterOnGameScreen(typeof(ToolbarControl), manager, this);
            manager.RegisterOnInventoryScreen(typeof(ToolbarInventoryControl), manager, this);
        }
        private void UpdateToolbar(InventoryComponent inventory)
        {
            foreach (InventorySlot slot in inventory.Inventory)
            {
                if (Tools.Any(s => s != null && s.Definition.Name == slot.Definition.Name))
                    continue;

                AddNewSlot(slot);
            }
        }
    }
}
