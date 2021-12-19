using engenious;
using engenious.Graphics;
using engenious.Helper;
using engenious.UserDefined.Effects;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace OctoAwesome.Client.Components
{
    internal sealed class EntityGameComponent : GameComponent
    {
        private class EntityModelEffect : entityEffect, IModelEffect
        {
            private Matrix projection;
            private Matrix view;
            private Matrix world;
            private Texture texture;

            public EntityModelEffect(GraphicsDevice graphicsDevice) : base(graphicsDevice)
            {
            }

            public Matrix Projection
            {
                get => projection;
                set
                {
                    if (projection == value)
                        return;
                    projection = value;
                    UpdateViewProjection();
                }
            }
            public Matrix View
            {
                get => view;
                set
                {
                    if (view == value)
                        return;
                    view = value;
                    UpdateViewProjection();
                }
            }
            public Matrix World
            {
                get => world;
                set
                {
                    if (world == value)
                        return;
                    world = value;
                    Ambient.World = value;
                    Shadow.World = value;
                }
            }

            public Texture Texture
            {
                get => texture;
                set
                {
                    texture = value;
                    Ambient.Texture = (Texture2D)value;
                }
            }

            private void UpdateViewProjection()
            {
                var viewProj = projection * view;
                Ambient.ViewProjection = viewProj;
                Shadow.ViewProjection = viewProj;
            }
        }
        private struct ModelInfo
        {
            public bool render;
            public Texture2D texture;
            public Model model;
        }
        private GraphicsDevice graphicsDevice;
        private EntityModelEffect effect;
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

            effect = game.Content.Load<EntityModelEffect>("Effects/entityEffect");
        }

        public void Draw(GameTime gameTime, RenderTarget2D shadowMap, Matrix view, Matrix projection, Matrix cropMatrix, Index3 chunkOffset, Index2 planetSize, Vector3 sunDirection)
        {
            var viewProj = projection * view;
            var biasMatrix = new Matrix(
              0.5f, 0.0f, 0.0f, 0.0f,
              0.0f, 0.5f, 0.0f, 0.0f,
              0.0f, 0.0f, 0.5f, 0.0f,
              0.5f, 0.5f, 0.5f, 1.0f
              );
            var depthBiasWorldViewProj = biasMatrix * cropMatrix;

            effect.Ambient.DepthBiasViewProj = depthBiasWorldViewProj;
            effect.CurrentTechnique = effect.Ambient;
            effect.Ambient.AmbientIntensity = 0.4f;
            effect.Ambient.AmbientColor = Color.White.ToVector4();
            effect.Ambient.ViewProjection = viewProj;
            effect.Ambient.ShadowMap = shadowMap;
            effect.Ambient.DiffuseColor = new Color(190, 190, 190);
            effect.Ambient.DiffuseIntensity = 0.6f;
            effect.Ambient.DiffuseDirection = sunDirection;

            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            //using var writer = File.AppendText(Path.Combine(".", "render.log"));
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var entity in Entities)
                {
                    var success = TryGetRenderInfo(entity, out var rendercomp, out var modelinfo);
                    if (!success)
                        continue;

                    SetTransforms(chunkOffset, planetSize, entity, rendercomp!, modelinfo);
                    modelinfo.model.Draw(effect);
                }

                foreach (var functionalBlock in FunctionalBlocks)
                {

                    var success = TryGetRenderInfo(functionalBlock, out var rendercomp, out var modelinfo);
                    if (!success)
                        continue;

                    var animationcomp = functionalBlock.Components.GetComponent<AnimationComponent>();
                    SetTransforms(chunkOffset, planetSize, functionalBlock, rendercomp, modelinfo, -0.5f);
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

        public void DrawShadow(GameTime gameTime, Matrix cropMatrix, Index3 chunkOffset, Index2 planetSize)
        {
            effect.CurrentTechnique = effect.Shadow;
            effect.Shadow.ViewProjection = cropMatrix;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var entity in Entities)
                {
                    var success = TryGetRenderInfo(entity, out var rendercomp, out var modelinfo);
                    if (!success)
                        continue;

                    SetShadowTransforms(chunkOffset, planetSize, entity, rendercomp, modelinfo);
                    modelinfo.model.Draw(effect);
                }

                foreach (var functionalBlock in FunctionalBlocks)
                {

                    var success = TryGetRenderInfo(functionalBlock, out var rendercomp, out var modelinfo);
                    if (!success)
                        continue;

                    var animationcomp = functionalBlock.Components.GetComponent<AnimationComponent>();
                    SetShadowTransforms(chunkOffset, planetSize, functionalBlock, rendercomp, modelinfo, -0.5f);
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


        private void SetTransforms<T>(Index3 chunkOffset, Index2 planetSize, ComponentContainer<T> componentContainer, RenderComponent rendercomp, ModelInfo modelinfo, float zOffset = 0.0f) where T : IComponent
        {
            var world = GetWorldMatrix(chunkOffset, planetSize, componentContainer, rendercomp, zOffset);

            effect.World = world;
            effect.Texture = modelinfo.texture;
            modelinfo.model.Transform = world;
        }

        private void SetShadowTransforms<T>(Index3 chunkOffset, Index2 planetSize, ComponentContainer<T> componentContainer, RenderComponent rendercomp, ModelInfo modelinfo, float zOffset = 0.0f) where T : IComponent
        {
            var world = GetWorldMatrix(chunkOffset, planetSize, componentContainer, rendercomp, zOffset);
       
            effect.World = world;
            modelinfo.model.Transform = world;
        }

        private static Matrix GetWorldMatrix<T>(Index3 chunkOffset, Index2 planetSize, ComponentContainer<T> componentContainer, RenderComponent rendercomp, float zOffset = 0.0f) where T : IComponent
        {
            var positioncomp = componentContainer.Components.GetComponent<PositionComponent>();
            Debug.Assert(positioncomp != null, nameof(positioncomp) + " != null");
            var position = positioncomp.Position;
            var body = componentContainer.Components.GetComponent<BodyComponent>();

            Index3 shift = chunkOffset.ShortestDistanceXY(
           position.ChunkIndex, planetSize);

            var rotation = MathHelper.WrapAngle(positioncomp.Direction + MathHelper.ToRadians(rendercomp.BaseZRotation));

            Matrix world = Matrix.CreateTranslation(
                shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z + zOffset) * Matrix.CreateScaling(body.Radius * 2, body.Radius * 2, body.Height) * Matrix.CreateRotationZ(rotation);
            return world;
        }

        private bool TryGetRenderInfo<T>(ComponentContainer<T> componentContainer, [MaybeNullWhen(false)] out RenderComponent rendercomp, out ModelInfo modelinfo) where T : IComponent
        {
            rendercomp = null;
            modelinfo = default;
            if (!componentContainer.Components.ContainsComponent<RenderComponent>())
            {
                return false;
            }

            rendercomp = componentContainer.Components.GetComponent<RenderComponent>();

            Debug.Assert(rendercomp != null, nameof(rendercomp) + " != null");
            if (!models.TryGetValue(rendercomp.Name, out modelinfo))
            {
                modelinfo = new ModelInfo()
                {
                    render = true,
                    model = Game.Content.Load<Model>(rendercomp.ModelName)!,
                    texture = Game.Content.Load<Texture2D>(rendercomp.TextureName)!,
                };
            }

            if (!modelinfo.render)
                return false;

            return true;
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
