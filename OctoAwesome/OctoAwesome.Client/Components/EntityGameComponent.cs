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
using NLog.LayoutRenderers;

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
                    Ambient.Proj = value;
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
                    Ambient.View = value;
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
        }
        private struct ModelInfo
        {
            public bool render;
            public Texture2D texture;
            public Model model;
        }
        private GraphicsDevice graphicsDevice;
        private EntityModelEffect effect;
        private readonly EffectInstantiator effectInstantiator;
        public SimulationComponent Simulation { get; private set; }

        private Dictionary<string, ModelInfo> models = new Dictionary<string, ModelInfo>();

        public List<ComponentContainer> ComponentContainers { get; set; }

        public EntityGameComponent(OctoGame game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            ComponentContainers = new List<ComponentContainer>();
            graphicsDevice = game.GraphicsDevice;

            effectInstantiator = game.Content.Load<EffectInstantiator>("Effects/entityEffect");
            
        }

        public void LoadShader(entityEffect.entityEffectSettings? settings)
        {
            effect?.Dispose();
            effect = effectInstantiator.CreateInstance<EntityModelEffect, entityEffect.entityEffectSettings>(settings);
        }

        public void Draw(GameTime gameTime, Texture2DArray shadowMaps, Matrix view, Matrix projection, Index3 chunkOffset, Index2 planetSize, Vector3 sunDirection)
        {
            effect.CurrentTechnique = effect.Ambient;
            effect.Ambient.AmbientIntensity = 0.4f;
            effect.Ambient.AmbientColor = Color.White.ToVector4();
            effect.Ambient.View = view;
            effect.Ambient.Proj = projection;
            effect.Ambient.ShadowMaps = shadowMaps;
            effect.Ambient.DiffuseColor = new Color(190, 190, 190);
            effect.Ambient.DiffuseIntensity = 0.6f;
            effect.Ambient.DiffuseDirection = sunDirection;

            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            //using var writer = File.AppendText(Path.Combine(".", "render.log"));
            foreach (var pass in effect.Ambient.Passes)
            {
                pass.Apply();


                foreach (var componentContainer in ComponentContainers)
                {

                    var success = TryGetRenderInfo(componentContainer, out var rendercomp, out var modelinfo);
                    if (!success)
                        continue;

                    var animationcomp = componentContainer.GetComponent<AnimationComponent>();
                    SetTransforms(chunkOffset, planetSize, componentContainer, rendercomp, modelinfo, -0.5f);
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

        public void ApplyCropMatrix(int index, float cascadeDepth, Matrix cropMatrix)
        {
            effect.Ambient.CropMatrices[index] = cropMatrix;
            effect.Ambient.CascadeDepth[index] = cascadeDepth;
            effect.Shadow.CropMatrices[index] = cropMatrix;
        }

        public void DrawShadow(GameTime gameTime, Index3 chunkOffset, Index2 planetSize)
        {
            effect.CurrentTechnique = effect.Shadow;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();


                foreach (var componentContainer in ComponentContainers)
                {

                    var success = TryGetRenderInfo(componentContainer, out var rendercomp, out var modelinfo);
                    if (!success)
                        continue;

                    var animationcomp = componentContainer.GetComponent<AnimationComponent>();
                    SetShadowTransforms(chunkOffset, planetSize, componentContainer, rendercomp, modelinfo, -0.5f);
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


        private void SetTransforms(Index3 chunkOffset, Index2 planetSize, ComponentContainer componentContainer, RenderComponent rendercomp, ModelInfo modelinfo, float zOffset = 0.0f) 
        {
            var world = GetWorldMatrix(chunkOffset, planetSize, componentContainer, rendercomp, zOffset);

            effect.World = world;
            effect.Texture = modelinfo.texture;
            modelinfo.model.Transform = world;
        }

        private void SetShadowTransforms(Index3 chunkOffset, Index2 planetSize, ComponentContainer componentContainer, RenderComponent rendercomp, ModelInfo modelinfo, float zOffset = 0.0f) 
        {
            var world = GetWorldMatrix(chunkOffset, planetSize, componentContainer, rendercomp, zOffset);

            effect.World = world;
            modelinfo.model.Transform = world;
        }

        private static Matrix GetWorldMatrix(Index3 chunkOffset, Index2 planetSize, ComponentContainer componentContainer, RenderComponent rendercomp, float zOffset = 0.0f) 
        {
            var positioncomp = componentContainer.GetComponent<PositionComponent>();
            Debug.Assert(positioncomp != null, nameof(positioncomp) + " != null");
            var position = positioncomp.Position;
            var body = componentContainer.GetComponent<BodyComponent>();

            Index3 shift = chunkOffset.ShortestDistanceXY(
           position.ChunkIndex, planetSize);

            var rotation = MathHelper.WrapAngle(positioncomp.Direction + MathHelper.ToRadians(rendercomp.BaseZRotation));

            Matrix world = Matrix.CreateTranslation(
                shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z + zOffset) * Matrix.CreateScaling(body.Radius * 2, body.Radius * 2, body.Height) * Matrix.CreateRotationZ(rotation);
            return world;
        }

        private bool TryGetRenderInfo(ComponentContainer componentContainer, [MaybeNullWhen(false)] out RenderComponent rendercomp, out ModelInfo modelinfo)
        {
            rendercomp = null;
            modelinfo = default;
            if (!componentContainer.ContainsComponent<RenderComponent>())
            {
                return false;
            }

            rendercomp = componentContainer.GetComponent<RenderComponent>();

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

            ComponentContainers.Clear();
            foreach (var item in simulation.GetByComponentType< PositionComponent>())
            {
                    ComponentContainers.Add(item);
            }

        }
    }
}