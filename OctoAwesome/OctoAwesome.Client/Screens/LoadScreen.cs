using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Components;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    internal class LoadScreen : Screen
    {
        private new ScreenComponent Manager;

        Button renameButton, deleteButton, createButton, playButton;
        StackPanel buttonStack;
        Grid mainStack;

        public LoadScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;

            Padding = new Border(0, 0, 0, 0);

            //Background
            Image background = new Image(manager);
            background.Texture = Manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_notext.png", Manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            Button backButton = Button.TextButton(manager, Languages.OctoClient.Back);
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);

            //Main Panel
            mainStack = new Grid(manager);
            mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 3 });
            mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainStack.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainStack.Margin = Border.All(50);
            mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainStack.VerticalAlignment = VerticalAlignment.Stretch;

            Controls.Add(mainStack);

            //Level Stack
            Listbox<IUniverse> levelList = new Listbox<IUniverse>(manager);
            levelList.Background = new BorderBrush(Color.White * 0.5f);
            levelList.VerticalAlignment = VerticalAlignment.Stretch;
            levelList.HorizontalAlignment = HorizontalAlignment.Stretch;
            levelList.Margin = Border.All(10);
            levelList.TemplateGenerator += (x) =>
            {
                return new Label(manager)
                {
                    Text = string.Format("{0} ({1})", x.Name, x.Seed),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Padding = Border.All(10),
                };
            };
            levelList.SelectedItemChanged += (s, e) =>
            {
                playButton.Enabled = e.NewItem != null;
                deleteButton.Enabled = e.NewItem != null;
            };
            mainStack.AddControl(levelList, 0, 0);


            //Sidebar
            Panel sidebar = new Panel(manager);
            sidebar.Padding = Border.All(20);
            sidebar.VerticalAlignment = VerticalAlignment.Stretch;
            sidebar.HorizontalAlignment = HorizontalAlignment.Stretch;
            sidebar.Background = new BorderBrush(Color.White * 0.5f);
            sidebar.Margin = Border.All(10);
            mainStack.AddControl(sidebar, 1, 0);

            //Universe Info
            Label l = new Label(manager);
            l.Text = " Placeholder ";
            l.VerticalAlignment = VerticalAlignment.Top;
            l.HorizontalAlignment = HorizontalAlignment.Left;
            sidebar.Controls.Add(l);

            //Buttons
            StackPanel buttonStack = new StackPanel(manager);
            buttonStack.VerticalAlignment = VerticalAlignment.Bottom;
            buttonStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            sidebar.Controls.Add(buttonStack);

            //renameButton = getButton("Rename");
            //buttonStack.Controls.Add(renameButton);

            deleteButton = getButton("Delete");
            buttonStack.Controls.Add(deleteButton);

            createButton = getButton("Create");
            createButton.LeftMouseClick += (s, e) => manager.NavigateToScreen(new CreateUniverseScreen(manager));
            buttonStack.Controls.Add(createButton);

            playButton = getButton("Play");
            playButton.LeftMouseClick += (s, e) =>
            {
                if (levelList.SelectedItem == null)
                    return;

                manager.Player.RemovePlayer();
                manager.Game.Simulation.LoadGame(levelList.SelectedItem.Id);
                manager.Game.Player.InsertPlayer();
                manager.NavigateToScreen(new GameScreen(manager));
            };
            buttonStack.Controls.Add(playButton);

            foreach (var universe in ResourceManager.Instance.ListUniverses())
                levelList.Items.Add(universe);
        }

        private Button getButton(string title)
        {
            Button button = Button.TextButton(Manager, title);
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            return button;
        }
    }
}
