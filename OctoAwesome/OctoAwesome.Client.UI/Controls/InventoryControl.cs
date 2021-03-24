using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Definitions;
using OctoAwesome.UI.Components;
using System.Collections.Generic;

namespace OctoAwesome.UI.Controls
{
    public sealed class InventoryControl : Panel
    {
        private const int COLUMNS = 8;

        /// <summary>
        /// Gibt den aktuell selektierten Slot an.
        /// </summary>
        public InventorySlot HoveredSlot { get; private set; }

        public InventoryControl(BaseScreenComponent manager, AssetComponent assets, List<InventorySlot> inventorySlots, int columns = COLUMNS) : base(manager)
        {           
            ScrollContainer scroll = new ScrollContainer(manager)
            {
                Margin = new Border(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Controls.Add(scroll);

            Grid grid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            for (int i = 0; i < columns; i++)
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            int rows = (int)System.Math.Ceiling((float)inventorySlots.Count / columns);
            for (int i = 0; i < rows; i++)
                grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            int column = 0;
            int row = 0;
            foreach (var inventorySlot in inventorySlots)
            {
                Texture2D texture;
                if (inventorySlot.Definition is null)
                    continue;
                else 
                    texture = assets.LoadTexture(inventorySlot.Definition.GetType(), inventorySlot.Definition.Icon);
                    

                var image = new Image(manager) { Texture = texture, Width = 42, Height = 42, VerticalAlignment = VerticalAlignment.Center };
                image.MouseEnter += (s, e) => { HoveredSlot = inventorySlot; };
                image.MouseLeave += (s, e) => { HoveredSlot = null; };
                image.StartDrag += (c,e) =>
                {
                    e.Handled = true;
                    e.Icon = texture;
                    e.Content = inventorySlot;
                    e.Sender = image;
                };
                var label = new Label(manager) { Text = inventorySlot.Amount.ToString(), HorizontalAlignment = HorizontalAlignment.Right, VerticalTextAlignment = VerticalAlignment.Bottom, Background = new BorderBrush(Color.White) };
                grid.AddControl(image, column, row);
                grid.AddControl(label, column, row);

                column++;
                if (column >= columns)
                {
                    row++;
                    column = 0;
                }
            }

            scroll.Content = grid;


        }
    }
}
