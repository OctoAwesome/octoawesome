using MonoGameUi;
using OctoAwesome.Client.Components;
using engenious;
using engenious.Graphics;
using System.Linq;
using engenious.Helper;
using System;
using System.Collections.Generic;
using OctoAwesome.Entities;

namespace OctoAwesome.Client.Controls
{
    internal class DebugControl : Panel
    {
        private int buffersize = 10;
        private float[] framebuffer;
        private int bufferindex = 0;

        private int framecount = 0;
        private double seconds = 0;
        private double lastfps = 0f;

        AssetComponent assets;

        public PlayerComponent Player { get; set; }

        private readonly ScreenComponent manager;

        private StackPanel leftView;
        private StackPanel rightView;
        private List<Action> updatefunctions;
        private Label loadedChunks;
        private Label loadedTextures;
        private Label loadedInfo;
        private Label controlInfo;
        private Label position;
        private Label rotation;
        private Label fps;
        private Label temperatureInfo;
        private Label precipitationInfo;
        private Label gravityInfo;
        private Label activeTool;
        private Label toolCount;
        private Label flyInfo;
        private Control devText;
        private Label box;

        public DebugControl(ScreenComponent screenManager) : base(screenManager)
        {
            framebuffer = new float[buffersize];
            Player = screenManager.Player;
            manager = screenManager;
            assets = screenManager.Game.Assets;
            updatefunctions = new List<Action>();

            //Brush for Debug Background
            BorderBrush bg = new BorderBrush(Color.Black * 0.2f);

            //The left side of the Screen
            leftView = new StackPanel(ScreenManager)
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };

            //The right Side of the Screen
            rightView = new StackPanel(ScreenManager)
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };

            //Register(false, () => $"fps: {(1f / lastfps).ToString("0.00")}");
            //Register(false, () => string.Format("{0}: {1}/{2}", Languages.OctoClient.LoadedChunks,
            //    manager.Game.ResourceManager.GlobalChunkCache.DirtyChunkColumn,
            //    manager.Game.ResourceManager.GlobalChunkCache.LoadedChunkColumns));
            //Register(false, () => $"Loaded Textures: {assets.LoadedTextures}");
            //Register(false, () => string.Format("{0} {1} - {2} {3}", manager.Game.DefinitionManager.GetItemDefinitions().Count(),
            //    Languages.OctoClient.Items, manager.Game.DefinitionManager.GetBlockDefinitions().Count(), Languages.OctoClient.Blocks));
            //Register(false, () => $"{Languages.OctoClient.ActiveControls}: {ScreenManager.ActiveScreen.Controls.Count}");
            
            //Register(true, () => screenManager.Player.CurrentEntity != null ? $"pos: {screenManager.Player.CurrentEntity.Position.ToString()}" : "");
            //Register(true, () =>
            //    $"rot: {(((screenManager.Player.Yaw / MathHelper.TwoPi) * 360) % 360).ToString("0.00")} / {((screenManager.Player.Tilt / MathHelper.TwoPi) * 360).ToString("0.00")}");
            //Register(true, () => screenManager.Player.CurrentEntity != null ?
            //    $"{Languages.OctoClient.Temperature}: {screenManager.Player.CurrentEntity.Cache.Planet.ClimateMap.GetTemperature(screenManager.Player.CurrentEntity.Position.GlobalBlockIndex)}" : "");
            //Register(true, () => screenManager.Player.CurrentEntity != null ?
            //    $"Precipitation:  {screenManager.Player.CurrentEntity.Cache.Planet.ClimateMap.GetPrecipitation(screenManager.Player.CurrentEntity.Position.GlobalBlockIndex)}" : "");
            //Register(true, () => screenManager.Player.CurrentEntity != null ? $"Gravity: {screenManager.Player.CurrentEntity.Cache.Planet.Gravity}" : "");

            //Creating all Labels
            devText = new Label(ScreenManager)
            {
                Text = Languages.OctoClient.DevelopmentVersion
            };
            leftView.Controls.Add(devText);

            loadedChunks = new Label(ScreenManager);
            leftView.Controls.Add(loadedChunks);

            loadedTextures = new Label(ScreenManager);
            leftView.Controls.Add(loadedTextures);

            loadedInfo = new Label(ScreenManager);
            leftView.Controls.Add(loadedInfo);

            controlInfo = new Label(ScreenManager);
            leftView.Controls.Add(controlInfo);

            position = new Label(ScreenManager);
            rightView.Controls.Add(position);

            rotation = new Label(ScreenManager);
            rightView.Controls.Add(rotation);

            fps = new Label(ScreenManager);
            rightView.Controls.Add(fps);

            temperatureInfo = new Label(ScreenManager);
            rightView.Controls.Add(temperatureInfo);

            precipitationInfo = new Label(ScreenManager);
            rightView.Controls.Add(precipitationInfo);

            gravityInfo = new Label(ScreenManager);
            rightView.Controls.Add(gravityInfo);

            activeTool = new Label(ScreenManager);
            rightView.Controls.Add(activeTool);

            toolCount = new Label(ScreenManager);
            rightView.Controls.Add(toolCount);

            flyInfo = new Label(ScreenManager);
            rightView.Controls.Add(flyInfo);

