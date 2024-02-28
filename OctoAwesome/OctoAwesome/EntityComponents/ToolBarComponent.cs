using OctoAwesome.Components;
using OctoAwesome.Definitions.Items;

using System;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for the toolbar of a player.
    /// </summary>
    public class ToolBarComponent : Component, IEntityComponent
    {
        /// <summary>
        /// The number of possible tools in the toolbar.
        /// </summary>
        public const int TOOLCOUNT = 10;

        /// <summary>
        /// Gets a list of inventory slots of the toolbar.
        /// </summary>
        public InventorySlot?[] Tools { get; }

        /// <summary>
        /// Gets the currently active tool.
        /// </summary>
        /// <remarks>Defaults to <c>null</c> if no tool is currently active.</remarks>
        public InventorySlot? ActiveTool => Tools[activeIndex];



        /// <summary>
        /// Gets or sets the currently active tool slot.
        /// </summary>
        public int ActiveIndex
        {
            get => activeIndex;
            set => activeIndex = (value + TOOLCOUNT) % TOOLCOUNT;
        }

        /// <summary>
        /// Called when a tool slot was changed.
        /// </summary>
        public event Action<InventorySlot?, int>? OnChanged;


        private int activeIndex;


        /// <summary>
        /// Initializes a new instance of the <see cref="ToolBarComponent"/> class.
        /// </summary>
        public ToolBarComponent()
        {
            Tools = new InventorySlot[TOOLCOUNT];
            ActiveIndex = 0;
        }

        /// <summary>
        /// Removes an inventory slot from the toolbar.
        /// </summary>
        /// <param name="slot">The slot to remove.</param>
        public void RemoveSlot(InventorySlot slot)
        {
            for (int i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == slot)
                {
                    Tools[i] = null;
                    OnChanged?.Invoke(null, i);
                    break;
                }
            }
        }

        /// <summary>
        /// Overrides the current slot at a specific toolbar slot index with a new slot value.
        /// </summary>
        /// <param name="slot">The new inventory slot to set the toolbar slot to.</param>
        /// <param name="index">The index of the slot to set.</param>
        public void SetTool(InventorySlot slot, int index)
        {
            RemoveSlot(slot);

            Tools[index] = slot;
            OnChanged?.Invoke(slot, index);
        }

        /// <summary>
        /// Gets the index for a specific inventory slot in the toolbar.
        /// </summary>
        /// <param name="slot">The slot to get the index to.</param>
        /// <returns>The index of the slot if it was found; otherwise -1.</returns>
        public int GetSlotIndex(InventorySlot? slot)
        {
            for (int j = 0; j < Tools.Length; j++)
                if (Tools[j] == slot)
                    return j;

            return -1;
        }

        /// <summary>
        /// Adds a new slot at the first empty toolbar slot.
        /// </summary>
        /// <param name="slot">The inventory slot to add.</param>
        public void AddNewSlot(InventorySlot slot)
        {
            for (int i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == null)
                {
                    Tools[i] = slot;
                    OnChanged?.Invoke(slot, i);
                    break;
                }
            }
        }
    }
}
