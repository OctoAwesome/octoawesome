using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using OctoAwesome.Runtime;
using System.Collections.Generic;
using System.IO;

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

            foreach (var item in DefinitionManager.Instance.GetItemDefinitions())
            {
                Texture2D texture = manager.Game.Assets.LoadTexture(item.GetType(), item.Icon);
                toolTextures.Add(item.GetType().FullName, texture);
            }

            player = manager.Player;
            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.5f);

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
            for (int i = 0; i < Player.TOOLCOUNT; i++)
                toolbar.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            toolbar.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            toolbar.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });

            images = new Image[Player.TOOLCOUNT];
            for (int i = 0; i < Player.TOOLCOUNT; i++)
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
                    InventorySlot slot = player.ActorHost.Player.Tools[(int)image.Tag];
                    if (slot != null)
                    {
                        e.Handled = true;
                        e.Icon = toolTextures[slot.Definition.GetType().FullName];
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
                        InventorySlot targetSlot = player.ActorHost.Player.Tools[targetIndex];
                        int sourceIndex = -1;
                        InventorySlot sourceSlot = e.Content as InventorySlot;

                        for (int j = 0; j < player.ActorHost.Player.Tools.Length; j++)
                        {
                            if (player.ActorHost.Player.Tools[j] == sourceSlot)
                            {
                                sourceIndex = j;
                                break;
                            }
                        }

                        SetTool(sourceSlot, targetIndex);
                        SetTool(targetSlot, sourceIndex);
                    }
                    else
                    {
                        // Inventory Drop
                        InventorySlot slot = e.Content as InventorySlot;
                        SetTool(slot, (int)image.Tag);
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
                for (int i = 0; i < player.ActorHost.Player.Tools.Length; i++)
                {
                    if (player.ActorHost.Player.Tools[i] == slot)
                        player.ActorHost.Player.Tools[i] = null;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            // Tool neu zuweisen
            switch (args.Key)
            {
                case Keys.D1: SetTool(inventory.HoveredSlot, 0); args.Handled = true; break;
                case Keys.D2: SetTool(inventory.HoveredSlot, 1); args.Handled = true; break;
                case Keys.D3: SetTool(inventory.HoveredSlot, 2); args.Handled = true; break;
                case Keys.D4: SetTool(inventory.HoveredSlot, 3); args.Handled = true; break;
                case Keys.D5: SetTool(inventory.HoveredSlot, 4); args.Handled = true; break;
                case Keys.D6: SetTool(inventory.HoveredSlot, 5); args.Handled = true; break;
                case Keys.D7: SetTool(inventory.HoveredSlot, 6); args.Handled = true; break;
                case Keys.D8: SetTool(inventory.HoveredSlot, 7); args.Handled = true; break;
                case Keys.D9: SetTool(inventory.HoveredSlot, 8); args.Handled = true; break;
                case Keys.D0: SetTool(inventory.HoveredSlot, 9); args.Handled = true; break;
            }

            if (Manager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        private void SetTool(InventorySlot slot, int index)
        {
            // Alle Slots entfernen die das selbe Tool enthalten
            for (int i = 0; i < player.ActorHost.Player.Tools.Length; i++)
            {
                if (player.ActorHost.Player.Tools[i] == slot)
                    player.ActorHost.Player.Tools[i] = null;
            }

            player.ActorHost.Player.Tools[index] = slot;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            nameLabel.Text = inventory.HoveredSlot != null ? inventory.HoveredSlot.Definition.Name : string.Empty;
            massLabel.Text = inventory.HoveredSlot != null ? inventory.HoveredSlot.Amount.ToString() : string.Empty;
            volumeLabel.Text = inventory.HoveredSlot != null ? inventory.HoveredSlot.Amount.ToString() : string.Empty;

            if (player.ActorHost == null) return;

            // Aktualisierung des aktiven Buttons
            for (int i = 0; i < Player.TOOLCOUNT; i++)
            {
                if (player.ActorHost.Player.Tools != null &&
                    player.ActorHost.Player.Tools.Length > i &&
                    player.ActorHost.Player.Tools[i] != null &&
                    player.ActorHost.Player.Tools[i].Definition != null)
                {
                    images[i].Texture = toolTextures[player.ActorHost.Player.Tools[i].Definition.GetType().FullName];
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
