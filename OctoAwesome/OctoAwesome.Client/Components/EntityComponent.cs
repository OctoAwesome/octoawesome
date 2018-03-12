using engenious;
using engenious.Graphics;
using engenious.Helper;
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
        private struct ModelInfo
        {
            public bool render;
            public Texture2D texture;
            public Model model;
        }
        private GraphicsDevice graphicsDevice;
        private Effect effect;
        public SimulationComponent Simulation { get; private set; }


        private Dictionary<string, ModelInfo> models = new Dictionary<string, ModelInfo>();


        public List<Entity> Entities { get; set; }

        public EntityComponent(Game game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            Entities = new List<Entity>();
            graphicsDevice = game.GraphicsDevice;

            effect = Game.Content.Load<Effect>("Effects/simple");
        }
        
        public void DrawShadow(Matrix viewProjection, Index3 chunkOffset, Index2 planetSize)
        {
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            foreach (var pass in effect.Techniques["EntityShadow"].Passes)
            {
                pass.Apply();

                foreach (var entity in Entities)
                {
                    if (!entity.Components.ContainsComponent<RenderComponent>())
                    {
                        continue;
                    }

                    var rendercomp = entity.Components.GetComponent<RenderComponent>();

                    ModelInfo modelinfo;

                    if (!models.TryGetValue(rendercomp.Name,out modelinfo))
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

                    HeadComponent head = new HeadComponent();
                    if (entity.Components.ContainsComponent<HeadComponent>())
                        head = entity.Components.GetComponent<HeadComponent>();

                    Index3 shift = chunkOffset.ShortestDistanceXY(
                   position.ChunkIndex, planetSize);

                    var rotation = MathHelper.WrapAngle(positioncomp.Direction + MathHelper.ToRadians(rendercomp.BaseZRotation));

                    Matrix world = Matrix.CreateTranslation(
                        shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                        shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                        shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z) 
                                   * Matrix.CreateScaling(body.Radius /40, body.Radius /40, body.Height/80)
                                   *Matrix.CreateRotationZ(rotation);
                    
                    effect.Parameters["WorldViewProj"].SetValue(viewProjection*world);
                    
                    modelinfo.model.Transform = world;
                    modelinfo.model.Draw();
                }
            }
        }
        
        public void Draw(Matrix view, Matrix projection, Matrix shadowViewProjection,RenderTarget2D shadowMap , Index3 chunkOffset, Index2 planetSize)
        {
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            foreach (var pass in effect.Techniques["EntityBasic"].Passes)
            {
                pass.Apply();

                foreach (var entity in Entities)
                {
                    if (!entity.Components.ContainsComponent<RenderComponent>())
                        continue;

                    var rendercomp = entity.Components.GetComponent<RenderComponent>();

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
                        shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z) 
                                   * Matrix.CreateScaling(body.Radius /40, body.Radius /40, body.Height/80)
                                   *Matrix.CreateRotationZ(rotation);

                    Matrix worldViewProj = projection * view * world;
                    Matrix shadowworldViewProj = shadowViewProjection * world;
                    
                    effect.Parameters["WorldViewProj"].SetValue(worldViewProj);
                    effect.Parameters["text"].SetValue(modelinfo.texture);
                    
                    
                    
                    effect.Parameters["shadowWorldViewProj"].SetValue(shadowworldViewProj);

                    if (shadowMap != null)
                    {
                        effect.Parameters["ShadowMap"].SetValue(shadowMap);
                        effect.Parameters["ShadowEnabled"].SetValue(1);
                    }
                    else
                    {
                        effect.Parameters["ShadowEnabled"].SetValue(0);
                    }
                    
                    
                    modelinfo.model.Transform = world;
                    modelinfo.model.Draw();
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
