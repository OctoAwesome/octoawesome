using OctoAwesome.Client.Controls;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private VertexBuffer vb, vbTransp;
        private int vertexCount, vertexCountTransp;
        private int indexCount, indexCountTransp;
        private int lastReset;
        private ILocalChunkCache _manager;

        /// <summary>
        /// Adresse des aktuellen Chunks
        /// </summary>
        public Index3? ChunkPosition { get; private set; }

        public ChunkRenderer(Effect simpleShader, GraphicsDevice graphicsDevice, Matrix projection,
            Texture2DArray textures)
        {
            this.graphicsDevice = graphicsDevice;
            this.textures = textures;
            this.lastReset = -1;

            simple = simpleShader;
        }

        public void SetChunk(ILocalChunkCache manager, int x, int y, int z)
        {
            var newPosition = new Index3(x, y, z);

            if (_manager == manager && newPosition == ChunkPosition)
                return;

            _manager = manager;
            ChunkPosition = newPosition;

            chunk = null;
            loaded = false;
        }

        public bool NeedUpdate()
        {
            // Kein Chunk selektiert -> kein Update notwendig
            if (!ChunkPosition.HasValue)
                return false;

            // Chunk vollständig geladen aber nicht vorahden -> kein Update
            if (chunk == null)
                return true;

            // Chunk geladen und existient -> nur Update, wenn sich seit dem letzten Reset was verändert hat.
            return chunk.ChangeCounter != lastReset;
        }

        public void Draw(Matrix view, Matrix projection, Index3 shift)
        {
            if (!loaded)
                return;

            Matrix worldView = view * Matrix.CreateTranslation(
                                       shift.X * Chunk.CHUNKSIZE_X,
                                       shift.Y * Chunk.CHUNKSIZE_Y,
                                       shift.Z * Chunk.CHUNKSIZE_Z);
            simple.CurrentTechnique = simple.Techniques["Ambient"];
            simple.Parameters["WorldView"].SetValue(worldView);
            simple.Parameters["Proj"].SetValue(projection );
            simple.Parameters["BlockTextures"].SetValue(textures);

            simple.Parameters["AmbientIntensity"].SetValue(0.4f);
            simple.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());

            lock (this)
            {
                if (vb != null)
                {
                    graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                    graphicsDevice.VertexBuffer = vb;

                    foreach (var pass in simple.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, vertexCount, 0,
                            indexCount / 3);
                    }
                }
            }
        }

        public void DrawTransparent(Matrix view, Matrix projection, Index3 shift)
        {
            if (!loaded)
                return;

            Matrix worldView = view * Matrix.CreateTranslation(
                                       shift.X * Chunk.CHUNKSIZE_X,
                                       shift.Y * Chunk.CHUNKSIZE_Y,
                                       shift.Z * Chunk.CHUNKSIZE_Z);
            simple.CurrentTechnique = simple.Techniques["water"];
            simple.Parameters["WorldView"].SetValue(worldView);
            simple.Parameters["Proj"].SetValue(projection );
            simple.Parameters["BlockTextures"].SetValue(textures);

            simple.Parameters["AmbientIntensity"].SetValue(0.4f);
            simple.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            simple.Parameters["time"].SetValue(System.Environment.TickCount/1000.0f);
            lock (this)
            {
                if (vbTransp != null)
                {
                    graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                    graphicsDevice.VertexBuffer = vbTransp;

                    foreach (var pass in simple.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, vertexCountTransp, 0,
                            indexCountTransp / 3);
                    }
                }
            }
        }

        public void RegenerateVertexBuffer()
        {
            if (!ChunkPosition.HasValue)
                return;

            // Chunk nachladen
            if (chunk == null)
            {
                chunk = _manager.GetChunk(ChunkPosition.Value);
                if (chunk == null)
                {
                    return;
                }
            }

            List<VertexPositionNormalTextureLight> vertices = new List<VertexPositionNormalTextureLight>();
            List<VertexPositionNormalTextureLight> transparentVertices = new List<VertexPositionNormalTextureLight>();
            // BlockTypes sammlen
            Dictionary<IBlockDefinition, int> textureOffsets = new Dictionary<IBlockDefinition, int>();
            // Dictionary<Type, BlockDefinition> definitionMapping = new Dictionary<Type, BlockDefinition>();
            int definitionIndex = 0;
            foreach (var definition in DefinitionManager.Instance.GetBlockDefinitions())
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

                        IBlockDefinition blockDefinition = DefinitionManager.Instance.GetBlockDefinitionByIndex(block);
                        if (blockDefinition == null)
                            continue;

                        int textureIndex;
                        if (!textureOffsets.TryGetValue(blockDefinition, out textureIndex))
                            continue;


                        ushort topBlock = _manager.GetBlock(
                            (ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z + 1));
                        IBlockDefinition topBlockDefintion =
                            DefinitionManager.Instance.GetBlockDefinitionByIndex(topBlock);

                        // Top
                        if (topBlock == 0 || (!topBlockDefintion.IsBottomSolidWall(_manager, x, y, z + 1) &&
                                              topBlock != block))
                        {
                            int rotation = -blockDefinition.GetTopTextureRotation(_manager, x, y, z);
                            var curVertices = blockDefinition.IsTopSolidWall(_manager, x, y, z)
                                ? vertices
                                : transparentVertices;
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 1),
                                new Vector3(0, 0, 1), uvOffsets[(6 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetTopTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 1),
                                new Vector3(0, 0, 1), uvOffsets[(7 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetTopTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 1),
                                new Vector3(0, 0, 1), uvOffsets[(5 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetTopTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 1),
                                new Vector3(0, 0, 1), uvOffsets[(4 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetTopTextureIndex(_manager, x, y, z)), 0));
                        }

                        ushort bottomBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z - 1));
                        IBlockDefinition bottomBlockDefintion =
                            DefinitionManager.Instance.GetBlockDefinitionByIndex(bottomBlock);


                        // Unten
                        if (bottomBlock == 0 || (!bottomBlockDefintion.IsTopSolidWall(_manager, x, y, z - 1) &&
                                                 bottomBlock != block))
                        {
                            int rotation = -blockDefinition.GetBottomTextureRotation(_manager, x, y, z);
                            var curVertices = blockDefinition.IsBottomSolidWall(_manager, x, y, z)
                                ? vertices
                                : transparentVertices;
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 0),
                                new Vector3(0, 0, -1), uvOffsets[(6 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetBottomTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 0),
                                new Vector3(0, 0, -1), uvOffsets[(7 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetBottomTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 0),
                                new Vector3(0, 0, -1), uvOffsets[(5 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetBottomTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 0),
                                new Vector3(0, 0, -1), uvOffsets[(4 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetBottomTextureIndex(_manager, x, y, z)), 0));
                        }

                        ushort southBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y + 1, z));
                        IBlockDefinition southBlockDefintion =
                            DefinitionManager.Instance.GetBlockDefinitionByIndex(southBlock);

                        // South
                        if (southBlock == 0 || (!southBlockDefintion.IsNorthSolidWall(_manager, x, y + 1, z) &&
                                                southBlock != block))
                        {
                            int rotation = -blockDefinition.GetSouthTextureRotation(_manager, x, y, z);
                            var curVertices = blockDefinition.IsSouthSolidWall(_manager, x, y, z)
                                ? vertices
                                : transparentVertices;
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 0),
                                new Vector3(0, 1, 0), uvOffsets[(6 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetSouthTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 0),
                                new Vector3(0, 1, 0), uvOffsets[(7 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetSouthTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 1),
                                new Vector3(0, 1, 0), uvOffsets[(5 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetSouthTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 1),
                                new Vector3(0, 1, 0), uvOffsets[(4 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetSouthTextureIndex(_manager, x, y, z)), 0));
                        }

                        ushort northBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y - 1, z));
                        IBlockDefinition northBlockDefintion =
                            DefinitionManager.Instance.GetBlockDefinitionByIndex(northBlock);

                        // North
                        if (northBlock == 0 || (!northBlockDefintion.IsSouthSolidWall(_manager, x, y - 1, z) &&
                                                northBlock != block))
                        {
                            int rotation = -blockDefinition.GetNorthTextureRotation(_manager, x, y, z);
                            var curVertices = blockDefinition.IsNorthSolidWall(_manager, x, y, z)
                                ? vertices
                                : transparentVertices;
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 1),
                                new Vector3(0, -1, 0), uvOffsets[(4 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetNorthTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 1),
                                new Vector3(0, -1, 0), uvOffsets[(5 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetNorthTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 0),
                                new Vector3(0, -1, 0), uvOffsets[(7 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetNorthTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 0),
                                new Vector3(0, -1, 0), uvOffsets[(6 + rotation) % 4],
                                (byte) (textureIndex +
                                        blockDefinition.GetNorthTextureIndex(_manager, x, y, z)), 0));
                        }

                        ushort westBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x - 1, y, z));
                        IBlockDefinition westBlockDefintion =
                            DefinitionManager.Instance.GetBlockDefinitionByIndex(westBlock);

                        // West
                        if (westBlock == 0 || (!westBlockDefintion.IsEastSolidWall(_manager, x - 1, y, z) &&
                                               westBlock != block))
                        {
                            int rotation = -blockDefinition.GetWestTextureRotation(_manager, x, y, z);
                            var curVertices = blockDefinition.IsWestSolidWall(_manager, x, y, z)
                                ? vertices
                                : transparentVertices;
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 0),
                                new Vector3(-1, 0, 0), uvOffsets[(7 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetWestTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 1),
                                new Vector3(-1, 0, 0), uvOffsets[(4 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetWestTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 0),
                                new Vector3(-1, 0, 0), uvOffsets[(6 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetWestTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 1),
                                new Vector3(-1, 0, 0), uvOffsets[(5 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetWestTextureIndex(_manager, x, y, z)), 0));
                        }

                        ushort eastBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x + 1, y, z));
                        IBlockDefinition eastBlockDefintion =
                            DefinitionManager.Instance.GetBlockDefinitionByIndex(eastBlock);

                        // Ost
                        if (eastBlock == 0 || (!eastBlockDefintion.IsWestSolidWall(_manager, x + 1, y, z) &&
                                               eastBlock != block))
                        {
                            int rotation = -blockDefinition.GetEastTextureRotation(_manager, x, y, z);
                            var curVertices = blockDefinition.IsEastSolidWall(_manager, x, y, z)
                                ? vertices
                                : transparentVertices;
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 1),
                                new Vector3(1, 0, 0), uvOffsets[(5 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetEastTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 0),
                                new Vector3(1, 0, 0), uvOffsets[(6 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetEastTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 1),
                                new Vector3(1, 0, 0), uvOffsets[(4 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetEastTextureIndex(_manager, x, y, z)), 0));
                            curVertices.Add(new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 0),
                                new Vector3(1, 0, 0), uvOffsets[(7 + rotation) % 4],
                                (byte) (textureIndex + blockDefinition.GetEastTextureIndex(_manager, x, y, z)), 0));
                        }
                    }
                }
            }

            vertexCount = vertices.Count;
            indexCount = vertices.Count / 2 * 3;
            vertexCountTransp = transparentVertices.Count;
            indexCountTransp = transparentVertices.Count / 2 * 3;

            if (vertexCount > 0)
            {
                try
                {
                    if (vb == null)
                    {
                        vb = new VertexBuffer(graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration,
                            vertexCount + 2);
                    }
                    if (vertexCount + 2 > vb.VertexCount)
                        vb.Resize(vertexCount + 2);
                    //vb2 = new VertexBuffer(graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration, vertexCount+2);//TODO: why do I need more vertices?
                    vb.SetData<VertexPositionNormalTextureLight>(vertices.ToArray());
                }
                catch (Exception)
                {
                }
            }
            if (vertexCountTransp > 0)
            {
                try
                {
                    if (vbTransp == null)
                    {
                        vbTransp = new VertexBuffer(graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration,
                            vertexCountTransp + 2);
                    }
                    if (vertexCountTransp + 2 > vbTransp.VertexCount)
                        vbTransp.Resize(vertexCountTransp + 2);
                    vbTransp.SetData(transparentVertices.ToArray());
                }
                catch (Exception)
                {
                }
            }
            lock (this)
            {
                loaded = true;
            }

            lastReset = chunk.ChangeCounter;
        }


        public void Dispose()
        {
            if (vb != null)
            {
                vb.Dispose
                    ();
                vb = null;
            }
        }
    }
}