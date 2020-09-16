using engenious.UI;
using OctoAwesome.Client.Components;
using engenious.Graphics;
using engenious.Input;
using System.Collections.Generic;
using OctoAwesome.Client.Controls;
using engenious;
using OctoAwesome.EntityComponents;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : Screen
    {
        private Dictionary<string, Texture2D> toolTextures = new Dictionary<string, Texture2D>();

        private PlayerComponent player;

        private AssetComponent assets;

        private InventoryControl inventory;

        private Label nameLabel;

        private Label massLabel;

        private Label volumeLabel;

        private Image[] images;

        private Brush backgroundBrush;

        private Brush hoverBrush;

        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            assets = manager.Game.Assets;

            foreach (var item in manager.Game.DefinitionManager.Definitions)
            {
                Texture2D texture = manager.Game.Assets.LoadTexture(item.GetType(), item.Icon);
                toolTextures.Add(item.GetType().FullName, texture);
            }

            player = manager.Player;

            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.3f);

            backgroundBrush = new BorderBrush(Color.Black);
            hoverBrush = new BorderBrush(Color.Brown);

            Texture2D panelBackground = assets.LoadTexture(typeof(ScreenComponent), "panel");

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

            inventory = new InventoryControl(manager)
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

            Grid toolbar = new Grid(manager)
            {
                Margin = Border.All(0, 10, 0, 0),
                Height = 100,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
            };

            toolbar.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
                toolbar.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            toolbar.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            toolbar.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });

            images = new Image[ToolBarComponent.TOOLCOUNT];
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                Image image = images[i] = new Image(manager)
                {
                    Width = 42,
                    Height = 42,
                    Background = backgroundBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Tag = i,
                    Padding = Border.All(2),
                };

                image.StartDrag += (e) =>
                {
                    InventorySlot slot = player.Toolbar.Tools[(int)image.Tag];
                    if (slot != null)
                    {
                        e.Handled = true;
                        e.Icon = toolTextures[slot.Item.GetType().FullName];
                        e.Content = slot;
                        e.Sender = toolbar;
                    }
                };

                image.DropEnter += (e) => { image.Background = hoverBrush; };
                image.DropLeave += (e) => { image.Background = backgroundBrush; };
                image.EndDrop += (e) =>
                {
                    e.Handled = true;

                    if (e.Sender is Grid) // && ShiftPressed
                    {
                        // Swap
                        int targetIndex = (int)image.Tag;
                        InventorySlot targetSlot = player.Toolbar.Tools[targetIndex];

                        InventorySlot sourceSlot = e.Content as InventorySlot;
                        int sourceIndex = player.Toolbar.GetSlotIndex(sourceSlot);

                        player.Toolbar.SetTool(sourceSlot, targetIndex);
                        player.Toolbar.SetTool(targetSlot, sourceIndex);
                    }
                    else
                    {
                        // Inventory Drop
                        InventorySlot slot = e.Content as InventorySlot;
                        player.Toolbar.SetTool(slot, (int)image.Tag);
                    }
                };

                toolbar.AddControl(image, i + 1, 0);
            }

            grid.AddControl(toolbar, 0, 1, 2);
            Title = Languages.OctoClient.Inventory;
        }

        protected override void OnEndDrop(DragEventArgs args)
        {
            base.OnEndDrop(args);

            if (args.Sender is Grid)
            {
                InventorySlot slot = args.Content as InventorySlot;
                player.Toolbar.RemoveSlot(slot);
            }
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            // Tool neu zuweisen
            if ((int)args.Key >= (int)Keys.D0 && (int)args.Key <= (int)Keys.D9)
            {
                int offset = (int)args.Key - (int)Keys.D0;
                player.Toolbar.SetTool(inventory.HoveredSlot, offset);
                args.Handled = true;
            }

            if (Manager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            nameLabel.Text = inventory.HoveredSlot?.Definition?.Name ?? "";
            massLabel.Text = volumeLabel.Text = inventory.HoveredSlot?.Amount.ToString() ?? "";

            // Aktualisierung des aktiven Buttons
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                if (player.Toolbar.Tools != null &&
                    player.Toolbar.Tools.Length > i &&
                    player.Toolbar.Tools[i] != null &&
                    player.Toolbar.Tools[i].Item != null)
                {
                    images[i].Texture = toolTextures[player.Toolbar.Tools[i].Definition.GetType().FullName];
                }
                else
                {
                    images[i].Texture = null;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
