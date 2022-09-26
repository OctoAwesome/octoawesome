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
using OctoAwesome.Extension;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : OctoScreen
    {
        private readonly Dictionary<string, Texture2D> toolTextures = new();

        private readonly PlayerComponent player;

        private readonly InventoryControl inventory;

        private readonly Label nameLabel;

        private readonly Label massLabel;

        private readonly Label volumeLabel;

        private readonly Image[] images;

        private readonly Brush backgroundBrush;

        private readonly Brush hoverBrush;

        public InventoryScreen(AssetComponent assets)
            : base(assets)
        {
            foreach (var item in ScreenManager.Game.DefinitionManager.Definitions)
            {
                var texture = assets.LoadTexture(item.GetType(), item.Icon);
                if (texture is null)
                    continue;
                toolTextures.Add(
                    NullabilityHelper.NotNullAssert(item.GetType().FullName, "Tool item definition type name is null!"),
                    texture);
            }

            player = ScreenManager.Player;

            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.3f);

            backgroundBrush = new BorderBrush(Color.Black);
            hoverBrush = new BorderBrush(Color.Brown);

            var panelBackground = assets.LoadTexture("panel");
            Debug.Assert(panelBackground != null, nameof(panelBackground) + " != null");
            Grid grid = new Grid()
            {
                Width = 800,
                Height = 500,
            };

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 600 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 200 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 100 });

            Controls.Add(grid);

            inventory = new InventoryControl(assets, ScreenManager.Player.Inventory.Inventory)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            grid.AddControl(inventory, 0, 0);

            StackPanel infoPanel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
                Margin = Border.All(10, 0, 0, 0),
            };

            nameLabel = new Label();
            infoPanel.Controls.Add(nameLabel);
            massLabel = new Label();
            infoPanel.Controls.Add(massLabel);
            volumeLabel = new Label();
            infoPanel.Controls.Add(volumeLabel);
            grid.AddControl(infoPanel, 1, 0);

            Grid toolbar = new Grid()
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
                Image image = images[i] = new Image()
                {
                    Width = 42,
                    Height = 42,
                    Background = backgroundBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Tag = i,
                    Padding = Border.All(2),
                };

                image.StartDrag += (c, e) =>
                {
                    Debug.Assert(image.Tag != null, nameof(image.Tag) + " != null");
                    var slot = player.Toolbar.Tools[(int)image.Tag];
                    if (slot is { Definition: { } slotDefinition })
                    {
                        e.Handled = true;
                        e.Icon = toolTextures.TryGetValue(NullabilityHelper.NotNullAssert(
                            slotDefinition.GetType().FullName,
                            "Slot definition type name not null!"), out var texture)
                            ? texture
                            : null;
                        e.Content = slot;
                        e.Sender = toolbar;
                    }
                };

                image.DropEnter += (c, e) => { image.Background = hoverBrush; };
                image.DropLeave += (c, e) => { image.Background = backgroundBrush; };
                image.EndDrop += (c, e) =>
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
                    else if (e.Content is InventorySlot slot)
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
            // Reassign tool
            if ((int)args.Key >= (int)Keys.D0 && (int)args.Key <= (int)Keys.D9)
            {
                int offset = (int)args.Key - (int)Keys.D0;
                var invSlot = inventory.HoveredSlot as InventorySlot;
                Debug.Assert(invSlot != null, nameof(invSlot) + " != null");
                player.Toolbar.SetTool(invSlot, offset);
                args.Handled = true;
            }

            if (ScreenManager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                ScreenManager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            var name = inventory.HoveredSlot?.Definition?.DisplayName;

            if (inventory.HoveredSlot?.Item is IItem item)
                name += " (" + item.Material.DisplayName + ")";

            nameLabel.Text = name ?? "";
            massLabel.Text = volumeLabel.Text = inventory.HoveredSlot?.Amount.ToString() ?? "";

            // Refresh the active button
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                var tool = player.Toolbar.Tools != null && player.Toolbar.Tools.Length > i ? player.Toolbar.Tools[i] : null;
                if (tool != null)
                {
                    Debug.Assert(tool.Definition != null, nameof(tool.Definition) + " != null");
                    var toolName = tool.Definition.GetType().FullName;

                    Debug.Assert(toolName != null, nameof(toolName) + " != null");
                    images[i].Texture = toolTextures.TryGetValue(toolName, out var texture) ? texture : null;
                }
                else
                {
                    images[i].Texture = null;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            ScreenManager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
