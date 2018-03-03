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
        private int vertexCount;
        private int indexCount;
        private ILocalChunkCache _manager;

        private readonly SceneControl _sceneControl;
        private IDefinitionManager definitionManager;

        /// <summary>
        /// Adresse des aktuellen Chunks
        /// </summary>
        public Index3? ChunkPosition
        {
            get
            {
                return _chunkPosition;
            }
            private set
            {
                _chunkPosition = value;
                NeedsUpdate = value != null;
            }
        }

        public ChunkRenderer(SceneControl sceneControl, IDefinitionManager definitionManager, Effect simpleShader, GraphicsDevice graphicsDevice, Matrix projection, Texture2DArray textures)
        {
            _sceneControl = sceneControl;
            this.definitionManager = definitionManager;
            this.graphicsDevice = graphicsDevice;
            this.textures = textures;

            simple = simpleShader;
            GenerateIndexBuffer();
        }

        public void SetChunk(ILocalChunkCache manager, int x, int y, int z)
        {
            var newPosition = new Index3(x, y, z);

            if (_manager == manager && newPosition == ChunkPosition)
            {
                NeedsUpdate = !loaded;
                return;
            }

            _manager = manager;
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
            _sceneControl.Enqueue(this);
        }
        
        public void Draw(Matrix view, Matrix projection, Index3 shift)
        {
            if (!loaded)
                return;

            Matrix worldViewProj = projection * view * Matrix.CreateTranslation(
                shift.X * Chunk.CHUNKSIZE_X,
                shift.Y * Chunk.CHUNKSIZE_Y,
                shift.Z * Chunk.CHUNKSIZE_Z);

            simple.Parameters["WorldViewProj"].SetValue(worldViewProj);
            simple.Parameters["BlockTextures"].SetValue(textures);

            simple.Parameters["AmbientIntensity"].SetValue(0.4f);
            simple.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());

            lock (this)
            {
                if (vb == null)
                    return;
                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                graphicsDevice.VertexBuffer = vb;
                graphicsDevice.IndexBuffer = ib;

                foreach (var pass in simple.CurrentTechnique.Passes.PassesList)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, vertexCount, 0, indexCount / 3);
                }
            }
        }
        private object ibLock = new object();
        private Index3? _chunkPosition;

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
                this.chunk = _manager.GetChunk(ChunkPosition.Value);
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
            List<VertexPositionNormalTextureLight> vertices = new List<VertexPositionNormalTextureLight>(4096);
            List<int> index = new List<int>();
            int textureColumns = textures.Width / SceneControl.TEXTURESIZE;
            float textureWidth = 1f / textureColumns;
            float texelSize = 1f / SceneControl.TEXTURESIZE;
            float textureSizeGap = texelSize;
            float textureGap = texelSize / 2;
            // BlockTypes sammlen
            Dictionary<IBlockDefinition, int> textureOffsets = new Dictionary<IBlockDefinition, int>();
            // Dictionary<Type, BlockDefinition> definitionMapping = new Dictionary<Type, BlockDefinition>();
            int definitionIndex = 0;
            foreach (var definition in definitionManager.GetBlockDefinitions())
            {
                int textureCount = definition.Textures.Count();
                textureOffsets.Add(definition, definitionIndex);
                // definitionMapping.Add(definition.GetBlockType(), definition);
                definitionIndex += textureCount;
            }

            Vector2[] uvOffsets = new[] 
            {
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

                        IBlockDefinition blockDefinition = definitionManager.GetDefinitionByIndex<IBlockDefinition>(block);
                        if (blockDefinition == null)
                            continue;

                        if (!textureOffsets.TryGetValue(blockDefinition, out int textureIndex))
                            continue;

                        // Textur-Koordinate "berechnen"
                        Vector2 textureOffset = new Vector2();
                        //Vector2 textureSize = new Vector2(textureWidth - textureSizeGap, textureWidth - textureSizeGap);

                        ushort topBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z + 1));
                        IBlockDefinition topBlockDefintion = definitionManager.GetDefinitionByIndex<IBlockDefinition>(topBlock);

                        // Top
                        if (topBlock == 0 || (!topBlockDefintion.IsSolidWall(Wall.Bottom) && topBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Top,_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Top,_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Top, _manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top,_manager, x, y, z)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort bottomBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z - 1));
                        IBlockDefinition bottomBlockDefintion = definitionManager.GetDefinitionByIndex<IBlockDefinition>(bottomBlock);

                        // Unten
                        if (bottomBlock == 0 || (!bottomBlockDefintion.IsSolidWall (Wall.Top) && bottomBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Bottom,_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom,_manager, x, y, z)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort southBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y + 1, z));
                        IBlockDefinition southBlockDefintion = definitionManager.GetDefinitionByIndex<IBlockDefinition>(southBlock);

                        // South
                        if (southBlock == 0 || (!southBlockDefintion.IsSolidWall(Wall.Front) && southBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Front,_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Front,_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Front, _manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front,_manager, x, y, z)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort northBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y - 1, z));
                        IBlockDefinition northBlockDefintion = definitionManager.GetDefinitionByIndex<IBlockDefinition>(northBlock);

                        // North
                        if (northBlock == 0 || (!northBlockDefintion.IsSolidWall (Wall.Back) && northBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Back,_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Back,_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Back, _manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back,_manager, x, y, z)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort westBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x - 1, y, z));
                        IBlockDefinition westBlockDefintion = definitionManager.GetDefinitionByIndex<IBlockDefinition>(westBlock);

                        // West
                        if (westBlock == 0 || (!westBlockDefintion.IsSolidWall(Wall.Right) && westBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Left,_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Left,_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);


                            int rotation = -blockDefinition.GetTextureRotation(Wall.Left, _manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left,_manager, x, y, z)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort eastBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x + 1, y, z));
                        IBlockDefinition eastBlockDefintion = definitionManager.GetDefinitionByIndex<IBlockDefinition>(eastBlock);

                        // Ost
                        if (eastBlock == 0 || (!eastBlockDefintion.IsSolidWall(Wall.Left) && eastBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Right,_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Right,_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);


                            int rotation = -blockDefinition.GetTextureRotation(Wall.Right,_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right,_manager, x, y, z)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right,_manager, x, y, z)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }
                    }
                }
            }

            vertexCount = vertices.Count;
            indexCount = index.Count;
            

            if (vertexCount > 0)
            {
                try
                {
                    if (vb == null || ib == null)
                    {
                        vb = new VertexBuffer(graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration, vertexCount + 2);
                        //ib = new IndexBuffer(graphicsDevice, DrawElementsType.UnsignedInt, indexCount);
                    }
                    if (vertexCount + 2 > vb.VertexCount)
                        vb.Resize(vertexCount + 2);
                    //vb2 = new VertexBuffer(graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration, vertexCount+2);//TODO: why do I need more vertices?
                    vb.SetData(vertices.ToArray());

                    //if (indexCount > ib.IndexCount)
                    //    ib.Resize(indexCount);
                    //ib.SetData<int>(index.ToArray());
                }
                catch (Exception ex)
                {
                    var foo = ex;
                }
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
