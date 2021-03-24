using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Controls;

namespace OctoAwesome.Basics.UI.Screens
{
    public class TransferScreen : Screen
    {
        private readonly AssetComponent assetComponent;
        private readonly Texture2D panelBackground;
        private readonly BorderBrush backgroundBrush;
        private readonly BorderBrush hoverBrush;
        private readonly InventoryControl inventory;
        private readonly Label nameLabel;
        private readonly Label massLabel;
        private readonly Label volumeLabel;

        public TransferScreen(BaseScreenComponent manager, AssetComponent assetComponent) : base(manager)
        {
            Background = new BorderBrush(Color.Black * 0.3f);
            IsOverlay = true;
            this.assetComponent = assetComponent;
            panelBackground = this.assetComponent.LoadTexture("panel");
            backgroundBrush = new BorderBrush(Color.Black);
            hoverBrush = new BorderBrush(Color.Brown);
            Grid grid = new Grid(manager)
            {
                Width = 800,
                Height = 500,
            };

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 600 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 200 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 100 });

            Controls.Add(grid);

            inventory = new InventoryControl(manager, assetComponent, new System.Collections.Generic.List<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            grid.AddControl(inventory, 0, 0);

            StackPanel infoPanel = new StackPanel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
                Margin = Border.All(10, 0, 0, 0),
            };

            nameLabel = new Label(manager);
            infoPanel.Controls.Add(nameLabel);
            massLabel = new Label(manager);
            infoPanel.Controls.Add(massLabel);
            volumeLabel = new Label(manager);
            infoPanel.Controls.Add(volumeLabel);
            grid.AddControl(infoPanel, 1, 0);
        }
        protected override void OnKeyDown(KeyEventArgs args)
        {
    
            if (Manager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }
    }
}
