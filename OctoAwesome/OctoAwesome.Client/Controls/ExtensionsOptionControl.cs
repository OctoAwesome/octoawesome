using engenious;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;

namespace OctoAwesome.Client.Controls
{
    internal sealed class ExtensionsOptionControl : Panel
    {
        private Button enableButton;
        private Button disableButton;
        private Button applyButton;
        private Listbox<IExtension> loadedPacksList;
        private Listbox<IExtension> activePacksList;
        private Label infoLabel;

        public ExtensionsOptionControl(ScreenComponent manager) : base(manager)
        {
            Grid grid = new Grid(manager)
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

            StackPanel buttons = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            grid.AddControl(buttons, 1, 0);

            #region Manipulationsbuttons

            enableButton = Button.TextButton(manager, "Enable");
            enableButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            enableButton.Visible = false;
            buttons.Controls.Add(enableButton);

            disableButton = Button.TextButton(manager, "Disable");
            disableButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            disableButton.Visible = false;
            buttons.Controls.Add(disableButton);

            #endregion

            applyButton = Button.TextButton(manager, "Apply");
            applyButton.HorizontalAlignment = HorizontalAlignment.Right;
            applyButton.VerticalAlignment = VerticalAlignment.Bottom;
            grid.AddControl(applyButton, 0, 2, 3);

            infoLabel = new Label(ScreenManager)
            {
                HorizontalTextAlignment = HorizontalAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                WordWrap = true,
            };
            grid.AddControl(infoLabel, 0, 1, 3);

            #region Listen

            loadedPacksList = new Listbox<IExtension>(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
                TemplateGenerator = ListTemplateGenerator,
            };

            grid.AddControl(loadedPacksList, 0, 0);

            activePacksList = new Listbox<IExtension>(manager)
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

            enableButton.LeftMouseClick += (s, e) =>
            {
                IExtension ext = loadedPacksList.SelectedItem;
                loadedPacksList.Items.Remove(ext);
                activePacksList.Items.Add(ext);
                activePacksList.SelectedItem = ext;
            };

            disableButton.LeftMouseClick += (s, e) =>
            {
                IExtension ext = activePacksList.SelectedItem;
                activePacksList.Items.Remove(ext);
                loadedPacksList.Items.Add(ext);
                loadedPacksList.SelectedItem = ext;
            };
            
            applyButton.LeftMouseClick += (s, e) =>
            {
                //TODO: Apply
                manager.Game.ExtensionLoader.ApplyExtensions(loadedPacksList.Items);
                Program.Restart();
            };

            // Daten laden
            var loader = manager.Game.ExtensionLoader;
            foreach (var item in loader.LoadedExtensions)
                loadedPacksList.Items.Add(item);

            foreach (var item in loader.ActiveExtensions)
            {
                activePacksList.Items.Add(item);
                if (loadedPacksList.Items.Contains(item))
                    loadedPacksList.Items.Remove(item);
            }
        }

        private Control ListTemplateGenerator(IExtension ext)
        {
            return new Label(ScreenManager)
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
                activePacksList.SelectedItem = null;
                SetPackInfo(e.NewItem);
            }
            else
            {
                if (activePacksList.SelectedItem == null)
                    SetPackInfo(null);
            }
        }

        private void activeList_SelectedItemChanged(Control control, SelectionEventArgs<IExtension> e)
        {
            e.Handled = true;
            disableButton.Visible = e.NewItem != null;

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

        private void SetPackInfo(IExtension ext)
        {
            if (ext != null)
                infoLabel.Text = string.Format("{0}{1}{2}", ext.Name, Environment.NewLine, ext.Description);
            else
                infoLabel.Text = string.Empty;
        }
    }
}
