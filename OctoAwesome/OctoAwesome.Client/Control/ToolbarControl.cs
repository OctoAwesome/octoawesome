﻿using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using System.Collections.Generic;
using System.Diagnostics;

namespace OctoAwesome.Client.Controls
{
    internal class ToolbarControl : Panel
    {
        public PlayerComponent Player { get; set; }

        private readonly Dictionary<string, Texture2D> toolTextures;

        private readonly Button[] buttons = new Button[ToolBarComponent.TOOLCOUNT];

        private readonly Image[] images = new Image[ToolBarComponent.TOOLCOUNT];

        private readonly Brush buttonBackgroud;

        private readonly Brush activeBackground;

        private readonly Label activeToolLabel;

        private int lastActiveIndex;

        public ToolbarControl(BaseScreenComponent screenManager, AssetComponent assets, PlayerComponent playerComponent, IDefinitionManager definitionManager)
            : base(screenManager)
        {
            Background = new SolidColorBrush(Color.Transparent);
            Player = playerComponent;
            Player.Toolbar.OnChanged += (slot, index) => SetTexture(slot, index);
            toolTextures = new Dictionary<string, Texture2D>();

            buttonBackgroud = new BorderBrush(new Color(Color.Black, 0.5f));
            activeBackground = new BorderBrush(new Color(Color.Black, 0.5f), LineType.Dotted, Color.Red, 3);

            foreach (IDefinition item in definitionManager.Definitions)
            {
                Texture2D texture = assets.LoadTexture(item.GetType(), item.Icon);
                toolTextures.Add(item.GetType().FullName, texture);
            }

            var grid = new Grid(screenManager)
            {
                Margin = new Border(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Controls.Add(grid);

            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            for (var i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            }

            activeToolLabel = new Label(screenManager)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new BorderBrush(Color.Black * 0.3f),
                TextColor = Color.White
            };
            grid.AddControl(activeToolLabel, 0, 0, ToolBarComponent.TOOLCOUNT);

            for (var i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                buttons[i] = new Button(screenManager)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = buttonBackgroud,
                    HoveredBackground = null,
                    PressedBackground = null,
                };
                buttons[i].Content = images[i] = new Image(screenManager)
                {
                    Width = 42,
                    Height = 42,
                };
                grid.AddControl(buttons[i], i, 1);
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (!Visible || !Enabled)
                return;

            if (Player.CurrentEntity == null)
                return;

            if (Player.Toolbar.ActiveIndex != lastActiveIndex)
            {
                buttons[lastActiveIndex].Background = buttonBackgroud;
                lastActiveIndex = Player.Toolbar.ActiveIndex;
            }

            buttons[Player.Toolbar.ActiveIndex].Background = activeBackground;
            SetTexture(Player.Toolbar.ActiveTool, Player.Toolbar.ActiveIndex);

            var newText = "";

            // Refresh the active tool label
            newText = Player.Toolbar.ActiveTool?.Definition?.Name ?? string.Empty;

            if (Player.Toolbar.ActiveTool?.Amount > 1)
                newText += $" ({Player.Toolbar.ActiveTool.Amount})";

            activeToolLabel.Text = newText;
            activeToolLabel.Visible = activeToolLabel.Text != string.Empty;

            base.OnUpdate(gameTime);
        }

        private void SetTexture(InventorySlot? inventorySlot, int index)
        {
            if (inventorySlot?.Definition is null)
            {
                images[index].Texture = null;
                return;
            }

            var definitionName = inventorySlot.Definition.GetType().FullName;

            Debug.Assert(definitionName != null, nameof(definitionName) + " != null");
            images[index].Texture = toolTextures.TryGetValue(definitionName, out var texture) ? texture : null;
        }
    }
}
