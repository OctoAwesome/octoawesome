using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class Render3DComponent : DrawableGameComponent
    {
        VertexPositionNormalTexture[] vertices;
        BasicEffect effect;
        Texture2D sand;

        public Render3DComponent(Game game)
            : base(game)
        {

        }

        protected override void LoadContent()
        {
            vertices = new VertexPositionNormalTexture[] {
                new VertexPositionNormalTexture(new Vector3(-1, 1, 0), Vector3.Forward, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 0), Vector3.Forward, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(1, -1, 0), Vector3.Forward, new Vector2(1, 1)),
                new VertexPositionNormalTexture(new Vector3(-1, 1, 0), Vector3.Forward, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(1, -1, 0), Vector3.Forward, new Vector2(1, 1)),
                new VertexPositionNormalTexture(new Vector3(-1, -1, 0), Vector3.Forward, new Vector2(0, 1)),
            };

            sand = Game.Content.Load<Texture2D>("Textures/sand_center");

            effect = new BasicEffect(GraphicsDevice);
            // effect.EnableDefaultLighting();
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1f, 10000f);
            effect.TextureEnabled = true;
            effect.Texture = sand;

            base.LoadContent();
        }

        float rotY = 0f;

        public override void Update(GameTime gameTime)
        {
            rotY += (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.RasterizerState.CullMode = CullMode.None;
            // GraphicsDevice.RasterizerState.FillMode = FillMode.WireFrame;

            effect.World = Matrix.CreateRotationY(rotY);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices, 0, 2);
            }
        }
    }
}
