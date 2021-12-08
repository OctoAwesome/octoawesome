using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Client.UI.Controls;

using OctoAwesome.Rx;
using OctoAwesome.Basics.UI.Components;
using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using NLog.Targets;
using System.Diagnostics;

namespace OctoAwesome.Basics.UI.Screens
{
    /// <summary>
    /// Transfer screen to be able to transfer items between inventories.
    /// </summary>
    public class TransferScreen : Screen
    {
        /// <summary>
        /// Called when the transfer screen was closed.
        /// </summary>
        public event EventHandler<NavigationEventArgs>? Closed;

        private readonly IDisposable subscription;
        private readonly AssetComponent assetComponent;
        private readonly TransferUIComponent transferComponent;
        private readonly Texture2D panelBackground;
        private readonly InventoryControl inventoryA;
        private readonly InventoryControl inventoryB;
        private readonly Label nameLabel;
        private readonly Label massLabel;
        private readonly Label volumeLabel;
        private TransferModel currentTransferModel;

        enum TransferDirection
        {
            AToB,
            BToA
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferScreen"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="T:engenious.UI.BaseScreenComponent" />.</param>
        /// <param name="assetComponent">The asset component to load resource assets.</param>
        /// <param name="inventoryComponentA">The inventory to show on the top.</param>
        /// <param name="inventoryComponentB">The inventory to show on the bottom.</param>
        public TransferScreen(BaseScreenComponent manager, AssetComponent assetComponent, TransferUIComponent transferComponent) : base(manager)
        {
            Background = new BorderBrush(Color.Black * 0.3f);
            IsOverlay = true;
            this.assetComponent = assetComponent;
            this.transferComponent = transferComponent;
            subscription = transferComponent.Changes.Subscribe(InventoryChanged);

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
            inventoryA.LeftMouseClick += (s, e) => OnMouseClick(inventoryA, componentA, inventoryB, componentB, e);

            inventoryB = new InventoryControl(manager, assetComponent, Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            inventoryB.EndDrop += (s, e) => OnInventoryDrop(TransferDirection.AToB, e);
            inventoryB.LeftMouseClick += (s, e) => OnMouseClick(inventoryB, componentB, inventoryA, componentA, e);

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

        private static void OnMouseClick(InventoryControl sourceControl, InventoryComponent source, InventoryControl targetControl, InventoryComponent target, MouseEventArgs mouseEventArgs)
        {
            if (sourceControl.HoveredSlot is null)
                return;

            var keyboardState = Keyboard.GetState();
            if (!keyboardState.IsKeyDown(Keys.ShiftLeft))
                return;

            var slot = sourceControl.HoveredSlot;

            mouseEventArgs.Handled = true;

            MoveSlot(slot, sourceControl, source, targetControl, target);
        }

        private void InventoryChanged(TransferModel transferModel)
        {
            if(transferModel.Transferring && Manager.ActiveScreen != this)
            {
                _ = Manager.NavigateToScreen(this);
            }

            currentTransferModel = transferModel;
            Rebuild(transferModel.InventoryA, transferModel.InventoryB);
        }

        private void OnInventoryDrop(TransferDirection transferDirection, DragEventArgs e)
        {
            if (e.Content is IInventorySlot slot)
            {
                e.Handled = true;
                if (transferDirection == TransferDirection.AToB)
                    transferComponent.Transfer(currentTransferModel.InventoryA, currentTransferModel.InventoryB, slot);
                else if (transferDirection == TransferDirection.BToA)
                    transferComponent.Transfer(currentTransferModel.InventoryB, currentTransferModel.InventoryA, slot);
                else
                    Debug.Fail($"{nameof(transferDirection)} has to be {nameof(TransferDirection.AToB)} or {nameof(TransferDirection.BToA)}");
                //if (source.RemoveSlot(slot))
                //    target.AddSlot(slot);
                MoveSlot(slot, sourceControl, source, targetControl, target);
            }
        }

        private static void MoveSlot(IInventorySlot slot, InventoryControl sourceControl, InventoryComponent source, InventoryControl targetControl, InventoryComponent target)
        {
            var toAddAndRemove = target.GetQuantityLimitFor(slot.Item, slot.Amount);
            if (toAddAndRemove == 0)
                return;
            var item = slot.Item;
            var amount = source.Remove(slot, toAddAndRemove);

            var addedAddedAmount= target.Add(item, toAddAndRemove);
            Debug.Assert(amount == addedAddedAmount, "The added value and removed value of the inventories is unequal, threading?");
            //sourceControl.Rebuild(source.Inventory);
            //targetControl.Rebuild(target.Inventory);
        }

        internal void Rebuild(InventoryComponent inventoryComponentA, InventoryComponent inventoryComponentB)
        {
            inventoryA.Rebuild(inventoryComponentA.Inventory);
            inventoryB.Rebuild(inventoryComponentB.Inventory);
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (Manager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        /// <inheritdoc />
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);
            Closed?.Invoke(this, args);
        }
    }
}
