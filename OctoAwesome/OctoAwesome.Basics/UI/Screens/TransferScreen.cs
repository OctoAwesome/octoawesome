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
using OctoAwesome.Extension;

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
        private IDisposable? subscription;
        private TransferUIComponent? transferComponent;

        private TransferUIComponent TransferComponent
        {
            get => NullabilityHelper.NotNullAssert(transferComponent, $"{nameof(TransferComponent)} was not initialized!");
            set => transferComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(TransferComponent)} cannot be initialized with null!");
        }

        private enum TransferDirection
        {
            AToB,
            BToA
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferScreen"/> class.
        /// </summary>
        /// <param name="assetComponent">The asset component to load resource assets.</param>
        public TransferScreen(AssetComponent assetComponent)
            : base(assetComponent)
        {
            Background = new BorderBrush(Color.Black * 0.3f);
            IsOverlay = true;
            this.assetComponent = assetComponent;

            var panelText = this.assetComponent.LoadTexture("panel");

            Debug.Assert(panelText != null, nameof(panelText) + " != null");
            panelBackground = panelText;
            var grid = new Grid()
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

            inventoryA = new InventoryControl(assetComponent, Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            inventoryA.EndDrop += (s, e) => OnInventoryDrop(e, TransferComponent.InventoryA);
            inventoryA.LeftMouseClick += (s, e) => OnMouseClick(TransferDirection.BToA, e);

            inventoryB = new InventoryControl(assetComponent, Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            inventoryB.EndDrop += (s, e) => OnInventoryDrop(e, TransferComponent.InventoryB);
            inventoryB.LeftMouseClick += (s, e) => OnMouseClick(TransferDirection.AToB, e);

            grid.AddControl(inventoryA, 0, 0);
            grid.AddControl(inventoryB, 0, 2);

            var infoPanel = new StackPanel()
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
            grid.AddControl(infoPanel, 1, 0, 1, 3);
        }

        /// <inheritdoc/>
        public override void AddUiComponent(UIComponent uiComponent)
        {
            if (uiComponent is not TransferUIComponent transferComponent)
                return;

            subscription?.Dispose();

            TransferComponent = transferComponent;
            subscription = transferComponent.Changes.Subscribe(InventoryChanged);
        }

        private void OnMouseClick(TransferDirection transferDirection, MouseEventArgs mouseEventArgs)
        {
            var sourceControl = transferDirection == TransferDirection.AToB ? inventoryA : inventoryB;

            if (sourceControl.HoveredSlot is null)
                return;

            var keyboardState = Keyboard.GetState();
            if (!keyboardState.IsKeyDown(Keys.ShiftLeft))
                return;

            var slot = sourceControl.HoveredSlot;

            mouseEventArgs.Handled = true;
            if (transferDirection == TransferDirection.AToB)
                TransferComponent.Transfer(TransferComponent.InventoryA, TransferComponent.InventoryB, slot);
            else if (transferDirection == TransferDirection.BToA)
                TransferComponent.Transfer(TransferComponent.InventoryB, TransferComponent.InventoryA, slot);
            else
                Debug.Fail($"{nameof(transferDirection)} has to be {nameof(TransferDirection.AToB)} or {nameof(TransferDirection.BToA)}");


        }

        /// <inheritdoc/>
        public override void RemoveUiComponent(UIComponent uiComponent)
        {
            if (uiComponent != transferComponent)
                return;

            subscription?.Dispose();

            transferComponent = null;
            base.RemoveUiComponent(uiComponent);
        }

        private void InventoryChanged(Unit unit)
        {
            if (TransferComponent.PrimaryUiKey != ScreenKey)
                return;

            if (TransferComponent.Show && ScreenManager.ActiveScreen != this)
            {
                _ = ScreenManager.NavigateToScreen(this);
            }

            Rebuild(TransferComponent.InventoryA, TransferComponent.InventoryB);
        }

        /// <inheritdoc/>
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
                TransferComponent.Transfer(source, target, slot);
            }
        }

        private static void MoveSlot(IInventorySlot slot, InventoryComponent source, InventoryComponent target)
        {
            var item = slot.Item;
            if (item is null)
                return;
            var toAddAndRemove = target.GetQuantityLimitFor(item, slot.Amount);
            if (toAddAndRemove == 0)
                return;
            var amount = source.Remove(slot, toAddAndRemove);

            var addedAddedAmount = target.Add(item, toAddAndRemove);
            Debug.Assert(amount == addedAddedAmount, "The added value and removed value of the inventories is unequal, threading?");
        }

        internal void Rebuild(InventoryComponent inventoryComponentA, InventoryComponent inventoryComponentB)
        {
            inventoryA.Rebuild(inventoryComponentA.Inventory);
            inventoryB.Rebuild(inventoryComponentB.Inventory);
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (ScreenManager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                ScreenManager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        /// <inheritdoc />
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            TransferComponent.OnClose(ScreenKey);
            base.OnNavigatedFrom(args);
            Closed?.Invoke(this, args);
        }
    }
}
