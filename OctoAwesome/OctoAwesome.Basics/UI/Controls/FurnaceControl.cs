using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Client.UI.Components;
using OctoAwesome.Client.UI.Controls;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Basics.UI.Controls;

/// <summary>
/// The class for the ui furnace control
/// </summary>
public sealed class FurnaceControl : Panel
{
    private const int COLUMNS = 8;

    internal InventoryControl inputSlotPanel, outputSlotPanel, resourceSlotPanel;

    /// <summary>
    /// Gibt den aktuell selektierten Slot an.
    /// </summary>
    public InventorySlot HoveredSlot { get; private set; }

    private Grid grid;
    private readonly AssetComponent assets;

    /// <summary>
    /// Initializes a new instance of the engenious.UI.Controls.Panel class.
    /// </summary>
    public FurnaceControl(BaseScreenComponent manager, AssetComponent assets, IReadOnlyCollection<IInventorySlot> inventorySlots, IReadOnlyCollection<IInventorySlot> outputSlots, IReadOnlyCollection<IInventorySlot> ressourceSlots, int columns = COLUMNS) : base(manager)
    {
        Background = new SolidColorBrush(Color.Transparent);

        grid = new Grid(manager)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 5 });
        grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 2 });
        grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 5 });
        
        grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
        grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });

        grid.AddControl(new Label(manager) { Text = "\u2668",  TextColor = Color.Red, Font= ScreenManager.Content.Load<SpriteFont>("Fonts/Emoji")}, 1, 0);
        inputSlotPanel = new InventoryControl(manager, assets, inventorySlots);
        outputSlotPanel = new InventoryControl(manager, assets, outputSlots);
        resourceSlotPanel = new InventoryControl(manager, assets, ressourceSlots);
        grid.AddControl(inputSlotPanel, 0, 0);
        grid.AddControl(outputSlotPanel, 2, 0);
        grid.AddControl(resourceSlotPanel, 0,1);

        Controls.Add(grid);

        this.assets = assets;
        Rebuild(inventorySlots, outputSlots, ressourceSlots, columns);
    }

    /// <summary>
    /// Rebuild the grids for input, output and ressource inventory after a change is needed
    /// </summary>
    /// <param name="inventorySlots">The input inventory slots on the left</param>
    /// <param name="outputInventory">The output slots on the right</param>
    /// <param name="ressourceInventory">The ressource slots in the middle</param>
    /// <param name="columns">The amount of columns for inventory and output slot</param>
    public void Rebuild(IReadOnlyCollection<IInventorySlot> inventorySlots, IReadOnlyCollection<IInventorySlot> outputInventory, IReadOnlyCollection<IInventorySlot> ressourceInventory, int columns = COLUMNS)
    {
        inputSlotPanel.Rebuild(inventorySlots, columns / 2);
        outputSlotPanel.Rebuild(outputInventory, columns / 2);
        resourceSlotPanel.Rebuild(ressourceInventory, columns / 2);
        //TODO Draw in a grid formation

    }
}
