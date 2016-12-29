using engenious;
using engenious.Graphics;
using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Components
{
    internal sealed class EntityComponent : GameComponent
    {
        private Texture2D texture;
        private Model model;
        private GraphicsDevice graphicsDevice;
        private BasicEffect effect;
        public SimulationComponent Simulation { get; private set; }


        public List<Entity> Entities { get; set; }

        public EntityComponent(Game game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            Entities = new List<Entity>();
            graphicsDevice = game.GraphicsDevice;
           
            effect = new BasicEffect(graphicsDevice);

            model = game.Content.Load<Model>("dog");
            texture = game.Content.Load<Texture2D>("texdog");

        }
        public void Draw(Matrix view, Matrix projection, Index3 chunkOffset, Index2 planetSize)
        {
            effect.Projection = projection;
            effect.View = view;
            effect.TextureEnabled = true;
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (var entity in Entities)
                {
                    var position = entity.Components.GetComponent<PositionComponent>().Position;
                    var body = entity.Components.GetComponent<BodyComponent>();

                    HeadComponent head = new HeadComponent();
                    if (entity.Components.ContainsComponent<HeadComponent>())
                        head = entity.Components.GetComponent<HeadComponent>();

                    Index3 shift = chunkOffset.ShortestDistanceXY(
                   position.ChunkIndex, planetSize);

                    Matrix world = Matrix.CreateTranslation(
                        shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                        shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                        shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z)* Matrix.CreateScaling(body.Radius*2, body.Radius*2, body.Height);
                    effect.World = world;
                    model.Transform = world;
                    model.Draw(effect,texture);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Simulation.Simulation == null)
                return;

            var simulation = Simulation.Simulation;

            if (!(simulation.State == SimulationState.Running || simulation.State == SimulationState.Paused))
                return;

            Entities = simulation.Entities.Where(i => i.Components.ContainsComponent<PositionComponent>()).ToList();

            //base.Update(gameTime);
        }
    }
}
