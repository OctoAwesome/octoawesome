using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Client.UI.Components;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OctoAwesome.Client.UI.Controls
{
    /// <summary>
    /// Control for displaying inventories.
    /// </summary>
    public sealed class InventoryControl : Panel
    {
        private const int COLUMNS = 8;

        /// <summary>
        /// Gets the slot that is currently hovered over by the cursor.
        /// </summary>
        public IInventorySlot HoveredSlot { get; private set; }

        private Grid grid;
        private readonly ScrollContainer scroll;
        private readonly AssetComponent assets;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryControl"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="engenious.UI.BaseScreenComponent" />.</param>
        /// <param name="assets">The asset component used to load resource assets.</param>
        /// <param name="inventorySlots">The inventory slots of the inventory to show.</param>
        /// <param name="columns">The number of columns for the inventory.</param>
        public InventoryControl(BaseScreenComponent manager, AssetComponent assets, IReadOnlyCollection<IInventorySlot> inventorySlots, int columns = COLUMNS) : base(manager)
        {
            Background = new SolidColorBrush(Color.Transparent);
            scroll = new ScrollContainer(manager)
            {
                Background = new SolidColorBrush(Color.Transparent),
                Margin = new Border(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalScrollbarVisibility = ScrollbarVisibility.Never,
                VerticalScrollbarEnabled = true

            };
            scroll.VerticalScrollbar.Background = new SolidColorBrush(Color.Transparent);

            grid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            this.assets = assets;

            scroll.Content = grid;

            Controls.Add(scroll);
            Rebuild(inventorySlots, columns);
        }

        /// <summary>
        /// Rebuild the controls for showing the inventory.
        /// </summary>
        /// <param name="inventorySlots">The inventory slots to create controls for.</param>
        /// <param name="columns">The number of columns to split the inventory into.</param>
        public void Rebuild(IReadOnlyCollection<IInventorySlot> inventorySlots, int columns = COLUMNS)
        {
            grid = new Grid(ScreenManager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            scroll.Content = grid;

            for (int i = 0; i < columns; i++)
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });

            int rows = (int)System.Math.Ceiling((float)inventorySlots.Count / columns);
            for (int i = 0; i < rows; i++)
                grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            int column = 0;
            int row = 0;

            foreach (var inventorySlot in inventorySlots.OrderBy(x => x.Item is null).ThenBy(x => x.Item?.GetType().Name))
            {
                Texture2D texture;

                var panel = new Panel(ScreenManager) {Width = 44, Height = 44 };
                panel.Background = new BorderBrush(LineType.Solid, Color.Black);
                var label = new Label(ScreenManager) { Text = "", HorizontalAlignment = HorizontalAlignment.Right, VerticalTextAlignment = VerticalAlignment.Bottom, Background = new BorderBrush(Color.Transparent) };
                if (inventorySlot.Definition is not null)
                {

                    texture = assets.LoadTexture(inventorySlot.Definition.GetType(), inventorySlot.Definition.Icon);
                    var image = new Image(ScreenManager) { Texture = texture, Width = 42, Height = 42, VerticalAlignment = VerticalAlignment.Center };
                    panel.Controls.Add(image);
                    image.MouseEnter += (_, _) => { HoveredSlot = inventorySlot; };
                    image.MouseLeave += (_, _) => { HoveredSlot = null; };
                    image.StartDrag += (_, e) =>
                    {
                        e.Handled = true;
                        e.Icon = texture;
                        e.Content = inventorySlot;
                        e.Sender = image;
                    };
                    image.LeftMouseClick += (s, e) => HoveredSlot = inventorySlot;
                    label.Background = new BorderBrush(Color.White);
                    label.Text = inventorySlot.Amount.ToString(CultureInfo.InvariantCulture);

                }
                grid.AddControl(panel, column, row);
                grid.AddControl(label, column, row);
                column++;
                if (column >= columns)
                {
                    row++;
                    column = 0;
                }
            }
        }
    }
}
