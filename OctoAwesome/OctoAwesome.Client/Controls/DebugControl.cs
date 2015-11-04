using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using System.Collections.Generic;
using OctoAwesome.Runtime;
using OctoAwesome.Client.Components;
using System;

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

        ResourceManager resMan;

        public PlayerComponent Player { get; set; }

        private Trigger<bool> debugTrigger = new Trigger<bool>();

        StackPanel leftView, rightView;
        Label devText, position, rotation, fps, box, controlInfo, loadedChunks, activeTool, loadedInfo, flyInfo;

        public DebugControl(ScreenComponent screenManager)
            : base(screenManager)
        {
            framebuffer = new float[buffersize];
            Player = screenManager.Player;

            //Get ResourceManager for further Information later...
            resMan = ResourceManager.Instance;

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
            devText.Text = "Developement Version";
            leftView.Controls.Add(devText);

            loadedChunks = new Label(ScreenManager);
            leftView.Controls.Add(loadedChunks);

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

            activeTool = new Label(ScreenManager);
            rightView.Controls.Add(activeTool);

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
            if (!Visible || !Enabled)
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
            controlInfo.Text = "Active Controls: " + ScreenManager.ActiveScreen.Controls.Count;

            //Draw Position
            string pos = "pos: " + Player.ActorHost.Position.ToString();
            position.Text = pos;

            //Draw Rotation
            float grad = (Player.ActorHost.Angle / MathHelper.TwoPi) * 360;
            string rot = "rot: " +
                (((Player.ActorHost.Angle / MathHelper.TwoPi) * 360) % 360).ToString("0.00") + " / " +
                ((Player.ActorHost.Tilt / MathHelper.TwoPi) * 360).ToString("0.00");
            rotation.Text = rot;

            //Draw Fps
            string fpsString = "fps: " + (1f / lastfps).ToString("0.00");
            fps.Text = fpsString;

            //Draw Loaded Chunks
            loadedChunks.Text = "Loaded Chunks: " + resMan.GlobalChunkCache.LoadedChunks;

            //Get Number of Loaded Items/Blocks
            loadedInfo.Text = "" + (DefinitionManager.GetItemDefinitions() as IList<IItemDefinition>).Count + " Items - " +
                (DefinitionManager.GetBlockDefinitions() as IList<IItemDefinition>).Count + " Blocks";

            //Additional Play Information

            //Active Tool
            if (Player.ActorHost.ActiveTool != null)
                activeTool.Text = "Active Item/Tool: " + Player.ActorHost.ActiveTool.Definition.Name;

                //Fly Info
                if (Player.ActorHost.Player.FlyMode) flyInfo.Text = "Flymode enabled";
                else flyInfo.Text = "";

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
