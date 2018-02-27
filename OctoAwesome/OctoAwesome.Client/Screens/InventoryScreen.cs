using MonoGameUi;
using OctoAwesome.Client.Components;
using engenious.Graphics;
using engenious.Input;
using System.Collections.Generic;
using OctoAwesome.Client.Controls;
using engenious;
using System;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : Screen
    {
        private AssetComponent assets;

        private Combobox<Control> comboboxLeft;
        private Panel panelleft;

        private Combobox<Control> comboboxRight;
        private Panel panelright;        

        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            assets = manager.Game.Assets;

            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.3f);

            Texture2D panelBackground = assets.LoadTexture(typeof(ScreenComponent), "panel");

            Grid grid = new Grid(manager);

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 40, });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            grid.Padding = Border.All(30);

            Controls.Add(grid);

            comboboxLeft = new Combobox<Control>(manager)
            {
                Height = 30,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 15),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                TemplateGenerator = s => new Label(manager) { Text = s.ToString(), HorizontalAlignment = HorizontalAlignment.Stretch },
                Margin = new Border(0, 0, 10, 0),
            };
            comboboxLeft.SelectedItemChanged += (s, e) => HandleItemChanged(s, e, 0);
            grid.AddControl(comboboxLeft, 0, 0);

            panelleft = new Panel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Border(0, 5, 10, 5),
            };
            grid.AddControl(panelleft, 0, 1);

            comboboxRight = new Combobox<Control>(manager)
            {
                Height = 30,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 15),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                TemplateGenerator = s => new Label(manager) { Text = s.ToString(), HorizontalAlignment = HorizontalAlignment.Stretch },
                Margin = new Border(10, 0, 0, 0),
            };
            comboboxRight.SelectedItemChanged += (s, e) => HandleItemChanged(s, e, 1);
            grid.AddControl(comboboxRight, 1, 0);

            panelright = new Panel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Border(10, 5, 0, 5),
            };
            grid.AddControl(panelright, 1, 1);

            foreach (Func<Control> creator in manager.InventoryScreenExtension)
            {
                Control con = creator.Invoke();
                if (con != null)
                {
                    comboboxLeft.Items.Add(con);
                    comboboxRight.Items.Add(con);
                }
            }
            if (comboboxLeft.Items.Count > 0)
                comboboxLeft.SelectedItem = comboboxLeft.Items[0];
            if (comboboxRight.Items.Count > 1)
                comboboxRight.SelectedItem = comboboxRight.Items[1];

            Title = Languages.OctoClient.Inventory;
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
        
        private void HandleItemChanged(Control sender, SelectionEventArgs<Control> args, int i)
        {
            if (i == 0) // left
            {
                panelleft.Controls.Clear();
                if (!Equals(comboboxRight.SelectedItem, args.NewItem))
                {
                    panelleft.Controls.Add(args.NewItem);
                }
            }
            else if(i == 1) // right
            {
                panelright.Controls.Clear();
                if (!Equals(comboboxLeft.SelectedItem, args.NewItem))
                {
                    panelright.Controls.Add(args.NewItem);
                }
            }
            args.Handled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
