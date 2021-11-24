using engenious;
using engenious.Graphics;
using engenious.Helper;

using OctoAwesome.EntityComponents;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Components
{
    internal sealed class EntityGameComponent : GameComponent
    {
        private struct ModelInfo
        {
            public bool render;
            public Texture2D texture;
            public Model model;
        }
        private GraphicsDevice graphicsDevice;
        private BasicEffect effect;
        public SimulationComponent Simulation { get; private set; }


        private Dictionary<string, ModelInfo> models = new Dictionary<string, ModelInfo>();


        public List<Entity> Entities { get; set; }
        public List<FunctionalBlock> FunctionalBlocks { get; set; }

        public EntityGameComponent(OctoGame game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            Entities = new List<Entity>();
            FunctionalBlocks = new List<FunctionalBlock>();
            graphicsDevice = game.GraphicsDevice;

            effect = new BasicEffect(graphicsDevice);
        }

        private int i = 0;
        public void Draw(GameTime gameTime, Matrix view, Matrix projection, Index3 chunkOffset, Index2 planetSize)
        {
            effect.Projection = projection;
            effect.View = view;
            effect.TextureEnabled = true;
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            using (var writer = File.AppendText(Path.Combine(".", "render.log")))
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    i++;
                    foreach (var entity in Entities)
                    {
                        if (!entity.Components.ContainsComponent<RenderComponent>())
                        {
                            continue;
                        }

                        var rendercomp = entity.Components.GetComponent<RenderComponent>();


                        if (!models.TryGetValue(rendercomp.Name, out ModelInfo modelinfo))
                        {
                            modelinfo = new ModelInfo()
                            {
                                render = true,
                                model = Game.Content.Load<Model>(rendercomp.ModelName)!,
                                texture = Game.Content.Load<Texture2D>(rendercomp.TextureName)!,
                            };
                        }

                        if (!modelinfo.render)
                            continue;

                        var positioncomp = entity.Components.GetComponent<PositionComponent>();
                        var position = positioncomp.Position;
                        var body = entity.Components.GetComponent<BodyComponent>();

                        HeadComponent head = new HeadComponent();
                        if (entity.Components.ContainsComponent<HeadComponent>())
                            head = entity.Components.GetComponent<HeadComponent>();

                        Index3 shift = chunkOffset.ShortestDistanceXY(
                       position.ChunkIndex, planetSize);

                        var rotation = MathHelper.WrapAngle(positioncomp.Direction + MathHelper.ToRadians(rendercomp.BaseZRotation));

                        Matrix world = Matrix.CreateTranslation(
                            shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                            shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                            shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z) * Matrix.CreateScaling(body.Radius * 2, body.Radius * 2, body.Height) * Matrix.CreateRotationZ(rotation);
                        effect.World = world;
                        effect.Texture = modelinfo.texture;
                        modelinfo.model.Transform = world;

                        modelinfo.model.Draw(effect);
                    }

                    foreach (var functionalBlock in FunctionalBlocks)
                    {
                        if (!functionalBlock.Components.ContainsComponent<RenderComponent>())
                        {
                            continue;
                        }

                        var rendercomp = functionalBlock.Components.GetComponent<RenderComponent>();


                        if (!models.TryGetValue(rendercomp.Name, out ModelInfo modelinfo))
                        {
                            modelinfo = new ModelInfo()
                            {
                                render = true,
                                model = Game.Content.Load<Model>(rendercomp.ModelName)!,
                                texture = Game.Content.Load<Texture2D>(rendercomp.TextureName)!,
                            };
                        }

                        if (!modelinfo.render)
                            continue;

                        var positioncomp = functionalBlock.Components.GetComponent<PositionComponent>();
                        var animationcomp = functionalBlock.Components.GetComponent<AnimationComponent>();
                        var position = positioncomp.Position;
                        var body = functionalBlock.Components.GetComponent<BodyComponent>();

                        Index3 shift = chunkOffset.ShortestDistanceXY(
                       position.ChunkIndex, planetSize);

                        var rotation = MathHelper.WrapAngle(positioncomp.Direction + MathHelper.ToRadians(rendercomp.BaseZRotation));

                        Matrix world = Matrix.CreateTranslation(
                            shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                            shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                            shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z - 0.5f) * Matrix.CreateScaling(body.Radius * 2, body.Radius * 2, body.Height) * Matrix.CreateRotationZ(rotation);
                        effect.World = world;
                        effect.Texture = modelinfo.texture;
                        modelinfo.model.Transform = world;
                        modelinfo.model.CurrentAnimation = modelinfo.model.Animations.FirstOrDefault();
                        if (animationcomp is not null)
                        {
                            animationcomp.MaxTime = modelinfo.model.CurrentAnimation?.MaxTime ?? 0f;
                            animationcomp.Update(gameTime, modelinfo.model);
                        }
                        //modelinfo.model.CurrentAnimation.MaxTime
                        //modelinfo.model.UpdateAnimation((float)(gameTime.TotalGameTime.TotalMilliseconds / 20));
                        //modelinfo.model.CurrentAnimation?.Update(DateTime.Now.Second);
                        modelinfo.model.Draw(effect);
                    }
                }
        }

        public override void Update(GameTime gameTime)
        {
            if (Simulation?.Simulation == null)
                return;

            var simulation = Simulation.Simulation;

            if (!(simulation.State == SimulationState.Running || simulation.State == SimulationState.Paused))
                return;

            Entities.Clear();
            foreach (var item in simulation.Entities)
            {
                if (item.Components.ContainsComponent<PositionComponent>())
                    Entities.Add(item);
            }
            FunctionalBlocks.Clear();
            foreach (var item in simulation.FunctionalBlocks)
            {
                if (item.Components.ContainsComponent<PositionComponent>())
                    FunctionalBlocks.Add(item);
            }
            //base.Update(gameTime);
        }
    }
}
