using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Client.UI.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System.Collections.Generic;

namespace OctoAwesome.Basics.UI.Controls;

public sealed class FurnaceControl : Panel
{
    private const int COLUMNS = 8;

    private Panel inputSlotPanel, outputSlotPanel;

    /// <summary>
    /// Gibt den aktuell selektierten Slot an.
    /// </summary>
    public InventorySlot HoveredSlot { get; private set; }

    private Grid grid;
    private readonly AssetComponent assets;

    public FurnaceControl(BaseScreenComponent manager, AssetComponent assets, IReadOnlyCollection<IInventorySlot> inventorySlots, IReadOnlyCollection<IInventorySlot> outputSlots, int columns = COLUMNS) : base(manager)
    {
        Background = new SolidColorBrush(Color.Transparent);

        grid = new Grid(manager)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
        grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
        grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });

        grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
        grid.AddControl(new Label(manager) { Text = "==>" }, 1, 0);
        inputSlotPanel = new Panel(manager);
        outputSlotPanel = new Panel(manager);
        grid.AddControl(inputSlotPanel, 0, 0);
        grid.AddControl(outputSlotPanel, 2, 0);

        Controls.Add(grid);

        this.assets = assets;
        Rebuild(inventorySlots, outputSlots, columns);
    }

    public void Rebuild(IReadOnlyCollection<IInventorySlot> inventorySlots, IReadOnlyCollection<IInventorySlot> outputInventory, int columns = COLUMNS)
    {
        //TODO Draw in a grid formation

        var inputPanel = inputSlotPanel;
        inputPanel.Controls.Clear();
        foreach (var inventorySlot in inventorySlots)
        {
            Texture2D texture;
            if (inventorySlot.Definition is null)
                continue;
            else
                texture = assets.LoadTexture(inventorySlot.Definition.GetType(), inventorySlot.Definition.Icon);


            var image = new Image(ScreenManager) { Texture = texture, Width = 42, Height = 42, VerticalAlignment = VerticalAlignment.Center };
            image.MouseEnter += (s, e) => { HoveredSlot = (InventorySlot)inventorySlot; };
            image.MouseLeave += (s, e) => { HoveredSlot = null; };
            image.StartDrag += (c, e) =>
            {
                e.Handled = true;
                e.Icon = texture;
                e.Content = inventorySlot;
                e.Sender = image;
            };
            var label = new Label(ScreenManager) { Text = inventorySlot.Amount.ToString(), HorizontalAlignment = HorizontalAlignment.Right, VerticalTextAlignment = VerticalAlignment.Bottom, Background = new BorderBrush(Color.White) };
            
            var grid = new Grid(ScreenManager);
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });
            grid.AddControl(image, 0, 0);
            grid.AddControl(label, 0, 0);

            inputPanel.Controls.Add(grid);
        }
        var outputPanel = outputSlotPanel;
        outputPanel.Controls.Clear();
        foreach (var inventorySlot in outputInventory)
        {
            Texture2D texture;
            if (inventorySlot.Definition is null)
                continue;
            else
                texture = assets.LoadTexture(inventorySlot.Definition.GetType(), inventorySlot.Definition.Icon);


            var image = new Image(ScreenManager) { Texture = texture, Width = 42, Height = 42, VerticalAlignment = VerticalAlignment.Center };
            image.MouseEnter += (s, e) => { HoveredSlot = (InventorySlot)inventorySlot; };
            image.MouseLeave += (s, e) => { HoveredSlot = null; };
            image.StartDrag += (c, e) =>
            {
                e.Handled = true;
                e.Icon = texture;
                e.Content = inventorySlot;
                e.Sender = image;
            };
            var label = new Label(ScreenManager) { Text = inventorySlot.Amount.ToString(), HorizontalAlignment = HorizontalAlignment.Right, VerticalTextAlignment = VerticalAlignment.Bottom, Background = new BorderBrush(Color.White) };
            
            var grid = new Grid(ScreenManager);
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });
            grid.AddControl(image, 0, 0);
            grid.AddControl(label, 0, 0);

            outputPanel.Controls.Add(grid);
        }
    }
}
