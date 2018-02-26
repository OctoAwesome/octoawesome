using MonoGameUi;
using System.Collections.Generic;
using engenious;
using engenious.Graphics;
using OctoAwesome.Entities;
using System;
using OctoAwesome.Basics.EntityComponents;

namespace OctoAwesome.Basics.Controls
{
    internal class ToolbarControl : Panel
    {
        private Button[] buttons;

        private Image[] images;

        private Brush buttonBackgroud;

        private Brush activeBackground;

        private ToolBarComponent toolbar;

        public Label activeToolLabel;

        private Dictionary<string, Texture2D> toolTextures;

        public ToolbarControl(BaseScreenComponent screenManager, IUserInterfaceManager manager, ToolBarComponent bar) : 
            base(screenManager)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Bottom;
            Height = 100;
            toolbar = bar;
            buttonBackgroud = new BorderBrush(Color.Black);
            activeBackground = new BorderBrush(Color.Red);
            buttons = new Button[toolbar.Tools.Length];
            images = new Image[toolbar.Tools.Length];

            toolTextures = new Dictionary<string, Texture2D>();

            foreach (var item in bar.Service.DefinitionManager.GetDefinitions())
            {
                Texture2D texture = manager.LoadTextures(item.GetType(), item.Icon);
                toolTextures.Add(item.GetType().FullName, texture);
            }

            Grid grid = new Grid(screenManager)
            {
                Margin = new Border(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Controls.Add(grid);

            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });
            
            for (int i = 0; i < toolbar.Tools.Length; i++)
            {
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            }

            activeToolLabel = new Label(screenManager);
            activeToolLabel.VerticalAlignment = VerticalAlignment.Top;
            activeToolLabel.HorizontalAlignment = HorizontalAlignment.Center;
            activeToolLabel.Background = new BorderBrush(Color.Black * 0.3f);
            activeToolLabel.TextColor = Color.White;
            grid.AddControl(activeToolLabel, 0, 0, 10);

            for (int i = 0; i < toolbar.Tools.Length; i++)
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
            if(!(toolbar.Entity is IControllable con) || con.Controller == null)
                return;
            #region Toolbar Update
            //TODO: validate - source playercomponent, vllt in die toolbarComponent
            if (toolbar.Tools != null && toolbar.Tools.Length > 0)
            {
                if (toolbar.ActiveTool == null) toolbar.ActiveTool = toolbar.Tools[0];
                for (int i = 0; i < Math.Min(toolbar.Tools.Length, con.Controller.SlotInput.Length); i++)
                {
                    if (con.Controller.SlotInput[i]) toolbar.ActiveTool = toolbar.Tools[i];
                    con.Controller.SlotInput[i] = false;
                }
            }

            //Index des aktiven Werkzeugs ermitteln
            int activeTool = -1;
            List<int> toolIndices = new List<int>();
            for (int i = 0; i < toolbar.Tools.Length; i++)
            {
                //TODO: validate - source blockinteractioncomponent
                if (toolbar.ActiveTool != null && toolbar.ActiveTool.Amount <= 0)
                    toolbar.RemoveSlot(toolbar.ActiveTool);

                if (toolbar.Tools[i] != null)
                    toolIndices.Add(i);

                if (toolbar.Tools[i] == toolbar.ActiveTool)
                    activeTool = toolIndices.Count - 1;
            }

            if (con.Controller.SlotLeftInput)
            {
                if (activeTool > -1)
                    activeTool--;
                else if (toolIndices.Count > 0)
                    activeTool = toolIndices[toolIndices.Count - 1];
            }
            con.Controller.SlotLeftInput = false;

            if (con.Controller.SlotRightInput)
            {
                if (activeTool > -1)
                    activeTool++;
                else if (toolIndices.Count > 0)
                    activeTool = toolIndices[0];
            }
            con.Controller.SlotRightInput = false;

            if (activeTool > -1)
            {
                activeTool = (activeTool + toolIndices.Count) % toolIndices.Count;
                toolbar.ActiveTool = toolbar.Tools[toolIndices[activeTool]];
            }
            #endregion
            // Aktualisierung des aktiven Buttons
            for (int i = 0; i < toolbar.Tools.Length; i++)
            {
                if (toolbar.Tools != null &&
                    toolbar.Tools.Length > i &&
                    toolbar.Tools[i] != null &&
                    toolbar.Tools[i].Definition != null)
                {
                    images[i].Texture = toolTextures[toolbar.Tools[i].Definition.GetType().FullName];
                    if (toolbar.ActiveTool == toolbar.Tools[i])
                        buttons[i].Background = activeBackground;
                    else
                        buttons[i].Background = buttonBackgroud;
                }
                else
                {
                    images[i].Texture = null;
                    buttons[i].Background = buttonBackgroud;
                }
            }
            
            //Aktualisierung des ActiveTool Labels
            activeToolLabel.Text = toolbar.ActiveTool != null ?
                string.Format("{0} ({1})", toolbar.ActiveTool.Definition.Name, toolbar.ActiveTool.Amount) :
                string.Empty;

            activeToolLabel.Visible = !(activeToolLabel.Text == string.Empty);

            base.OnUpdate(gameTime);
        }
    }
}
