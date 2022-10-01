﻿using engenious;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Runtime;

using System;
using System.Diagnostics;


namespace OctoAwesome.Client.Controls
{
    internal sealed class ExtensionsOptionControl : Panel
    {
        private readonly Button enableButton;
        private readonly Button disableButton;
        private readonly Button applyButton;
        private readonly Listbox<IExtension> loadedExtensionsList;
        private readonly Listbox<IExtension> activeExtensionsList;
        private readonly Label infoLabel;

        public ExtensionsOptionControl(ExtensionLoader extensionLoader)
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

            enableButton = new TextButton(UI.Languages.OctoClient.Enable);
            enableButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            enableButton.Visible = false;
            buttons.Controls.Add(enableButton);

            disableButton = new TextButton(UI.Languages.OctoClient.Disable);
            disableButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            disableButton.Visible = false;
            buttons.Controls.Add(disableButton);

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

            loadedExtensionsList = new Listbox<IExtension>()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
                TemplateGenerator = ListTemplateGenerator,
            };

            grid.AddControl(loadedExtensionsList, 0, 0);

            activeExtensionsList = new Listbox<IExtension>()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
                TemplateGenerator = ListTemplateGenerator,
            };

            grid.AddControl(activeExtensionsList, 2, 0);

            #endregion

            loadedExtensionsList.SelectedItemChanged += loadedList_SelectedItemChanged;
            activeExtensionsList.SelectedItemChanged += activeList_SelectedItemChanged;

            enableButton.LeftMouseClick += (s, e) =>
            {
                var ext = loadedExtensionsList.SelectedItem;
                if (ext == null)
                    throw new NullReferenceException("Enable button should not be clickable when no item is selected.");

                loadedExtensionsList.Items.Remove(ext);
                activeExtensionsList.Items.Add(ext);
                activeExtensionsList.SelectedItem = ext;
            };

            disableButton.LeftMouseClick += (s, e) =>
            {
                var ext = activeExtensionsList.SelectedItem;
                if (ext == null)
                    throw new NullReferenceException("Disable button should not be clickable when no item is selected.");

                activeExtensionsList.Items.Remove(ext);
                loadedExtensionsList.Items.Add(ext);
                loadedExtensionsList.SelectedItem = ext;
            };

            applyButton.LeftMouseClick += (s, e) =>
            {
                extensionLoader.Apply(loadedExtensionsList.Items);
                Program.Restart();
            };

            // List loaded extensions
            var loader = extensionLoader;
            foreach (var item in loader.LoadedExtensions)
                loadedExtensionsList.Items.Add(item);

            foreach (var item in loader.ActiveExtensions)
            {
                activeExtensionsList.Items.Add(item);
                if (loadedExtensionsList.Items.Contains(item))
                    loadedExtensionsList.Items.Remove(item);
            }
        }

        private Control ListTemplateGenerator(IExtension? ext)
        {
            Debug.Assert(ext != null, nameof(ext) + " != null");
            return new Label(Style, ScreenManager)
            {
                Text = ext.Name,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalTextAlignment = HorizontalAlignment.Left
            };
        }

        private void loadedList_SelectedItemChanged(Control control, SelectionEventArgs<IExtension> e)
        {
            e.Handled = true;
            enableButton.Visible = e.NewItem != null;

            if (e.NewItem != null)
            {
                activeExtensionsList.SelectedItem = null;
                SetPackInfo(e.NewItem);
            }
            else
            {
                if (activeExtensionsList.SelectedItem == null)
                    SetPackInfo(null);
            }
        }

        private void activeList_SelectedItemChanged(Control control, SelectionEventArgs<IExtension> e)
        {
            e.Handled = true;
            disableButton.Visible = e.NewItem != null;

            if (e.NewItem != null)
            {
                loadedExtensionsList.SelectedItem = null;
                SetPackInfo(e.NewItem);
            }
            else
            {
                if (loadedExtensionsList.SelectedItem == null)
                    SetPackInfo(null);
            }
        }

        private void SetPackInfo(IExtension? ext)
        {
            if (ext != null)
                infoLabel.Text = $"{ext.Name}{Environment.NewLine}{ext.Description}";
            else
                infoLabel.Text = string.Empty;
        }
    }
}
