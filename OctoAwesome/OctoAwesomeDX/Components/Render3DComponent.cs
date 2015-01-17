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
        VertexPositionColor[] vertices;
        BasicEffect effect;

        public Render3DComponent(Game game)
            : base(game)
        {

        }

        protected override void LoadContent()
        {
            vertices = new VertexPositionColor[] {
                new VertexPositionColor(new Vector3(-5, 5, 0), Color.Red),
                new VertexPositionColor(new Vector3(5, 5, 0), Color.Green),
                new VertexPositionColor(new Vector3(0, -5, 0), Color.Yellow),
            };

            effect = new BasicEffect(GraphicsDevice);
            // effect.EnableDefaultLighting();
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 20), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1f, 10000f);
            effect.VertexColorEnabled = true;

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
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, 1);
            }
        }
    }
}
