using System;
using engenious;
using engenious.Graphics;
using engenious.Helper;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Definitions;
using System.Linq;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Client.Controls
{
    internal class DebugControl : Panel
    {
        private readonly int buffersize = 10;
        private readonly float[] framebuffer;
        private int bufferindex = 0;

        private int framecount = 0;
        private double seconds = 0;
        private double lastfps = 0f;
        private readonly AssetComponent assets;
        private readonly IResourceManager resourceManager;
        private readonly IDefinitionManager definitionManager;

        public PlayerComponent Player { get; }

        private readonly StackPanel leftView, rightView;
        private readonly Label devText, position, rotation, fps, box, controlInfo, targetedBlockName, targetedBlockPosition, loadedChunks, loadedTextures, activeTool, toolCount, loadedInfo, flyInfo, temperatureInfo, precipitationInfo, gravityInfo;

        public DebugControl(AssetComponent assets, PlayerComponent playerComponent, IResourceManager resourceManager,
            IDefinitionManager definitionManager)
        {
            framebuffer = new float[buffersize];
            Player = playerComponent ?? throw new ArgumentNullException(nameof(playerComponent));
            this.assets = assets;
            this.resourceManager = resourceManager;
            this.definitionManager = definitionManager;
            Background = new SolidColorBrush(Color.Transparent);

            //Brush for Debug Background
            var bg = new BorderBrush(Color.Black * 0.2f);
            //The left side of the Screen
            leftView = new StackPanel()
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };

            //The right Side of the Screen
            rightView = new StackPanel()
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };

            //Creating all Labels
            devText = new Label()
            {
                Text = UI.Languages.OctoClient.DevelopmentVersion
            };
            leftView.Controls.Add(devText);

            loadedChunks = new Label();
            leftView.Controls.Add(loadedChunks);

            loadedTextures = new Label();
            leftView.Controls.Add(loadedTextures);

            loadedInfo = new Label();
            leftView.Controls.Add(loadedInfo);

            position = new Label();
            rightView.Controls.Add(position);

            rotation = new Label();
            rightView.Controls.Add(rotation);

            fps = new Label();
            rightView.Controls.Add(fps);

            controlInfo = new Label();
            leftView.Controls.Add(controlInfo);

            leftView.Controls.Add(new Label()
            {
                Text = UI.Languages.OctoClient.TargetedBlock + ": " + Environment.NewLine
            });

            targetedBlockName = new Label();
            leftView.Controls.Add(targetedBlockName);

            targetedBlockPosition = new Label();
            leftView.Controls.Add(targetedBlockPosition);

            temperatureInfo = new Label();
            rightView.Controls.Add(temperatureInfo);

            precipitationInfo = new Label();
            rightView.Controls.Add(precipitationInfo);

            gravityInfo = new Label();
            rightView.Controls.Add(gravityInfo);

            activeTool = new Label();
            rightView.Controls.Add(activeTool);

            toolCount = new Label();
            rightView.Controls.Add(toolCount);

            flyInfo = new Label();
            rightView.Controls.Add(flyInfo);

            //This Label gets added to the root and is set to Bottom Left
            box = new Label()
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextColor = Color.White
            };
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

            if (Player.CurrentEntity == null || Player.CurrentEntityHead == null)
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
            controlInfo.Text = UI.Languages.OctoClient.ActiveControls + ": " + ScreenManager.ActiveScreen!.Controls.Count;

            // Draw targeted block info
            Player.Selection?.Visit(
            blockInfo =>
            {
                targetedBlockName.Text = definitionManager.GetBlockDefinitionByIndex(blockInfo.Block)?.DisplayName ?? "";
                targetedBlockPosition.Text = blockInfo.Position == default ? "" : blockInfo.Position.ToString();
            },
            componentContainer => SetTargetedBlockInfo(componentContainer));

            void SetTargetedBlockInfo(ComponentContainer container)
            {
                targetedBlockName.Text = container.GetType()?.Name ?? "";
                targetedBlockPosition.Text = container.GetComponent<PositionComponent>()?.Position.ToString() ?? "";
            }

            //Draw Position
            var pos = "pos: " + Player.Position.Position.ToString();
            position.Text = pos;

            //Draw Rotation
            var grad = Player.CurrentEntityHead.Angle / MathHelper.TwoPi * 360;
            var rot = "rot: " +
                (Player.CurrentEntityHead.Angle / MathHelper.TwoPi * 360 % 360).ToString("0.00") + " / " +
                (Player.CurrentEntityHead.Tilt / MathHelper.TwoPi * 360).ToString("0.00");
            rotation.Text = rot;

            //Draw Fps
            var fpsString = "fps: " + (1f / lastfps).ToString("0.00");
            fps.Text = fpsString;

            //Draw Loaded Chunks
            loadedChunks.Text = string.Format("{0}: {1}/{2}",
                UI.Languages.OctoClient.LoadedChunks,
                resourceManager.GetPlanet(Player.Position.Position.Planet).GlobalChunkCache.DirtyChunkColumn,
                resourceManager.GetPlanet(Player.Position.Position.Planet).GlobalChunkCache.LoadedChunkColumns);

            // Draw Loaded Textures
            loadedTextures.Text = $"Loaded Textures: {assets.LoadedTextures}";

            //Get Number of Loaded Items/Blocks
            loadedInfo.Text = "" + definitionManager.ItemDefinitions.Count() + " " + UI.Languages.OctoClient.Items + " - " +
                definitionManager.BlockDefinitions.Count() + " " + UI.Languages.OctoClient.Blocks;

            //Additional Play Information

            //Active Tool
            if (Player.Toolbar.ActiveTool != null)
                activeTool.Text = UI.Languages.OctoClient.ActiveItemTool + ": " + (Player.Toolbar.ActiveTool?.Definition?.DisplayName ?? "No tool") + " | " + Player.Toolbar.GetSlotIndex(Player.Toolbar.ActiveTool);

            toolCount.Text = UI.Languages.OctoClient.ToolCount + ": " + Player.Toolbar.Tools.Count(slot => slot != null);

            ////Fly Info
            //if (Player.ActorHost.Player.FlyMode) flyInfo.Text = UI.Languages.OctoClient.FlymodeEnabled;
            //else flyInfo.Text = "";

            IPlanet planet = resourceManager.GetPlanet(Player.Position.Position.Planet);
            // Temperature Info
            temperatureInfo.Text = UI.Languages.OctoClient.Temperature + ": " + planet.ClimateMap.GetTemperature(Player.Position.Position.GlobalBlockIndex);

            // Precipitation Info
            precipitationInfo.Text = "Precipitation: " + planet.ClimateMap.GetPrecipitation(Player.Position.Position.GlobalBlockIndex);

            // Gravity Info
            gravityInfo.Text = "Gravity" + ": " + planet.Gravity;

            //Draw Box Information
            if (Player.SelectedBox.HasValue)
            {
                var selection =
                    $"box: {Player.SelectedBox.Value} on {Player.SelectedSide} ({Player.SelectedPoint?.X:0.000000}/{Player.SelectedPoint?.Y:0.000000}) -> {Player.SelectedEdge} -> {Player.SelectedCorner}";
                box.Text = selection;
            }
            else
                box.Text = "";

        }
    }
}
