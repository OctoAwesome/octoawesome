using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Rx;
using OctoAwesome.Basics.UI.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Controls;
using System;
using System.Collections.Generic;
using NLog.Targets;
using System.Diagnostics;
using OctoAwesome.UI.Screens;

namespace OctoAwesome.Basics.UI.Screens
{
    public class TransferScreen : BaseScreen
    {
        public event EventHandler<NavigationEventArgs> Closed;

        private readonly AssetComponent assetComponent;
        private readonly Texture2D panelBackground;
        private readonly InventoryControl inventoryA;
        private readonly InventoryControl inventoryB;
        private readonly Label nameLabel;
        private readonly Label massLabel;
        private readonly Label volumeLabel;
        private IDisposable subscription;
        private TransferUIComponent transferComponent;
        private TransferModel currentTransferModel;

        enum TransferDirection
        {
            AToB,
            BToA
        }

        //TODO Where and how to initialize this screen?

        public TransferScreen(BaseScreenComponent manager, AssetComponent assetComponent) : base(manager, assetComponent)
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

            inventoryA = new InventoryControl(manager, assetComponent, Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            inventoryA.EndDrop += (s, e) => OnInventoryDrop(TransferDirection.BToA, e);

            inventoryB = new InventoryControl(manager, assetComponent, Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            inventoryB.EndDrop += (s, e) => OnInventoryDrop(TransferDirection.AToB, e);

            grid.AddControl(inventoryA, 0, 0);
            grid.AddControl(inventoryB, 0, 2);

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
            if (uiComponent is not TransferUIComponent transferComponent)
                return;

            if (subscription is not null)
                subscription.Dispose();

            this.transferComponent = transferComponent;
            subscription = transferComponent.Changes.Subscribe(InventoryChanged);
        }
        /// <inheritdoc/>

        public override void RemoveUiComponent(UIComponent uiComponent)
        {
            if (uiComponent != transferComponent)
                return;

            if (subscription is not null)
                subscription.Dispose();

            transferComponent = null;
            base.RemoveUiComponent(uiComponent);
        }

        private void InventoryChanged(TransferModel transferModel)
        {
            if (transferModel.Transferring && Manager.ActiveScreen != this)
            {
                _ = Manager.NavigateToScreen(this);
            }

            currentTransferModel = transferModel;
            Rebuild(transferModel.InventoryA, transferModel.InventoryB);
        }

        private void OnInventoryDrop(TransferDirection transferDirection, DragEventArgs e)
        {
            if (transferComponent is not null && e.Content is InventorySlot slot)
            {
                e.Handled = true;
                if (transferDirection == TransferDirection.AToB)
                    transferComponent.Transfer(currentTransferModel.InventoryA, currentTransferModel.InventoryB, slot);
                else if (transferDirection == TransferDirection.BToA)
                    transferComponent.Transfer(currentTransferModel.InventoryB, currentTransferModel.InventoryA, slot);
                else
                    Debug.Fail($"{nameof(transferDirection)} has to be {nameof(TransferDirection.AToB)} or {nameof(TransferDirection.BToA)}");
            }
        }

        internal void Rebuild(InventoryComponent inventoryComponentA, InventoryComponent inventoryComponentB)
        {
            inventoryA.Rebuild(inventoryComponentA.Inventory);
            inventoryB.Rebuild(inventoryComponentB.Inventory);
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
            transferComponent.OnClose();
            base.OnNavigatedFrom(args);
            Closed?.Invoke(this, args);
        }
    }
}
