using engenious;
using engenious.Graphics;
using engenious.Helper;
using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
namespace OctoAwesome.Client.Components
{
    internal sealed class EntityComponent : GameComponent
    {
        public SimulationComponent Simulation { get; private set; }
        public IEnumerable<Entity> Entities { get; private set; }
        private struct ModelInfo
        {
            public bool render;
            public Texture2D texture;
            public Model model;
        }
        private GraphicsDevice graphicsDevice;
        private BasicEffect effect;
        private Dictionary<string, ModelInfo> models = new Dictionary<string, ModelInfo>();
        // added rendertype field
        private Type rendertype;
        public EntityComponent(Game game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            Entities = new List<Entity>();
            graphicsDevice = game.GraphicsDevice;

            effect = new BasicEffect(graphicsDevice);

            rendertype = typeof(RenderComponent);
        }

        public override void Update(GameTime gameTime)
        {
            if (Simulation.Simulation == null)
                return;

            var simulation = Simulation.Simulation;

            if (!(simulation.State == SimulationState.Running || simulation.State == SimulationState.Paused))
                return;
            // Changed from
            // Entities = simulation.Entities.Where(i => i.Components.ContainsComponent<PositionComponent>()).ToList();
            // to
            Entities = simulation.Entities.Where(i => i.Components.ContainsComponent(rendertype)).ToArray();

            //base.Update(gameTime);
        }

        public void Draw(Matrix view, Matrix projection, Index3 chunkOffset, Index2 planetSize)
        {
            // will be casted to IList internal...
            if (Entities.Count() == 0)
                return;

            effect.Projection = projection;
            effect.View = view;
            effect.TextureEnabled = true;
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (var entity in Entities)
                {
                    RenderComponent rendercomp;
                    if(!entity.Components.TryGetComponent(rendertype, out rendercomp))
                        continue;
                    ModelInfo modelinfo;
                    if (!models.TryGetValue(rendercomp.Name, out modelinfo))
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
                    
                    //TODO: ?? braucht man das : nein
                    //HeadComponent head = new HeadComponent();
                    //if (entity.Components.ContainsComponent<HeadComponent>())
                    //    head = entity.Components.GetComponent<HeadComponent>();

                    Index3 shift = chunkOffset.ShortestDistanceXY(entity.Position.ChunkIndex, planetSize);
                    Vector3 local = entity.Position.LocalPosition;
                    Matrix world = Matrix.CreateTranslation(
                        shift.X * Chunk.CHUNKSIZE_X + local.X,
                        shift.Y * Chunk.CHUNKSIZE_Y + local.Y,
                        shift.Z * Chunk.CHUNKSIZE_Z + local.Z) * 
                        Matrix.CreateScaling(entity.Radius * 2, entity.Radius * 2, entity.Height) *
                        Matrix.CreateRotationZ(MathHelper.WrapAngle(
                            entity.Azimuth + MathHelper.ToRadians(rendercomp.BaseZRotation)));
                    effect.World = world;
                    modelinfo.model.Transform = world;
                    modelinfo.model.Draw(effect, modelinfo.texture);
                }
            }
        }
    }
}
