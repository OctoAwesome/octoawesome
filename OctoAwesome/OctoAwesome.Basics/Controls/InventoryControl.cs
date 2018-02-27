using engenious;
using engenious.Graphics;
using MonoGameUi;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Entities;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Basics.Controls
{
    internal sealed class InventoryControl : Panel
    {
        private const int COLUMNS = 8;

        /// <summary>
        /// Gibt den aktuell selektierten Slot an.
        /// </summary>
        public InventorySlot HoveredSlot { get; private set; }

        private InventoryComponent inventory;
        private Label volumeLabel;
        private Label massLabel;
        private Label nameLabel;

        public InventoryControl(BaseScreenComponent manager, IUserInterfaceExtensionManager interfacemanager, 
            InventoryComponent inventory) : base(manager)
        {
            this.inventory = inventory;

            Texture2D panelBackground = interfacemanager.LoadTextures(manager.GetType(), "panel");
            Margin = new Border(0, 0, 0, 0);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            Grid parentgrid = new Grid(manager);
            parentgrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1, });
            parentgrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1, });
            parentgrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 100, });
            Controls.Add(parentgrid);

            Panel panel = new Panel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30)
            };
            parentgrid.AddControl(panel, 0, 0);

            ScrollContainer scroll = new ScrollContainer(manager)
            {
                Margin = Border.All(0, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            panel.Controls.Add(scroll);

            StackPanel infoPanel = new StackPanel(manager)
            {
                Margin = new Border(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
            };

            nameLabel = new Label(manager);
            infoPanel.Controls.Add(nameLabel);
            massLabel = new Label(manager);
            infoPanel.Controls.Add(massLabel);
            volumeLabel = new Label(manager);
            infoPanel.Controls.Add(volumeLabel);
            parentgrid.AddControl(infoPanel, 0, 1);

            Grid grid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            for (int i = 0; i < COLUMNS; i++)
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });

            int rows = (int) System.Math.Ceiling((float) inventory.Inventory.Count / COLUMNS);
            for (int i = 0; i < rows; i++)
                grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            int column = 0;
            int row = 0;
            foreach (var item in inventory.Inventory)
            {
                Texture2D texture = interfacemanager.LoadTextures(item.Definition.GetType(), item.Definition.Icon);

                var image = new Image(manager) { Texture = texture, Width = 42, Height = 42, VerticalAlignment = VerticalAlignment.Center };
                image.MouseEnter += (s, e) => { HoveredSlot = item; };
                image.MouseLeave += (s, e) => { HoveredSlot = null; };
                image.StartDrag += (e) =>
                {
                    e.Handled = true;
                    e.Icon = texture;
                    e.Content = item;
                    e.Sender = image;
                };
                var label = new Label(manager) { Text = item.Amount.ToString(), HorizontalAlignment = HorizontalAlignment.Right, VerticalTextAlignment = VerticalAlignment.Bottom, Background = new BorderBrush(Color.White) };
                grid.AddControl(image, column, row);
                grid.AddControl(label, column, row);

                column++;
                if (column >= COLUMNS)
                {
                    row++;
                    column = 0;
                }
            }
            scroll.Content = grid;
        }
        protected override void OnUpdate(GameTime gameTime)
        {
            nameLabel.Text = HoveredSlot?.Definition.Name ?? "";
            massLabel.Text = volumeLabel.Text = HoveredSlot?.Amount.ToString() ?? "";
        }
        public override string ToString()
        {
            // TODO: resx ->  Languages.OctoBasics.Inventory or what ever...
            return "Inventar";
        }
    }
}
