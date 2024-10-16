﻿using System;
using System.Diagnostics;

using engenious;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.UI;
using OctoAwesome.UI.Components;

using OctoAwesome.Client.UI;
using OctoAwesome.Client.UI.Components;

namespace OctoAwesome.Client.Controls
{
    internal sealed class ResourcePacksOptionControl : Panel
    {
        private readonly Button addButton;
        private readonly Button removeButton;
        private readonly Button moveUpButton;
        private readonly Button moveDownButton;
        private readonly Button applyButton;
        private readonly Listbox<ResourcePack> loadedPacksList;
        private readonly Listbox<ResourcePack> activePacksList;
        private readonly Label infoLabel;

        public ResourcePacksOptionControl(AssetComponent asset)
        {
            Grid grid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = Border.All(15),
            };
            Controls.Add(grid);

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 100 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });

            StackPanel buttons = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            grid.AddControl(buttons, 1, 0);

            #region Manipulationsbuttons

            addButton = new TextButton(UI.Languages.OctoClient.Add);
            addButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            addButton.Visible = false;
            buttons.Controls.Add(addButton);

            removeButton = new TextButton(UI.Languages.OctoClient.Remove);
            removeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            removeButton.Visible = false;
            buttons.Controls.Add(removeButton);

            moveUpButton = new TextButton(UI.Languages.OctoClient.Up);
            moveUpButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            moveUpButton.Visible = false;
            buttons.Controls.Add(moveUpButton);

            moveDownButton = new TextButton(UI.Languages.OctoClient.Down);
            moveDownButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            moveDownButton.Visible = false;
            buttons.Controls.Add(moveDownButton);

            #endregion

            applyButton = new TextButton(UI.Languages.OctoClient.Apply);
            applyButton.HorizontalAlignment = HorizontalAlignment.Right;
            applyButton.VerticalAlignment = VerticalAlignment.Bottom;
            grid.AddControl(applyButton, 0, 2, 3);

            infoLabel = new Label()
            {
                HorizontalTextAlignment = HorizontalAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                WordWrap = true,
            };
            grid.AddControl(infoLabel, 0, 1, 3);

            #region Listen

            loadedPacksList = new Listbox<ResourcePack>()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
                TemplateGenerator = ListTemplateGenerator,
            };

            grid.AddControl(loadedPacksList, 0, 0);

            activePacksList = new Listbox<ResourcePack>()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
                TemplateGenerator = ListTemplateGenerator,
            };

            grid.AddControl(activePacksList, 2, 0);

            #endregion

            #region Info Grid

            //Grid infoGrid = new Grid(ScreenManager)
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //};

            //infoGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Auto, Width = 1 });
            //infoGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            //infoGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });
            //infoGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });
            //infoGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });

            //Label nameLabel = new Label(ScreenManager)
            //{
            //    Text = "Name:",
            //};
            //infoGrid.AddControl(nameLabel, 0, 0);

            //Label authorLabel = new Label(ScreenManager)
            //{
            //    Text = "Author:",
            //};
            //infoGrid.AddControl(authorLabel, 0, 1);

            //Label descriptionLabel = new Label(ScreenManager)
            //{
            //    Text = "Description:",
            //};
            //infoGrid.AddControl(descriptionLabel, 0, 2);

            //grid.AddControl(infoGrid, 0, 1, 3);

            #endregion

            loadedPacksList.SelectedItemChanged += loadedList_SelectedItemChanged;
            activePacksList.SelectedItemChanged += activeList_SelectedItemChanged;

            addButton.LeftMouseClick += (s, e) =>
            {
                var pack = loadedPacksList.SelectedItem;
                if (pack == null)
                    throw new NullReferenceException("Add button should not be clickable when no item is selected.");

                loadedPacksList.Items.Remove(pack);
                activePacksList.Items.Add(pack);
                activePacksList.SelectedItem = pack;
            };

            removeButton.LeftMouseClick += (s, e) =>
            {
                var pack = activePacksList.SelectedItem;
                if (pack == null)
                    throw new NullReferenceException("Remove button should not be clickable when no item is selected.");

                activePacksList.Items.Remove(pack);
                loadedPacksList.Items.Add(pack);
                loadedPacksList.SelectedItem = pack;
            };

            moveUpButton.LeftMouseClick += (s, e) =>
            {
                var pack = activePacksList.SelectedItem;
                if (pack == null)
                    return;

                int index = activePacksList.Items.IndexOf(pack);
                if (index > 0)
                {
                    activePacksList.Items.Remove(pack);
                    activePacksList.Items.Insert(index - 1, pack);
                    activePacksList.SelectedItem = pack;
                }
            };

            moveDownButton.LeftMouseClick += (s, e) =>
            {
                var pack = activePacksList.SelectedItem;
                if (pack == null)
                    return;

                int index = activePacksList.Items.IndexOf(pack);
                if (index < activePacksList.Items.Count - 1)
                {
                    activePacksList.Items.Remove(pack);
                    activePacksList.Items.Insert(index + 1, pack);
                    activePacksList.SelectedItem = pack;
                }
            };

            applyButton.LeftMouseClick += (s, e) =>
            {
                asset.ApplyResourcePacks(activePacksList.Items);
                Program.Restart();
            };

            // List loaded resource packs

            AssetComponent assets = asset;
            foreach (var item in assets.LoadedResourcePacks)
                loadedPacksList.Items.Add(item);

            foreach (var item in asset.ActiveResourcePacks)
            {
                activePacksList.Items.Add(item);
                if (loadedPacksList.Items.Contains(item))
                    loadedPacksList.Items.Remove(item);
            }
        }

        private Control ListTemplateGenerator(ResourcePack? pack)
        {
            Debug.Assert(pack != null, nameof(pack) + " != null");
            return new Label()
            {
                Text = pack.Name,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalTextAlignment = HorizontalAlignment.Left
            };
        }

        private void loadedList_SelectedItemChanged(Control control, SelectionEventArgs<ResourcePack> e)
        {
            e.Handled = true;
            addButton.Visible = e.NewItem != null;

            if (e.NewItem != null)
            {
                activePacksList.SelectedItem = null;
                SetPackInfo(e.NewItem);
            }
            else
            {
                if (activePacksList.SelectedItem == null)
                    SetPackInfo(null);
            }
        }

        private void activeList_SelectedItemChanged(Control control, SelectionEventArgs<ResourcePack> e)
        {
            e.Handled = true;
            removeButton.Visible = e.NewItem != null;
            moveUpButton.Visible = e.NewItem != null;
            moveDownButton.Visible = e.NewItem != null;

            if (e.NewItem != null)
            {
                loadedPacksList.SelectedItem = null;
                SetPackInfo(e.NewItem);
            }
            else
            {
                if (loadedPacksList.SelectedItem == null)
                    SetPackInfo(null);
            }
        }

        private void SetPackInfo(ResourcePack? pack)
        {
            if (pack != null)
                infoLabel.Text = $"{pack.Name} ({pack.Version})\r\n{pack.Author}\r\n{pack.Description}";
            else
                infoLabel.Text = string.Empty;
        }
    }
}
