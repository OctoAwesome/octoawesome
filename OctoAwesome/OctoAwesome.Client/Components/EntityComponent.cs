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
        private GraphicsDevice graphicsDevice;
        private VertexBuffer vb;
        private IndexBuffer ib;
        private BasicEffect effect;
        public SimulationComponent Simulation { get; private set; }


        public List<Entity> Entities { get; set; }

        public EntityComponent(Game game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            Entities = new List<Entity>();
            graphicsDevice = game.GraphicsDevice;
            vb = new VertexBuffer(graphicsDevice, VertexPositionColor.VertexDeclaration, 24);
            vb.SetData(new[] {  new VertexPositionColor(new Vector3(-0.5f,-0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,-0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,+0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,+0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,-0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,+0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,+0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,-0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,+0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,+0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,+0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,+0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,-0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,-0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,-0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,-0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,-0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,+0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,+0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(+0.5f,-0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,-0.5f,-0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,-0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,+0.5f,+0.5f+0.5f),Color.Red),
                                new VertexPositionColor(new Vector3(-0.5f,+0.5f,-0.5f+0.5f),Color.Red) });

            ib = new IndexBuffer(graphicsDevice, DrawElementsType.UnsignedByte, 36);
            ib.SetData(new byte[]
            {
              0,  1,  2,      0,  2,  3,    // front
              4,  5,  6,      4,  6,  7,    // back
              8,  9,  10,     8,  10, 11,   // top
              12, 13, 14,     12, 14, 15,   // bottom
              16, 17, 18,     16, 18, 19,   // right
              20, 21, 22,     20, 22, 23    // left
            }
            );
            effect = new BasicEffect(graphicsDevice);
        }
        public void Draw(Matrix view, Matrix projection, Index3 chunkOffset, Index2 planetSize)
        {
            graphicsDevice.VertexBuffer = vb;
            graphicsDevice.IndexBuffer = ib;
            effect.Projection = projection;
            effect.View = view;
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (var entity in Entities)
                {
                    var position = entity.Components.GetComponent<PositionComponent>().Position;
                    var body = entity.Components.GetComponent<BodyComponent>();
                    Index3 shift = chunkOffset.ShortestDistanceXY(
                   position.ChunkIndex, planetSize);

                    Matrix world = Matrix.CreateTranslation(
                        shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                        shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                        shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z)* Matrix.CreateScaling(body.Radius*2, body.Radius*2, body.Height);
                    effect.World = world;
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, ib.IndexCount, 0, ib.IndexCount / 3);
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
