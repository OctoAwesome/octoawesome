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
    internal class LoadScreen : BaseScreen
    {
        private new ScreenComponent Manager;

        Button renameButton, deleteButton, createButton, playButton;
        StackPanel buttonStack;
        Grid mainStack;
        Label nameLabel, seedLabel, lastPlayedLabel, planetsLabel, sizeLabel;

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

            EnableBackButton();

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
            levelList.SelectedItemBrush = new BorderBrush(Color.White);
            levelList.TemplateGenerator += (x) =>
            {
                StackPanel stack = new StackPanel(manager);
                stack.Padding = Border.All(10);
                stack.HorizontalAlignment = HorizontalAlignment.Stretch;
                stack.Controls.Add(
                    new Label(manager)
                    {
                        Text = x.Name,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Padding = Border.All(5),
                    });
                stack.Controls.Add(
                    new Label(manager)
                    {
                        Text = "Last played: " + "00.00.0000",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Padding = Border.All(5),
                    });

                return stack;
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
            Grid infoGrid = new Grid(manager);
            infoGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            infoGrid.VerticalAlignment = VerticalAlignment.Top;
            infoGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Auto });
            infoGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Auto });
            sidebar.Controls.Add(infoGrid);

            nameLabel = new Label(manager);
            seedLabel = new Label(manager);
            lastPlayedLabel = new Label(manager);
            planetsLabel = new Label(manager);
            sizeLabel = new Label(manager);

            AddInfoToGrid(infoGrid, Languages.OctoClient.Name + ": ", nameLabel);
            AddInfoToGrid(infoGrid, Languages.OctoClient.Seed + ": ", seedLabel);
            AddInfoToGrid(infoGrid, Languages.OctoClient.LastPlayed + ": ", lastPlayedLabel);
            AddInfoToGrid(infoGrid, Languages.OctoClient.Planets + ": ", planetsLabel);
            AddInfoToGrid(infoGrid, Languages.OctoClient.Size + ": ", sizeLabel);

            //Buttons
            StackPanel buttonStack = new StackPanel(manager);
            buttonStack.VerticalAlignment = VerticalAlignment.Bottom;
            buttonStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            sidebar.Controls.Add(buttonStack);

            //renameButton = getButton("Rename");
            //buttonStack.Controls.Add(renameButton);

            deleteButton = getButton(Languages.OctoClient.Delete);
            buttonStack.Controls.Add(deleteButton);
            deleteButton.LeftMouseClick += (s, e) =>
            {
                if (levelList.SelectedItem == null)
                    return;

                // Sicherstellen, dass universe nicht geladen ist
                if (ResourceManager.Instance.CurrentUniverse != null && 
                    ResourceManager.Instance.CurrentUniverse.Id == levelList.SelectedItem.Id)
                    return;

                ResourceManager.Instance.DeleteUniverse(levelList.SelectedItem.Id);
                levelList.Items.Remove(levelList.SelectedItem);
                levelList.SelectedItem = null;
                levelList.InvalidateDimensions();
            };

            createButton = getButton(Languages.OctoClient.Create);
            createButton.LeftMouseClick += (s, e) => manager.NavigateToScreen(new CreateUniverseScreen(manager));
            buttonStack.Controls.Add(createButton);

            playButton = getButton(Languages.OctoClient.Play);
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

        private void AddInfoToGrid(Grid grid, String title, Control control)
        {
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto });
            grid.AddControl(new Label(Manager) { Text = title , HorizontalAlignment = HorizontalAlignment.Left}, 0, grid.Rows.Count - 1);
            grid.AddControl(control, 1, grid.Rows.Count - 1);
            grid.Rows.Add(new RowDefinition() { Height = 10, ResizeMode = ResizeMode.Fixed });              
        }
    }
}