            //This Label gets added to the root and is set to Bottom Left
            box = new Label(ScreenManager)
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextColor = Color.White
            };
            Controls.Add(box);

            //Add the left & right side to the root
            Controls.Add(leftView);
            Controls.Add(rightView);

            foreach ((bool right, Func<string> updatefunc) in screenManager.DebugControlExtensions)
                Register(right, updatefunc);

            //Label Setup - Set Settings for all Labels in one place
            foreach (Control control in leftView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Left;
                if (control is Label label)
                {
                    label.TextColor = Color.White;
                }
            }
            foreach (Control control in rightView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Right;
                if (control is Label label)
                {
                    label.TextColor = Color.White;
                }
            }
        }

        public void Register(bool right, Func<string> updatefunction)
        {
            Label label = new Label(ScreenManager);
            if (right)
            {
                rightView.Controls.Add(label);
            }
            else
            {
                leftView.Controls.Add(label);
            }
            updatefunctions.Add(() => label.Text = updatefunction());
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!Visible || !Enabled || !assets.Ready)
                return;

            if (Player == null || Player.CurrentEntity == null)
                return;

            //Calculate FPS
            framecount++;
            seconds += gameTime.ElapsedGameTime.TotalSeconds;

            if (framecount == 10)
            {
                lastfps = seconds / framecount;
                framecount = 0;
                seconds = 0;
            }
            framebuffer[bufferindex++] = (float) gameTime.ElapsedGameTime.TotalSeconds;
            bufferindex %= buffersize;

            //Draw Control Info
            controlInfo.Text = Languages.OctoClient.ActiveControls + ": " + ScreenManager.ActiveScreen.Controls.Count;

            //Draw Position
            string pos = "pos: " + Player.CurrentEntity.Position.ToString();
            position.Text = pos;

            //Draw Rotation
            string rot = "rot: " +
                (((Player.Yaw / MathHelper.TwoPi) * 360) % 360).ToString("0.00") + " / " +
                ((Player.Tilt / MathHelper.TwoPi) * 360).ToString("0.00");
            rotation.Text = rot;

            //Draw Fps
            string fpsString = $"fps: {(1f / lastfps).ToString("0.00")}";
            fps.Text = fpsString;

            //Draw Loaded Chunks
            loadedChunks.Text = string.Format("{0}: {1}/{2}",
                Languages.OctoClient.LoadedChunks,
                manager.Game.ResourceManager.GlobalChunkCache.DirtyChunkColumn,
                manager.Game.ResourceManager.GlobalChunkCache.LoadedChunkColumns);

            // Draw Loaded Textures
            loadedTextures.Text = $"Loaded Textures: {assets.LoadedTextures}";

            //Get Number of Loaded Items/Blocks
            loadedInfo.Text = "" + manager.Game.DefinitionManager.GetItemDefinitions().Count() + " " + Languages.OctoClient.Items + " - " +
                manager.Game.DefinitionManager.GetBlockDefinitions().Count() + " " + Languages.OctoClient.Blocks;

            //Additional Play Information

            //Active Tool
            //if (Player.Toolbar.ActiveTool != null)
            //    activeTool.Text = Languages.OctoClient.ActiveItemTool + ": " + Player.Toolbar.ActiveTool.Definition.Name + " | " + Player.Toolbar.GetSlotIndex(Player.Toolbar.ActiveTool);

            //toolCount.Text = Languages.OctoClient.ToolCount + ": " + Player.Toolbar.Tools.Count(slot => slot != null);

            ////Fly Info
            //if (Player.ActorHost.Player.FlyMode) flyInfo.Text = Languages.OctoClient.FlymodeEnabled;
            //else flyInfo.Text = "";

            IPlanet planet = manager.Game.ResourceManager.GetPlanet(Player.CurrentEntity.Position.Planet);

            // Temperature Info
            temperatureInfo.Text = Languages.OctoClient.Temperature + ": " + planet.ClimateMap.GetTemperature(Player.CurrentEntity.Position.GlobalBlockIndex);

            // Precipitation Info
            precipitationInfo.Text = "Precipitation: " + planet.ClimateMap.GetPrecipitation(Player.CurrentEntity.Position.GlobalBlockIndex);

            // Gravity Info
            gravityInfo.Text = "Gravity" + ": " + planet.Gravity;

            //Draw Box Information
            if (Player.SelectedBlock.HasValue)
            {
                string selection = "box: " +
                    Player.SelectedBlock.Value.ToString() + " on " +
                    Player.SelectedSide.ToString() + " (" +
                    Player.SelectedPoint.Value.X.ToString("0.00") + "/" +
                    Player.SelectedPoint.Value.Y.ToString("0.00") + ") -> " +
                    Player.SelectedEdge.ToString() + " -> " + Player.SelectedCorner.ToString();
                box.Text = selection;
            }
            else
                box.Text = "";

            int indextoremove = -1;
            try
            {
                for (int i = 0; i < updatefunctions.Count; i++)
                {
                    indextoremove = i;
                    updatefunctions[i].Invoke();
                }
            }
            catch (Exception)
            {
                //TODO: loggen
                if (indextoremove < updatefunctions.Count && indextoremove >= 0)
                    updatefunctions.RemoveAt(indextoremove);
            }

        }
    }
}
