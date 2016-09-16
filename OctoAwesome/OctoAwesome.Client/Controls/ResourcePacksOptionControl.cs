using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Controls
{
    internal sealed class ResourcePacksOptionControl : Panel
    {
        public ResourcePacksOptionControl(ScreenComponent manager) : base(manager)
        {
            Grid grid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = Border.All(15),
            };
            Controls.Add(grid);

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Auto, Width = 1 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });

            StackPanel buttons = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Top,
            };
            grid.AddControl(buttons, 1, 0);

            Button addButton = Button.TextButton(manager, "Add");
            addButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            addButton.Visible = false;
            addButton.LeftMouseClick += (s, e) =>
            {

            };
            buttons.Controls.Add(addButton);

            Button removeButton = Button.TextButton(manager, "Remove");
            removeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            removeButton.Visible = false;
            buttons.Controls.Add(removeButton);

            Button moveUpButton = Button.TextButton(manager, "Up");
            moveUpButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            moveUpButton.Visible = false;
            buttons.Controls.Add(moveUpButton);

            Button moveDownButton = Button.TextButton(manager, "Down");
            moveDownButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            moveDownButton.Visible = false;
            buttons.Controls.Add(moveDownButton);

            Button applyButton = Button.TextButton(manager, "Apply");
            applyButton.HorizontalAlignment = HorizontalAlignment.Right;
            grid.AddControl(applyButton, 0, 1, 3);

            Listbox<ResourcePack> loadedPacksList = new Listbox<ResourcePack>(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
            };

            loadedPacksList.SelectedItemChanged += (s, e) =>
            {
                e.Handled = true;
                addButton.Visible = e.NewItem != null;
            };

            loadedPacksList.TemplateGenerator = (item) =>
            {
                return new Label(manager) { Text = item.Name, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalTextAlignment = HorizontalAlignment.Left };
            };
            grid.AddControl(loadedPacksList, 0, 0);

            Listbox<ResourcePack> activePacksList = new Listbox<ResourcePack>(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
            };
            activePacksList.TemplateGenerator = (item) =>
            {
                return new Label(manager) { Text = item.Name, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalTextAlignment = HorizontalAlignment.Left };

            };
            activePacksList.SelectedItemChanged += (s, e) =>
            {
                e.Handled = true;
                removeButton.Visible = e.NewItem != null;
                moveUpButton.Visible = e.NewItem != null;
                moveDownButton.Visible = e.NewItem != null;
            };
            grid.AddControl(activePacksList, 2, 0);

            AssetComponent assets = manager.Game.Assets;
            foreach (var item in assets.LoadedResourcePacks)
                loadedPacksList.Items.Add(item);

            foreach (var item in manager.Game.Assets.ActiveResourcePacks)
            {
                activePacksList.Items.Add(item);
                if (loadedPacksList.Items.Contains(item))
                    loadedPacksList.Items.Remove(item);
            }

            addButton.LeftMouseClick += (s, e) =>
            {
                ResourcePack pack = loadedPacksList.SelectedItem;
                loadedPacksList.Items.Remove(pack);
                activePacksList.Items.Add(pack);
            };

            removeButton.LeftMouseClick += (s, e) =>
            {
                ResourcePack pack = activePacksList.SelectedItem;
                activePacksList.Items.Remove(pack);
                loadedPacksList.Items.Add(pack);
            };

            applyButton.LeftMouseClick += (s, e) =>
            {
                manager.Game.Assets.ApplyResourcePacks(activePacksList.Items);
                Program.Restart();
            };
        }
    }
}
