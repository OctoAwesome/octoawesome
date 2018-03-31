using OctoAwesome.Client.Controls;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Components
{
    internal sealed class ChunkRenderer : IDisposable
    {
        private Effect simple;
        private GraphicsDevice graphicsDevice;

        private Texture2DArray textures;

        /// <summary>
        /// Referenz auf den aktuellen Chunk (falls vorhanden)
        /// </summary>
        private IChunk chunk;
        private bool loaded = false;

        private VertexBuffer vb;
        private static IndexBuffer ib;
        private int vertexCount = 0;
        private int indexCount = 0;
        private ILocalChunkCache manager;

        private readonly SceneControl sceneControl;
        private IDefinitionManager definitionManager;

        private readonly object ibLock = new object();
        private Index3? chunkPosition;
        
        /// <summary>
        /// Adresse des aktuellen Chunks
        /// </summary>
        public Index3? ChunkPosition
        {
            get => chunkPosition;
            private set
            {
                chunkPosition = value;
                NeedsUpdate = value != null;
            }
        }

        public ChunkRenderer(SceneControl sceneControl, IDefinitionManager definitionManager, Effect simpleShader, GraphicsDevice graphicsDevice, Matrix projection, Texture2DArray textures)
        {
            this.sceneControl = sceneControl;
            this.definitionManager = definitionManager;
            this.graphicsDevice = graphicsDevice;
            this.textures = textures;

            simple = simpleShader;
            GenerateIndexBuffer();
        }

        public void SetChunk(ILocalChunkCache manager, int x, int y, int z)
        {
            var newPosition = new Index3(x, y, z);

            if (this.manager == manager && newPosition == ChunkPosition)
            {
                NeedsUpdate = !loaded;
                return;
            }

            this.manager = manager;
            ChunkPosition = newPosition;

            if (chunk != null)
            {
                chunk.Changed -= OnChunkChanged;
                chunk = null;
            }
           
            loaded = false;
            NeedsUpdate = true;
        }

        public bool NeedsUpdate = false;
        

        private void OnChunkChanged(IChunk c, int n)
        {
            NeedsUpdate = true;
            sceneControl.Enqueue(this);
        }
        
        public void DrawShadow(Matrix viewProjection, Index3 shift)
        {
            if (!loaded)
                return;

            
            Matrix worldViewProj = viewProjection * Matrix.CreateTranslation(
                                       shift.X * Chunk.CHUNKSIZE_X,
                                       shift.Y * Chunk.CHUNKSIZE_Y,
                                       shift.Z * Chunk.CHUNKSIZE_Z);

            simple.Parameters["WorldViewProj"].SetValue(worldViewProj);

            lock (this)
            {
                if (vb == null)
                    return;
                
                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                graphicsDevice.VertexBuffer = vb;
                graphicsDevice.IndexBuffer = ib;

                foreach (var pass in simple.Techniques["BlockShadow"].Passes)
                {
                    if (vertexCount == 0 || indexCount == 0)
                        continue;
                    pass.Apply();
                    //graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, vertexCount, 0, indexCount / 3);
                }
            }
        }
        
        public void Draw(Matrix view, Matrix projection,Matrix shadowViewProjection,RenderTarget2D shadowMap, Index3 shift)
        {
            if (!loaded)
                return;

            Matrix worldViewProj = projection * view * Matrix.CreateTranslation(
                shift.X * Chunk.CHUNKSIZE_X,
                shift.Y * Chunk.CHUNKSIZE_Y,
                shift.Z * Chunk.CHUNKSIZE_Z);
            
            simple.Parameters["WorldViewProj"].SetValue(worldViewProj);
            simple.Parameters["BlockTextures"].SetValue(textures);
     
            Matrix shadowworldViewProj = shadowViewProjection * Matrix.CreateTranslation(
                                             shift.X * Chunk.CHUNKSIZE_X,
                                             shift.Y * Chunk.CHUNKSIZE_Y,
                                             shift.Z * Chunk.CHUNKSIZE_Z);
            
            simple.Parameters["shadowWorldViewProj"].SetValue(shadowworldViewProj);
            if (shadowMap != null)
            {
                simple.Parameters["ShadowMap"].SetValue(shadowMap);
                simple.Parameters["ShadowEnabled"].SetValue(1);
            }
            else
            {
                simple.Parameters["ShadowEnabled"].SetValue(0);
            }
            

            lock (this)
            {
                if (vb == null)
                    return;
                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                graphicsDevice.VertexBuffer = vb;
                graphicsDevice.IndexBuffer = ib;

                foreach (var pass in simple.Techniques["BlockBasic"].Passes)
                {
                    if (vertexCount == 0 || indexCount == 0)
                        continue;
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, vertexCount, 0, indexCount / 3);
                }
            }
        }

        public void GenerateIndexBuffer()
        {
            lock(ibLock){
                if (ib != null)
                    return;
                ib = new IndexBuffer(graphicsDevice,DrawElementsType.UnsignedInt,Chunk.CHUNKSIZE_X*Chunk.CHUNKSIZE_Y*Chunk.CHUNKSIZE_Z*6*6);
                List<int> indices = new List<int>(ib.IndexCount);
                for (int i=0;i<ib.IndexCount*2/3;i+=4){
                    indices.Add(i + 0);
                    indices.Add(i + 1);
                    indices.Add(i + 3);

                    indices.Add(i + 0);
                    indices.Add(i + 3);
                    indices.Add(i + 2);
                }
                ib.SetData(indices.ToArray());
            }

        }
        
        private int ChangeStart = -1;

        public bool RegenerateVertexBuffer()
        {
            if (!ChunkPosition.HasValue)
                return false;


            // Chunk nachladen
            if (this.chunk == null)
            {
                this.chunk = manager.GetChunk(ChunkPosition.Value);
                if (this.chunk == null)
                {
                    //Thread.Sleep(10);
                    //RegenerateVertexBuffer();
                    //NeedsUpdate = false;
                    return false;
                }

                this.chunk.Changed += OnChunkChanged;
            }
            var chunk = this.chunk;
            ChangeStart = chunk.ChangeCounter;
            List<VertexPositionNormalTextureLight> vertices = new List<VertexPositionNormalTextureLight>();
            
            int textureColumns = textures.Width / SceneControl.TEXTURESIZE;
            float textureWidth = 1f / textureColumns;
            float texelSize = 1f / SceneControl.TEXTURESIZE;
            float textureSizeGap = texelSize;
            float textureGap = texelSize / 2;
            
            // BlockTypes sammlen
            Dictionary<IBlockDefinition, int> textureOffsets = new Dictionary<IBlockDefinition, int>();
            
            int definitionIndex = 0;
            foreach (var definition in definitionManager.GetBlockDefinitions())
            {
                int textureCount = definition.Textures.Count();
                textureOffsets.Add(definition, definitionIndex);
                // definitionMapping.Add(definition.GetBlockType(), definition);
                definitionIndex += textureCount;
            }


            var indexCount = 0;
            Vector2[] uvOffsets = new[] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
            for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    {
                        ushort block = chunk.GetBlock(x, y, z);
                        if (block == 0)
                            continue;

                        IBlockDefinition blockDefinition = (IBlockDefinition)definitionManager.GetDefinitionByIndex(block);
                        if (blockDefinition == null)
                            continue;

                        int textureIndex;
                        if (!textureOffsets.TryGetValue(blockDefinition, out textureIndex))
                            continue;

                        // Textur-Koordinate "berechnen"
                        Vector2 textureOffset = new Vector2();


                        ushort topBlock = manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z + 1));
                        IBlockDefinition topBlockDefintion = (IBlockDefinition)definitionManager.GetDefinitionByIndex(topBlock);

                        // Top
                        if (topBlock == 0 || (!topBlockDefintion.IsSolidWall(Wall.Bottom) && topBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Top,manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Top,manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Top, manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top,manager, x, y, z)),
                                    0));
                            indexCount += 6;
                        }

                        ushort bottomBlock = manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z - 1));
                        IBlockDefinition bottomBlockDefintion = (IBlockDefinition)definitionManager.GetDefinitionByIndex(bottomBlock);


                        // Unten
                        if (bottomBlock == 0 || (!bottomBlockDefintion.IsSolidWall (Wall.Top) && bottomBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Bottom,manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom,manager, x, y, z)),
                                    0));
                            indexCount += 6;
                        }

                        ushort southBlock = manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y + 1, z));
                        IBlockDefinition southBlockDefintion = (IBlockDefinition)definitionManager.GetDefinitionByIndex(southBlock);

                        // South
                        if (southBlock == 0 || (!southBlockDefintion.IsSolidWall(Wall.Front) && southBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Front,manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Front,manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Front, manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front,manager, x, y, z)),
                                    0));
                            indexCount += 6;
                        }

                        ushort northBlock = manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y - 1, z));
                        IBlockDefinition northBlockDefintion = (IBlockDefinition)definitionManager.GetDefinitionByIndex(northBlock);

                        // North
                        if (northBlock == 0 || (!northBlockDefintion.IsSolidWall (Wall.Back) && northBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Back,manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Back,manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Back, manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back,manager, x, y, z)),
                                    0));
                            indexCount += 6;
                        }

                        ushort westBlock = manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x - 1, y, z));
                        IBlockDefinition westBlockDefintion = (IBlockDefinition)definitionManager.GetDefinitionByIndex(westBlock);

                        // West
                        if (westBlock == 0 || (!westBlockDefintion.IsSolidWall(Wall.Right) && westBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Left,manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Left,manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);


                            int rotation = -blockDefinition.GetTextureRotation(Wall.Left, manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left,manager, x, y, z)),
                                    0));
                            indexCount += 6;
                        }

                        ushort eastBlock = manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x + 1, y, z));
                        IBlockDefinition eastBlockDefintion = (IBlockDefinition)definitionManager.GetDefinitionByIndex(eastBlock);

                        // Ost
                        if (eastBlock == 0 || (!eastBlockDefintion.IsSolidWall(Wall.Left) && eastBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Right,manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Right,manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);


                            int rotation = -blockDefinition.GetTextureRotation(Wall.Right,manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right,manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right,manager, x, y, z)),
                                    0));
                            indexCount += 6;
                        }
                    }
                }
            }

            var vertexCount = vertices.Count;

            if (vertexCount > 0)
            {
                try
                {
                    if (vb == null)
                        vb = new VertexBuffer(graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration, vertexCount + 2);

                    if (vertexCount + 2 > vb.VertexCount)
                        vb.Resize(vertexCount + 2);

                    vb.SetData(vertices.ToArray());

                    this.vertexCount = vertexCount;
                    this.indexCount = indexCount;
                }
                catch (Exception ex)
                {
                    var foo = ex;
                }
            }
            else
            {
                this.vertexCount = 0;
                this.indexCount = 0;
            }


            lock (this)
            {
                loaded = true;
            }

            NeedsUpdate = chunk.ChangeCounter != ChangeStart || chunk != this.chunk;
            return !NeedsUpdate;
        }

        public void Dispose()
        {
            if (vb != null)
            {
                vb.Dispose();
                vb = null;
            }

            if (chunk != null)
            {
                chunk.Changed -= OnChunkChanged;
                chunk = null;
            }

            //if (ib != null)
            //{
            //    ib.Dispose();
            //    ib = null;
            //}
        }
    }
}
