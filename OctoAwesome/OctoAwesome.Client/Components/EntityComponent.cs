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
    internal sealed class EntityComponent : GameComponent
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

        public EntityComponent(OctoGame game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            Entities = new List<Entity>();
            graphicsDevice = game.GraphicsDevice;

            effect = new BasicEffect(graphicsDevice);
        }

        private int i = 0;
        public void Draw(Matrix view, Matrix projection, Index3 chunkOffset, Index2 planetSize)
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
                                model = Game.Content.Load<Model>(rendercomp.ModelName),
                                texture = Game.Content.Load<Texture2D>(rendercomp.TextureName),
                            };
                        }

                        if (!modelinfo.render)
                            continue;

                        var positioncomp = entity.Components.GetComponent<PositionComponent>();
                        var position = positioncomp.Position;
                        var body = entity.Components.GetComponent<BodyComponent>();

                        HeadComponent head = new HeadComponent(Simulation.Simulation.ResourceManager.DefinitionManager);
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
                        modelinfo.model.Transform = world;

                        modelinfo.model.Draw(effect, modelinfo.texture);
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

            Entities = simulation.Entities.Where(i => i.Components.ContainsComponent<PositionComponent>()).ToList();

            //base.Update(gameTime);
        }
    }
}
