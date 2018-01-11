using MonoGameUi;
using System.Collections.Generic;
using OctoAwesome.Runtime;
using OctoAwesome.Client.Components;
using System;
using engenious;
using engenious.Graphics;
using System.Linq;
using engenious.Helper;

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

        StackPanel leftView, rightView;
        Label devText, position, rotation, fps, box, controlInfo, loadedChunks, loadedTextures, activeTool, toolCount, loadedInfo, flyInfo, temperatureInfo, precipitationInfo;

        public DebugControl(ScreenComponent screenManager)
            : base(screenManager)
        {
            framebuffer = new float[buffersize];
            Player = screenManager.Player;
            manager = screenManager;
            assets = screenManager.Game.Assets;

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

            //Creating all Labels
            devText = new Label(ScreenManager);
            devText.Text = Languages.OctoClient.DevelopmentVersion;
            leftView.Controls.Add(devText);

            loadedChunks = new Label(ScreenManager);
            leftView.Controls.Add(loadedChunks);

            loadedTextures = new Label(ScreenManager);
            leftView.Controls.Add(loadedTextures);

            loadedInfo = new Label(ScreenManager);
            leftView.Controls.Add(loadedInfo);

            position = new Label(ScreenManager);
            rightView.Controls.Add(position);

            rotation = new Label(ScreenManager);
            rightView.Controls.Add(rotation);

            fps = new Label(ScreenManager);
            rightView.Controls.Add(fps);

            controlInfo = new Label(ScreenManager);
            leftView.Controls.Add(controlInfo);
          
            temperatureInfo = new Label(ScreenManager);
            rightView.Controls.Add(temperatureInfo);

            precipitationInfo = new Label(ScreenManager);
            rightView.Controls.Add(precipitationInfo);

            activeTool = new Label(ScreenManager);
            rightView.Controls.Add(activeTool);

            toolCount = new Label(ScreenManager);
            rightView.Controls.Add(toolCount);

            flyInfo = new Label(ScreenManager);
            rightView.Controls.Add(flyInfo);

            //This Label gets added to the root and is set to Bottom Left
            box = new Label(ScreenManager);
            box.VerticalAlignment = VerticalAlignment.Bottom;
            box.HorizontalAlignment = HorizontalAlignment.Left;
            box.TextColor = Color.White;
            Controls.Add(box);

            //Add the left & right side to the root
            Controls.Add(leftView);
            Controls.Add(rightView);

            //Label Setup - Set Settings for all Labels in one place
            foreach (Control control in leftView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Left;
                if (control is Label)
                {
                    ((Label)control).TextColor = Color.White;
                }
            }
            foreach (Control control in rightView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Right;
                if (control is Label)
                {
                    ((Label)control).TextColor = Color.White;

                }
            }
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

            framebuffer[bufferindex++] = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bufferindex %= buffersize;

            //Draw Control Info
            controlInfo.Text = Languages.OctoClient.ActiveControls + ": " + ScreenManager.ActiveScreen.Controls.Count;

            //Draw Position
            string pos = "pos: " + Player.Position.Position.ToString();
            position.Text = pos;

            //Draw Rotation
            float grad = (Player.CurrentEntityHead.Angle / MathHelper.TwoPi) * 360;
            string rot = "rot: " +
                (((Player.CurrentEntityHead.Angle / MathHelper.TwoPi) * 360) % 360).ToString("0.00") + " / " +
                ((Player.CurrentEntityHead.Tilt / MathHelper.TwoPi) * 360).ToString("0.00");
            rotation.Text = rot;

            //Draw Fps
            string fpsString = "fps: " + (1f / lastfps).ToString("0.00");
            fps.Text = fpsString;

            //Draw Loaded Chunks
            loadedChunks.Text = string.Format("{0}: {1}/{2}", 
                Languages.OctoClient.LoadedChunks, 
                manager.Game.ResourceManager.GlobalChunkCache.DirtyChunkColumn,
                manager.Game.ResourceManager.GlobalChunkCache.LoadedChunkColumns);

            // Draw Loaded Textures
            loadedTextures.Text = string.Format("Loaded Textures: {0}",
                assets.LoadedTextures);

            //Get Number of Loaded Items/Blocks
            loadedInfo.Text = "" + manager.Game.DefinitionManager.GetItemDefinitions().Count() + " " + Languages.OctoClient.Items + " - " +
                manager.Game.DefinitionManager.GetBlockDefinitions().Count() + " " + Languages.OctoClient.Blocks;

            //Additional Play Information

            ////Active Tool
            //if (Player.ActorHost.ActiveTool != null)
            //    activeTool.Text = Languages.OctoClient.ActiveItemTool + ": " + Player.ActorHost.ActiveTool.Definition.Name + " | " + Array.FindIndex(Player.ActorHost.Player.Tools, (i => i.Definition == Player.ActorHost.ActiveTool.Definition));

            // toolCount.Text = Languages.OctoClient.ToolCount + ": " + Player.ActorHost.Player.Tools.Length;

            ////Fly Info
            //if (Player.ActorHost.Player.FlyMode) flyInfo.Text = Languages.OctoClient.FlymodeEnabled;
            //else flyInfo.Text = "";

            IPlanet planet = manager.Game.ResourceManager.GetPlanet(Player.Position.Position.Planet);
            // Temperature Info
            temperatureInfo.Text = Languages.OctoClient.Temperature + ": " + planet.ClimateMap.GetTemperature(Player.Position.Position.GlobalBlockIndex);

            // Precipitation Info
            precipitationInfo.Text = "Precipitation: " + planet.ClimateMap.GetPrecipitation(Player.Position.Position.GlobalBlockIndex);

            //Draw Box Information
            if (Player.SelectedBox.HasValue)
            {
                string selection = "box: " +
                    Player.SelectedBox.Value.ToString() + " on " +
                    Player.SelectedSide.ToString() + " (" +
                    Player.SelectedPoint.Value.X.ToString("0.00") + "/" +
                    Player.SelectedPoint.Value.Y.ToString("0.00") + ") -> " +
                    Player.SelectedEdge.ToString() + " -> " + Player.SelectedCorner.ToString();
                box.Text = selection;
            }
            else
                box.Text = "";

        }
    }
}
