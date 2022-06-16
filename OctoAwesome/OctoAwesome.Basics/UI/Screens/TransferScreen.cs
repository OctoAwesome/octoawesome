using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Client.UI.Controls;

using OctoAwesome.Basics.UI.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Rx;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Screens;

using System;
using System.Diagnostics;

namespace OctoAwesome.Basics.UI.Screens
{
    /// <summary>
    /// Transfer screen to be able to transfer items between inventories.
    /// </summary>
    public class TransferScreen : BaseScreen
    {
        /// <summary>
        /// Called when the transfer screen was closed.
        /// </summary>
        public event EventHandler<NavigationEventArgs>? Closed;

        private const string ScreenKey = "Transfer";
        private readonly AssetComponent assetComponent;
        private readonly Texture2D panelBackground;
        private readonly InventoryControl inventoryA;
        private readonly InventoryControl inventoryB;
        private readonly Label nameLabel;
        private readonly Label massLabel;
        private readonly Label volumeLabel;
        private IDisposable subscription;
        private TransferUIComponent transferComponent;

        private enum TransferDirection
        {
            AToB,
            BToA
        }

        //TODO Where and how to initialize this screen?
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferScreen"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="T:engenious.UI.BaseScreenComponent" />.</param>
        /// <param name="assetComponent">The asset component to load resource assets.</param>
        /// <param name="inventoryComponentA">The inventory to show on the top.</param>
        /// <param name="inventoryComponentB">The inventory to show on the bottom.</param>
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

            inventoryA.EndDrop += (s, e) => OnInventoryDrop(e, transferComponent.InventoryA);
            inventoryA.LeftMouseClick += (s, e) => OnMouseClick(TransferDirection.BToA, e);

            inventoryB = new InventoryControl(manager, assetComponent, Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            inventoryB.EndDrop += (s, e) => OnInventoryDrop(e, transferComponent.InventoryB);
            inventoryB.LeftMouseClick += (s, e) => OnMouseClick(TransferDirection.AToB, e);

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

        private void OnMouseClick(TransferDirection transferDirection, MouseEventArgs mouseEventArgs)
        {
            var sourceControl = transferDirection == TransferDirection.AToB ? inventoryA : inventoryB;

            if (sourceControl.HoveredSlot is null)
                return;

            var keyboardState = Keyboard.GetState();
            if (!keyboardState.IsKeyDown(Keys.ShiftLeft))
                return;

            var slot = sourceControl.HoveredSlot as InventorySlot;

            mouseEventArgs.Handled = true;
            if (transferDirection == TransferDirection.AToB)
                transferComponent.Transfer(transferComponent.InventoryA, transferComponent.InventoryB, slot);
            else if (transferDirection == TransferDirection.BToA)
                transferComponent.Transfer(transferComponent.InventoryB, transferComponent.InventoryA, slot);
            else
                Debug.Fail($"{nameof(transferDirection)} has to be {nameof(TransferDirection.AToB)} or {nameof(TransferDirection.BToA)}");


        }

        public override void RemoveUiComponent(UIComponent uiComponent)
        {
            if (uiComponent != transferComponent)
                return;

            if (subscription is not null)
                subscription.Dispose();

            transferComponent = null;
            base.RemoveUiComponent(uiComponent);
        }

        private void InventoryChanged(Unit unit)
        {
            if (transferComponent.PrimaryUiKey != ScreenKey)
                return;

            if (transferComponent.Show && Manager.ActiveScreen != this)
            {
                _ = Manager.NavigateToScreen(this);
            }

            Rebuild(transferComponent.InventoryA, transferComponent.InventoryB);
        }

        protected override void OnUpdate(GameTime gameTime)
        {

            base.OnUpdate(gameTime);
        }

        private void OnInventoryDrop(DragEventArgs e, InventoryComponent target)
        {
            if (transferComponent is not null && e.Content is InventorySlot slot)
            {
                var source = slot.GetParentInventory();
                e.Handled = true;
                transferComponent.Transfer(source, target, slot);
                //if (source.RemoveSlot(slot))
                //    target.AddSlot(slot);
            }
        }

        private static void MoveSlot(IInventorySlot slot, InventoryComponent source, InventoryComponent target)
        {
            var toAddAndRemove = target.GetQuantityLimitFor(slot.Item, slot.Amount);
            if (toAddAndRemove == 0)
                return;
            var item = slot.Item;
            var amount = source.Remove(slot, toAddAndRemove);

            var addedAddedAmount = target.Add(item, toAddAndRemove);
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
            transferComponent.OnClose(ScreenKey);
            base.OnNavigatedFrom(args);
            Closed?.Invoke(this, args);
        }
    }
}
