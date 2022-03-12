using engenious;
using engenious.Graphics;
using engenious.Helper;

using OctoAwesome.Chunking;
using OctoAwesome.EntityComponents;
using OctoAwesome.Location;

using System.Collections.Generic;
using System.IO;
using System.Linq;

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


        public IReadOnlyCollection<ComponentContainer> ComponentContainers { get; set; }

        public EntityGameComponent(OctoGame game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            ComponentContainers = new List<ComponentContainer>();
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
                    foreach (var componentContainer in ComponentContainers)
                    {
                        var rendercomp = componentContainer.GetComponent<RenderComponent>();
                        if (rendercomp == default)
                        {
                            continue;
                        }


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

                        var positioncomp = componentContainer.GetComponent<PositionComponent>();
                        var position = positioncomp.Position;
                        var animationcomp = componentContainer.GetComponent<AnimationComponent>();
                        var body = componentContainer.GetComponent<BodyComponent>();

                        //HeadComponent head = new HeadComponent();
                        //if (entity.ContainsComponent<HeadComponent>())
                        //    head = entity.GetComponent<HeadComponent>();

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
                        modelinfo.model.CurrentAnimation = modelinfo.model.Animations.FirstOrDefault();

                        if (animationcomp is not null)
                        {
                            animationcomp.MaxTime = modelinfo.model.CurrentAnimation?.MaxTime ?? 0f;
                            animationcomp.Update(gameTime, modelinfo.model);
                        }

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

            ComponentContainers = simulation.GetByComponentType<PositionComponent>();
            //base.Update(gameTime);
        }
    }
}
