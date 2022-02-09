using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Basics.UI.Components;
using OctoAwesome.Basics.UI.Controls;
using OctoAwesome.EntityComponents;
using OctoAwesome.Rx;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Controls;
using OctoAwesome.UI.Screens;

using System;
using System.Diagnostics;

namespace OctoAwesome.Basics.UI.Screens
{
    public class FurnaceScreen : BaseScreen
    {
        public event EventHandler<NavigationEventArgs> Closed;

        private const string ScreenKey = "Furnace";
        private readonly AssetComponent assetComponent;
        private readonly Texture2D panelBackground;
        private readonly InventoryControl entityInventory;
        private readonly FurnaceControl furnace;
        private readonly Label nameLabel;
        private readonly Label massLabel;
        private readonly Label volumeLabel;
        private IDisposable subscription;
        private FurnaceUIComponent furnaceUIComponent;

        private enum TransferDirection
        {
            AToB,
            BToA
        }

        public FurnaceScreen(BaseScreenComponent manager, AssetComponent assetComponent) : base(manager, assetComponent)
        {
            Background = new BorderBrush(Color.Black * 0.3f);
            IsOverlay = true;
            this.assetComponent = assetComponent;


            panelBackground = this.assetComponent.LoadTexture("panel");
            var grid = new Grid(manager)
            {
                Width = 800,
                Height = 500,
            };

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 600 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 200 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 100 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 5, });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 100 });

            Controls.Add(grid);

            entityInventory = new InventoryControl(manager, assetComponent, Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            entityInventory.EndDrop += (s, e) => OnInventoryDrop(TransferDirection.BToA, e);

            furnace = new FurnaceControl(manager, assetComponent, Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            furnace.EndDrop += (s, e) => OnInventoryDrop(TransferDirection.AToB, e);

            grid.AddControl(entityInventory, 0, 0);
            grid.AddControl(furnace, 0, 2);

            var infoPanel = new StackPanel(manager)
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
            grid.AddControl(infoPanel, 1, 0, 1, 3);
        }

        /// <inheritdoc/>
        public override void AddUiComponent(UIComponent uiComponent)
        {
            if (uiComponent is not FurnaceUIComponent transferComponent)
                return;

            if (subscription is not null)
                subscription.Dispose();

            this.furnaceUIComponent = transferComponent;
            subscription = transferComponent.Changes.Subscribe(InventoryChanged);
        }
        /// <inheritdoc/>

        public override void RemoveUiComponent(UIComponent uiComponent)
        {
            if (uiComponent != furnaceUIComponent)
                return;

            if (subscription is not null)
                subscription.Dispose();

            furnaceUIComponent = null;
            base.RemoveUiComponent(uiComponent);
        }

        private void InventoryChanged(Unit unit)
        {
            if (furnaceUIComponent.PrimaryUiKey != ScreenKey)
                return;

            if (furnaceUIComponent.Show && Manager.ActiveScreen != this)
            {
                _ = Manager.NavigateToScreen(this);
            }

            Rebuild(furnaceUIComponent.InventoryA, furnaceUIComponent.InputInventory);
        }

        private void OnInventoryDrop(TransferDirection transferDirection, DragEventArgs e)
        {
            if (furnaceUIComponent is not null && e.Content is InventorySlot slot)
            {
                e.Handled = true;
                if (transferDirection == TransferDirection.AToB)
                    furnaceUIComponent.Transfer(furnaceUIComponent.InventoryA, furnaceUIComponent.InputInventory, slot);
                else if (transferDirection == TransferDirection.BToA)
                    furnaceUIComponent.Transfer(furnaceUIComponent.InputInventory, furnaceUIComponent.InventoryA, slot);
                else
                    Debug.Fail($"{nameof(transferDirection)} has to be {nameof(TransferDirection.AToB)} or {nameof(TransferDirection.BToA)}");
            }
        }

        internal void Rebuild(InventoryComponent inventoryComponentA, InventoryComponent inventoryComponentB)
        {
            entityInventory.Rebuild(inventoryComponentA.Inventory);
            furnace.Rebuild(inventoryComponentB.Inventory);
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

        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            furnaceUIComponent.OnClose(ScreenKey);
            base.OnNavigatedFrom(args);
            Closed?.Invoke(this, args);
        }
    }
}
