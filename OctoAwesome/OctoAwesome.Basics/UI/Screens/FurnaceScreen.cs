using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Basics.UI.Components;
using OctoAwesome.Basics.UI.Controls;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Client.UI.Controls;
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
    /// Screen container control page for Furnace screen.
    /// </summary>
    public class FurnaceScreen : BaseScreen
    {
        /// <summary>
        /// Eventhandler that gets called when this screen was closed
        /// </summary>
        public event EventHandler<NavigationEventArgs>? Closed;

        private FurnaceUIComponent FurnaceUiComponent
        {
            get => NullabilityHelper.NotNullAssert(furnaceUiComponent, $"{nameof(FurnaceUiComponent)} was not initialized!");
            set => furnaceUiComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(FurnaceUiComponent)} cannot be initialized with null!");
        }
        private InventoryControl InputInventory => NullabilityHelper.NotNullAssert(inputInventory, $"{nameof(InputInventory)} was not initialized!");

        private const string ScreenKey = "Furnace";
        private readonly AssetComponent assetComponent;
        private readonly InventoryControl? inputInventory;
        private readonly FurnaceControl furnace;
        private IDisposable? subscription;
        private FurnaceUIComponent? furnaceUiComponent;


        private enum TransferDirection
        {
            AToB,
            BToA
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurnaceScreen"/> class.
        /// </summary>
        public FurnaceScreen(AssetComponent assetComponent)
            : base(assetComponent)
        {
            Background = new BorderBrush(Color.Black * 0.3f);
            IsOverlay = true;
            this.assetComponent = assetComponent;

            var panelText = this.assetComponent.LoadTexture("panel");

            Debug.Assert(panelText != null, nameof(panelText) + " != null");
            var panelBackground = panelText;
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

            Debug.Assert(panelBackground != null, nameof(panelBackground) + " != null");
            inputInventory = new InventoryControl(assetComponent, Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            inputInventory.EndDrop += (s, e) => OnInventoryDrop(e, FurnaceUiComponent.InventoryA);

            furnace = new FurnaceControl(assetComponent, Array.Empty<InventorySlot>(),
                          Array.Empty<InventorySlot>(), Array.Empty<InventorySlot>())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            furnace.EndDrop += (s, e) =>
            {
                if (furnace.resourceSlotPanel.ActualClientArea.Contains(e.GlobalPosition - furnace.resourceSlotPanel.AbsolutePosition))
                    OnInventoryDrop(e, FurnaceUiComponent.ProductionResourceInventory);
                else
                    OnInventoryDrop(e, FurnaceUiComponent.InputInventory);
            };


            grid.AddControl(inputInventory, 0, 0);
            grid.AddControl(furnace, 0, 2);

            var infoPanel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
                Margin = Border.All(10, 0, 0, 0),
            };

            var nameLabel = new Label();
            infoPanel.Controls.Add(nameLabel);
            var massLabel = new Label();
            infoPanel.Controls.Add(massLabel);
            var volumeLabel = new Label();
            infoPanel.Controls.Add(volumeLabel);
            grid.AddControl(infoPanel, 1, 0, 1, 3);
        }

        /// <inheritdoc/>
        public override void AddUiComponent(UIComponent uiComponent)
        {
            if (uiComponent is not FurnaceUIComponent transferComponent)
                return;

            subscription?.Dispose();

            FurnaceUiComponent = transferComponent;
            subscription = transferComponent.Changes.Subscribe(InventoryChanged);
        }

        /// <inheritdoc/>
        public override void RemoveUiComponent(UIComponent uiComponent)
        {
            if (uiComponent != FurnaceUiComponent)
                return;

            subscription?.Dispose();

            furnaceUiComponent = null;
            base.RemoveUiComponent(uiComponent);
        }

        private void InventoryChanged(Unit unit)
        {
            if (FurnaceUiComponent.PrimaryUiKey != ScreenKey)
                return;

            if (FurnaceUiComponent.Show && ScreenManager.ActiveScreen != this)
            {
                _ = ScreenManager.NavigateToScreen(this);
            }

            Rebuild(FurnaceUiComponent.InventoryA, FurnaceUiComponent.InputInventory, FurnaceUiComponent.OutputInventory, FurnaceUiComponent.ProductionResourceInventory);
        }

        private void OnInventoryDrop(DragEventArgs e, InventoryComponent target)
        {
            if (e.Content is not InventorySlot slot)
                return;


            if (furnaceUiComponent is not null)
            {
                e.Handled = true;
                var source = slot.GetParentInventory();
                FurnaceUiComponent.Transfer(source, target, slot);
            }
        }

        /// <summary>
        /// Rebuild and refresh the furnace UI screen with new inventory information.
        /// </summary>
        /// <param name="inventoryComponentA">Mostly player</param>
        /// <param name="inventoryComponentB">Mostly input to furnace</param>
        /// <param name="inventoryComponentC">Mostly output of furnace</param>
        /// <param name="productionResourceInventory">Mostly fuel of furnace</param>
        internal void Rebuild(InventoryComponent inventoryComponentA, InventoryComponent inventoryComponentB, InventoryComponent inventoryComponentC, InventoryComponent productionResourceInventory)
        {
            InputInventory.Rebuild(inventoryComponentA.Inventory);
            furnace.Rebuild(inventoryComponentB.Inventory, inventoryComponentC.Inventory, productionResourceInventory.Inventory);
        }



        ///<inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (ScreenManager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                ScreenManager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        ///<inheritdoc/>
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            FurnaceUiComponent.OnClose(ScreenKey);
            base.OnNavigatedFrom(args);
            Closed?.Invoke(this, args);
        }
    }
}
