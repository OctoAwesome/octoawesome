using engenious;
using engenious.Graphics;
using engenious.Helper;
using OctoAwesome.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Client.Components
{
    internal sealed class EntityComponent : GameComponent
    {
        public SimulationComponent Simulation { get; private set; }
        public IEnumerable<Entity> Entities => entities;
        private struct ModelInfo
        {
            public bool render;
            public Texture2D texture;
            public Model model;
        }
        private GraphicsDevice graphicsDevice;
        private BasicEffect effect;
        private Dictionary<string, ModelInfo> models = new Dictionary<string, ModelInfo>();
        private List<Entity> entities;
        public EntityComponent(Game game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            entities = new List<Entity>();
            graphicsDevice = game.GraphicsDevice;

            effect = new BasicEffect(graphicsDevice);
        }
        public void Draw(Matrix view, Matrix projection, Index3 chunkOffset, Index2 planetSize)
        {
            if (entities.Count() == 0)
                return;

            effect.Projection = projection;
            effect.View = view;
            effect.TextureEnabled = true;
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            foreach (var pass in effect.CurrentTechnique.Passes.PassesList)
            {
                pass.Apply();

                foreach (var entity in entities)
                {
                    var drawable = entity as Entities.IDrawable;

                    if (!models.TryGetValue(drawable.Name, out ModelInfo modelinfo))
                    {
                        modelinfo = new ModelInfo()
                        {
                            model = Game.Content.Load<Model>(drawable.ModelName),
                            texture = Game.Content.Load<Texture2D>(drawable.TextureName),
                            render = true,
                        };
                    }

                    if (!modelinfo.render)
                        continue;
                    
                    Coordinate position = entity.Position;

                    Index3 shift = chunkOffset.ShortestDistanceXY(position.ChunkIndex, planetSize);
                    var rotation = MathHelper.WrapAngle(drawable.Azimuth + MathHelper.ToRadians(drawable.BaseRotationZ));

                    Matrix world = Matrix.CreateTranslation(
                        shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                        shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                        shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z) * 
                        Matrix.CreateScaling(drawable.Radius, drawable.Radius, drawable.Height) *
                        Matrix.CreateRotationZ(rotation);
                    effect.World = world;
                    modelinfo.model.Transform = world;
                    modelinfo.model.Draw(effect, modelinfo.texture);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            var simulation = Simulation.Simulation;

            if (simulation == null || !(simulation.State == SimulationState.Running || simulation.State == SimulationState.Paused))
                return;

            entities = simulation.Entities.Where(i => i is Entities.IDrawable ent && ent.DrawUpdate).ToList();

            //base.Update(gameTime);
        }
    }
}
