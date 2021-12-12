using engenious.UI;
using OctoAwesome.Client.Components;
using engenious.Graphics;
using engenious.Input;
using System.Collections.Generic;
using System.Diagnostics;
using engenious;
using OctoAwesome.EntityComponents;
using engenious.UI.Controls;
using OctoAwesome.Definitions;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Client.UI.Controls;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : Screen
    {
        private readonly Dictionary<string, Texture2D> toolTextures = new();

        private readonly PlayerComponent player;

        private readonly AssetComponent assets;

        private readonly InventoryControl inventory;

        private readonly Label nameLabel;

        private readonly Label massLabel;

        private readonly Label volumeLabel;

        private readonly Image[] images;

        private readonly Brush backgroundBrush;

        private readonly Brush hoverBrush;

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

            Texture2D panelBackground = assets.LoadTexture("panel");

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

            inventory = new InventoryControl(manager, assets, manager.Player.Inventory.Inventory)
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

                image.StartDrag += (c,e) =>
                {
                    Debug.Assert(image.Tag != null, nameof(image.Tag) + " != null");
                    var slot = player.Toolbar.Tools[(int)image.Tag];
                    if (slot != null)
                    {
                        e.Handled = true;
                        e.Icon = toolTextures[slot.Definition.GetType().FullName];
                        e.Content = slot;
                        e.Sender = toolbar;
                    }
                };

                image.DropEnter += (c,e) => { image.Background = hoverBrush; };
                image.DropLeave += (c,e) => { image.Background = backgroundBrush; };
                image.EndDrop += (c,e) =>
                {
                    e.Handled = true;
                    
                    Debug.Assert(image.Tag != null, nameof(image.Tag) + " != null");

                    if (e.Sender is Grid && e.Content is InventorySlot sourceSlot) // && ShiftPressed
                    {
                        // Swap
                        int targetIndex = (int)image.Tag;
                        var targetSlot = player.Toolbar.Tools[targetIndex];

                        if (targetSlot != null)
                        {
                            int sourceIndex = player.Toolbar.GetSlotIndex(sourceSlot);

                            player.Toolbar.SetTool(sourceSlot, targetIndex);
                            player.Toolbar.SetTool(targetSlot, sourceIndex);
                        }
                    }
                    else if(e.Content is InventorySlot slot)
                    {
                        // Inventory Drop
                        player.Toolbar.SetTool(slot, (int)image.Tag);
                    }
                };

                toolbar.AddControl(image, i + 1, 0);
            }

            grid.AddControl(toolbar, 0, 1, 2);
            Title = UI.Languages.OctoClient.Inventory;
        }

        protected override void OnEndDrop(DragEventArgs args)
        {
            base.OnEndDrop(args);

            if (args.Sender is Grid && args.Content is InventorySlot slot)
            {
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

            var name = inventory.HoveredSlot?.Definition?.Name;

            if (inventory.HoveredSlot?.Item is IItem item)
                name += " (" + item.Material.Name + ")";

            nameLabel.Text = name ?? "";
            massLabel.Text = volumeLabel.Text = inventory.HoveredSlot?.Amount.ToString() ?? "";

            // Aktualisierung des aktiven Buttons
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                var tool = player.Toolbar.Tools != null && player.Toolbar.Tools.Length > i ? player.Toolbar.Tools[i] : null;
                if (tool != null)
                {
                    Debug.Assert(tool.Definition != null, nameof(tool.Definition) + " != null");
                    var toolName = tool.Definition.GetType().FullName;

                    Debug.Assert(toolName != null, nameof(toolName) + " != null");
                    images[i].Texture = toolTextures[toolName];
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
