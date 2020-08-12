using OctoAwesome.Client.Controls;
using System.Collections.Generic;
using engenious.Graphics;
using System.Linq;
using engenious;
using System;
using System.Windows.Threading;
using System.Threading;
using OctoAwesome.Threading;

namespace OctoAwesome.Client.Components
{
    internal sealed class ChunkRenderer : IDisposable
    {
        private Effect simple;
        private GraphicsDevice graphicsDevice;

        private Texture2DArray textures;
        private readonly Dispatcher dispatcher;

        public static float OverrideLightLevel { get; set; }
        public static bool WireFrame { get; set; }

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
        private static RasterizerState wireFrameState;

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

        public bool DispatchRequired => Thread.CurrentThread.ManagedThreadId != dispatcher.Thread.ManagedThreadId;

        static ChunkRenderer()
        {
            wireFrameState = new RasterizerState() { FillMode = PolygonMode.Line, CullMode = CullMode.CounterClockwise };
            OverrideLightLevel = 0;
            WireFrame = false;
        }

        public ChunkRenderer(SceneControl sceneControl, IDefinitionManager definitionManager, Effect simpleShader, GraphicsDevice graphicsDevice, Matrix projection, Texture2DArray textures)
        {
            _sceneControl = sceneControl;
            this.definitionManager = definitionManager;
            this.graphicsDevice = graphicsDevice;
            this.textures = textures;
            dispatcher = Dispatcher.CurrentDispatcher;
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


        private void OnChunkChanged(IChunk c)
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

            simple.Parameters["OverrideLightLevel"].SetValue(OverrideLightLevel);
            simple.Parameters["WorldViewProj"].SetValue(worldViewProj);
            simple.Parameters["BlockTextures"].SetValue(textures);

            simple.Parameters["AmbientIntensity"].SetValue(0.4f);
            simple.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());

            lock (this)
            {
                if (vb == null)
                    return;

                graphicsDevice.RasterizerState = WireFrame ? wireFrameState : RasterizerState.CullCounterClockwise;
                graphicsDevice.VertexBuffer = vb;
                graphicsDevice.IndexBuffer = ib;

                foreach (var pass in simple.CurrentTechnique.Passes)
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
            lock (ibLock)
            {
                if (ib != null)
                    return;

                ib = new IndexBuffer(graphicsDevice, DrawElementsType.UnsignedInt, Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y * Chunk.CHUNKSIZE_Z * 6 * 6);
                List<int> indices = new List<int>(ib.IndexCount);
                for (int i = 0; i < ib.IndexCount * 2 / 3; i += 4)
                {
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
#if DEBUG
        public bool RegenerateVertexBuffer()
#else
        public unsafe bool RegenerateVertexBuffer()
#endif
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
            List<VertexPositionNormalTextureLight> vertices = new List<VertexPositionNormalTextureLight>();
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
#if DEBUG
            var blocks = new ushort[27];
#else
            var blocks = stackalloc ushort[27];
#endif

            var blockDefinitions = new IBlockDefinition[27];

            for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    {
                        ushort block = chunk.GetBlock(x, y, z);

                        if (block == 0)
                            continue;

                        IBlockDefinition blockDefinition = (IBlockDefinition)definitionManager.GetBlockDefinitionByIndex(block);

                        if (blockDefinition == null)
                            continue;

                        int textureIndex;
                        if (!textureOffsets.TryGetValue(blockDefinition, out textureIndex))
                            continue;

                        for (int zOffset = -1; zOffset <= 1; zOffset++)
                            for (int yOffset = -1; yOffset <= 1; yOffset++)
                                for (int xOffset = -1; xOffset <= 1; xOffset++)
                                {
                                    blocks[GetIndex(zOffset, yOffset, xOffset)] = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x + xOffset, y + yOffset, z + zOffset));
                                    blockDefinitions[GetIndex(zOffset, yOffset, xOffset)] =
                                        (IBlockDefinition)definitionManager.GetBlockDefinitionByIndex(blocks[GetIndex(zOffset, yOffset, xOffset)]);
                                }

                        ushort topBlock = blocks[GetIndex(1, 0, 0)];
                        IBlockDefinition topBlockDefintion = blockDefinitions[GetIndex(1, 0, 0)];
                        ushort bottomBlock = blocks[GetIndex(-1, 0, 0)];
                        IBlockDefinition bottomBlockDefintion = blockDefinitions[GetIndex(-1, 0, 0)];
                        ushort southBlock = blocks[GetIndex(0, 1, 0)];
                        IBlockDefinition southBlockDefintion = blockDefinitions[GetIndex(0, 1, 0)];
                        ushort northBlock = blocks[GetIndex(0, -1, 0)];
                        IBlockDefinition northBlockDefintion = blockDefinitions[GetIndex(0, -1, 0)];
                        ushort westBlock = blocks[GetIndex(0, 0, -1)];
                        IBlockDefinition westBlockDefintion = blockDefinitions[GetIndex(0, 0, -1)];
                        ushort eastBlock = blocks[GetIndex(0, 0, 1)];
                        IBlockDefinition eastBlockDefintion = blockDefinitions[GetIndex(0, 0, 1)];

                        var globalX = x + chunk.Index.X * Chunk.CHUNKSIZE_X;
                        var globalY = y + chunk.Index.Y * Chunk.CHUNKSIZE_Y;
                        var globalZ = z + chunk.Index.Z * Chunk.CHUNKSIZE_Z;

                        // Top
                        if (topBlock == 0 || (!topBlockDefintion.IsSolidWall(Wall.Bottom) && topBlock != block))
                        {

                            var top = (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ));
                            int rotation = -blockDefinition.GetTextureRotation(Wall.Top, _manager, globalX, globalY, globalZ);


                            var valueYZ = VertexAO(blockDefinitions, GetIndex(1, 1, -1), GetIndex(1, 0, -1), Wall.Left, GetIndex(1, 1, 0), Wall.Front);
                            var valueXYZ = VertexAO(blockDefinitions, GetIndex(1, 1, 1), GetIndex(1, 0, 1), Wall.Left, GetIndex(1, 1, 0), Wall.Front);
                            var valueZ = VertexAO(blockDefinitions, GetIndex(1, -1, -1), GetIndex(1, 0, -1), Wall.Left, GetIndex(1, -1, 0), Wall.Front);
                            var valueXZ = VertexAO(blockDefinitions, GetIndex(1, -1, 1), GetIndex(1, 0, 1), Wall.Left, GetIndex(1, -1, 0), Wall.Front);

                            var vertYZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(6 + rotation) % 4],
                                    top,
                                    AmbientToBrightness(valueYZ));
                            var vertXYZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(7 + rotation) % 4],
                                    top,
                                    AmbientToBrightness(valueXYZ));
                            var vertZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(5 + rotation) % 4],
                                    top,
                                    AmbientToBrightness(valueZ));
                            var vertXZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(4 + rotation) % 4],
                                    top,
                                    AmbientToBrightness(valueXZ));

                            if (valueXYZ + valueZ <= valueYZ + valueXZ)
                            {
                                vertices.Add(vertYZ);
                                vertices.Add(vertXYZ);
                                vertices.Add(vertZ);
                                vertices.Add(vertXZ);
                            }
                            else
                            {
                                vertices.Add(vertXYZ);
                                vertices.Add(vertXZ);
                                vertices.Add(vertYZ);
                                vertices.Add(vertZ);
                            }
                        }


                        // Unten
                        if (bottomBlock == 0 || (!bottomBlockDefintion.IsSolidWall(Wall.Top) && bottomBlock != block))
                        {
                            var bottom = (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX, globalY, globalZ));
                            var valueY = VertexAO(blockDefinitions, GetIndex(-1, 1, -1), GetIndex(-1, 0, -1), Wall.Left, GetIndex(-1, 1, 0), Wall.Front);
                            var valueXY = VertexAO(blockDefinitions, GetIndex(-1, 1, 1), GetIndex(-1, 0, 1), Wall.Left, GetIndex(-1, 1, 0), Wall.Front);
                            var value = VertexAO(blockDefinitions, GetIndex(-1, -1, -1), GetIndex(-1, 0, -1), Wall.Left, GetIndex(-1, -1, 0), Wall.Front);
                            var valueX = VertexAO(blockDefinitions, GetIndex(-1, -1, 1), GetIndex(-1, 0, 1), Wall.Left, GetIndex(-1, -1, 0), Wall.Front);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Bottom, _manager, globalX, globalY, globalZ);

                            var vertXY = new VertexPositionNormalTextureLight(
                      new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 0, -1), uvOffsets[(6 + rotation) % 4], bottom, AmbientToBrightness(valueXY));
                            var vertY = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 0, -1), uvOffsets[(7 + rotation) % 4], bottom, AmbientToBrightness(valueY));
                            var vertX = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0), new Vector3(0, 0, -1), uvOffsets[(5 + rotation) % 4], bottom, AmbientToBrightness(valueX));
                            var vert = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0), new Vector3(0, 0, -1), uvOffsets[(4 + rotation) % 4], bottom, AmbientToBrightness(value));

                            if (value + valueXY <= valueY + valueX)
                            {
                                vertices.Add(vertY);
                                vertices.Add(vert);
                                vertices.Add(vertXY);
                                vertices.Add(vertX);
                            }
                            else
                            {
                                vertices.Add(vertXY);
                                vertices.Add(vertY);
                                vertices.Add(vertX);
                                vertices.Add(vert);
                            }
                        }


                        // South
                        if (southBlock == 0 || (!southBlockDefintion.IsSolidWall(Wall.Front) && southBlock != block))
                        {
                            var front = (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX, globalY, globalZ));
                            int rotation = -blockDefinition.GetTextureRotation(Wall.Front, _manager, globalX, globalY, globalZ);

                            var valueY = VertexAO(blockDefinitions, GetIndex(-1, 1, -1), GetIndex(0, 1, -1), Wall.Right, GetIndex(-1, 1, 0), Wall.Front);
                            var valueXY = VertexAO(blockDefinitions, GetIndex(-1, 1, 1), GetIndex(0, 1, 1), Wall.Left, GetIndex(-1, 1, 0), Wall.Front);
                            var valueYZ = VertexAO(blockDefinitions, GetIndex(1, 1, -1), GetIndex(0, 1, -1), Wall.Right, GetIndex(1, 1, 0), Wall.Back);
                            var valueXYZ = VertexAO(blockDefinitions, GetIndex(1, 1, 1), GetIndex(1, 1, 0), Wall.Left, GetIndex(0, 1, 1), Wall.Back);

                            var vertY = new VertexPositionNormalTextureLight(
                         new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 1, 0), uvOffsets[(6 + rotation) % 4], front, AmbientToBrightness(valueY));
                            var vertXY = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 1, 0), uvOffsets[(7 + rotation) % 4], front, AmbientToBrightness(valueXY));
                            var vertYZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1), new Vector3(0, 1, 0), uvOffsets[(5 + rotation) % 4], front, AmbientToBrightness(valueYZ));
                            var vertXYZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1), new Vector3(0, 1, 0), uvOffsets[(4 + rotation) % 4], front, AmbientToBrightness(valueXYZ));


                            if (valueY + valueXYZ >= valueYZ + valueXY)
                            {
                                vertices.Add(vertY);
                                vertices.Add(vertXY);
                                vertices.Add(vertYZ);
                                vertices.Add(vertXYZ);
                            }
                            else
                            {
                                vertices.Add(vertXY);
                                vertices.Add(vertXYZ);
                                vertices.Add(vertY);
                                vertices.Add(vertYZ);
                            }
                        }



                        // North
                        if (northBlock == 0 || (!northBlockDefintion.IsSolidWall(Wall.Back) && northBlock != block))
                        {
                            var back = (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ));
                            int rotation = -blockDefinition.GetTextureRotation(Wall.Back, _manager, globalX, globalY, globalZ);
                            var value = VertexAO(blockDefinitions, GetIndex(-1, -1, -1), GetIndex(0, -1, -1), Wall.Right, GetIndex(-1, -1, 0), Wall.Front);
                            var valueX = VertexAO(blockDefinitions, GetIndex(-1, -1, 1), GetIndex(0, -1, 1), Wall.Left, GetIndex(-1, -1, 0), Wall.Front);
                            var valueZ = VertexAO(blockDefinitions, GetIndex(1, -1, -1), GetIndex(0, -1, -1), Wall.Right, GetIndex(1, -1, 0), Wall.Back);
                            var valueXZ = VertexAO(blockDefinitions, GetIndex(1, -1, 1), GetIndex(1, -1, 0), Wall.Left, GetIndex(0, -1, 1), Wall.Back);

                            var vertZ = new VertexPositionNormalTextureLight(
                        new Vector3(x + 0, y + 0, z + 1), new Vector3(0, -1, 0), uvOffsets[(4 + rotation) % 4], back, AmbientToBrightness(valueZ));
                            var vertXZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1), new Vector3(0, -1, 0), uvOffsets[(5 + rotation) % 4], back, AmbientToBrightness(valueXZ));
                            var vert = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0), new Vector3(0, -1, 0), uvOffsets[(7 + rotation) % 4], back, AmbientToBrightness(value));
                            var vertX = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0), new Vector3(0, -1, 0), uvOffsets[(6 + rotation) % 4], back, AmbientToBrightness(valueX));

                            if (value + valueXZ <= valueZ + valueX)
                            {
                                vertices.Add(vertZ);
                                vertices.Add(vertXZ);
                                vertices.Add(vert);
                                vertices.Add(vertX);
                            }
                            else
                            {
                                vertices.Add(vertXZ);
                                vertices.Add(vertX);
                                vertices.Add(vertZ);
                                vertices.Add(vert);
                            }
                        }


                        // West
                        if (westBlock == 0 || (!westBlockDefintion.IsSolidWall(Wall.Right) && westBlock != block))
                        {
                            var left = (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ));
                            int rotation = -blockDefinition.GetTextureRotation(Wall.Left, _manager, globalX, globalY, globalZ);

                            var valueY = VertexAO(blockDefinitions, GetIndex(-1, 1, -1), GetIndex(0, 1, -1), Wall.Left, GetIndex(-1, 0, -1), Wall.Front);
                            var valueYZ = VertexAO(blockDefinitions, GetIndex(1, 1, -1), GetIndex(1, 0, -1), Wall.Left, GetIndex(0, 1, -1), Wall.Back);
                            var value = VertexAO(blockDefinitions, GetIndex(-1, -1, -1), GetIndex(0, -1, -1), Wall.Right, GetIndex(-1, 0, -1), Wall.Front);
                            var valueZ = VertexAO(blockDefinitions, GetIndex(1, -1, -1), GetIndex(0, -1, -1), Wall.Right, GetIndex(1, 0, -1), Wall.Back);

                            var vertY = new VertexPositionNormalTextureLight(
                       new Vector3(x + 0, y + 1, z + 0), new Vector3(-1, 0, 0), uvOffsets[(7 + rotation) % 4], left, AmbientToBrightness(valueY));
                            var vertYZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1), new Vector3(-1, 0, 0), uvOffsets[(4 + rotation) % 4], left, AmbientToBrightness(valueYZ));
                            var vert = new VertexPositionNormalTextureLight(
                                   new Vector3(x + 0, y + 0, z + 0), new Vector3(-1, 0, 0), uvOffsets[(6 + rotation) % 4], left, AmbientToBrightness(value));
                            var vertZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1), new Vector3(-1, 0, 0), uvOffsets[(5 + rotation) % 4], left, AmbientToBrightness(valueZ));

                            if (value + valueYZ <= valueZ + valueY)
                            {
                                vertices.Add(vertY);
                                vertices.Add(vertYZ);
                                vertices.Add(vert);
                                vertices.Add(vertZ);
                            }
                            else
                            {
                                vertices.Add(vertYZ);
                                vertices.Add(vertZ);
                                vertices.Add(vertY);
                                vertices.Add(vert);
                            }
                        }


                        // Ost
                        if (eastBlock == 0 || (!eastBlockDefintion.IsSolidWall(Wall.Left) && eastBlock != block))
                        {
                            var right = (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX, globalY, globalZ));
                            var valueXY = VertexAO(blockDefinitions, GetIndex(-1, 1, 1), GetIndex(0, 1, 1), Wall.Left, GetIndex(-1, 0, 1), Wall.Front);
                            var valueXYZ = VertexAO(blockDefinitions, GetIndex(1, 1, 1), GetIndex(1, 0, 1), Wall.Left, GetIndex(0, 1, 1), Wall.Back);
                            var valueX = VertexAO(blockDefinitions, GetIndex(-1, -1, 1), GetIndex(0, -1, 1), Wall.Right, GetIndex(-1, 0, 1), Wall.Front);
                            var valueXZ = VertexAO(blockDefinitions, GetIndex(1, -1, 1), GetIndex(0, -1, 1), Wall.Right, GetIndex(1, 0, 1), Wall.Back);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Right, _manager, globalX, globalY, globalZ);

                            var vertXYZ = new VertexPositionNormalTextureLight(
                     new Vector3(x + 1, y + 1, z + 1), new Vector3(1, 0, 0), uvOffsets[(5 + rotation) % 4], right, AmbientToBrightness(valueXYZ));
                            var vertXY = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0), new Vector3(1, 0, 0), uvOffsets[(6 + rotation) % 4], right, AmbientToBrightness(valueXY));
                            var vertXZ = new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1), new Vector3(1, 0, 0), uvOffsets[(4 + rotation) % 4], right, AmbientToBrightness(valueXZ));
                            var vertX = new VertexPositionNormalTextureLight(
                                   new Vector3(x + 1, y + 0, z + 0), new Vector3(1, 0, 0), uvOffsets[(7 + rotation) % 4], right, AmbientToBrightness(valueX));

                            if (valueX + valueXYZ >= valueXZ + valueXY)
                            {
                                vertices.Add(vertXYZ);
                                vertices.Add(vertXY);
                                vertices.Add(vertXZ);
                                vertices.Add(vertX);
                            }
                            else
                            {
                                vertices.Add(vertXY);
                                vertices.Add(vertX);
                                vertices.Add(vertXYZ);
                                vertices.Add(vertXZ);
                            }
                        }
                    }
                }
            }

            vertexCount = vertices.Count;
            indexCount = vertices.Count * 6 / 4;

            if (vertexCount > 0)
            {
                Dispatch(() =>
                {
                    if (vb == null || ib == null)
                    {
                        vb = new VertexBuffer(graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration, vertexCount);
                    }
                    if (vertexCount > vb.VertexCount)
                        vb.Resize(vertexCount);


                    vb.SetData(vertices.ToArray());
                });
            }

            lock (this)
            {
                if (chunk != null && chunk.Index != ChunkPosition)
                {
                    return loaded;
                }

                loaded = true;
                NeedsUpdate |= chunk != this.chunk;
                return !NeedsUpdate;
            }
        }

        private static int VertexAO(int side1, int side2, int corner)
            => ((side1 & side2) ^ 1) * (3 - (side1 + side2 + corner));

        private uint AmbientToBrightness(uint ambient)
            => (0xFFFFFF / 2) + (0xFFFFFF / 6 * ambient);

        private static /*unsafe */uint VertexAO(IBlockDefinition[] blockDefinitions, int cornerIndex, int side1Index, Wall side1Wall, int side2Index, Wall side2Wall)
        {
            var cornerBlock = blockDefinitions[cornerIndex]?.SolidWall ?? 0;
            var side1Def = blockDefinitions[side1Index];
            var side2Def = blockDefinitions[side2Index];
            var side1 = IsSolidWall(side1Wall, side1Def?.SolidWall ?? 0);
            var side2 = IsSolidWall(side2Wall, side2Def?.SolidWall ?? 0);

            return (uint)VertexAO(side1, side2, cornerBlock == 0 ? 0 : 1);
        }

        private static int GetIndex(int zOffset, int yOffset, int xOffset)
            => ((((zOffset + 1) * 3) + yOffset + 1) * 3) + xOffset + 1;

        private static int IsSolidWall(Wall wall, uint solidWall)
            => ((int)solidWall >> (int)wall) & 1;

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

        }

        private void Dispatch(Action action)
        {
            if (DispatchRequired)
                dispatcher.Invoke(action);
            else
                action();
        }
    }
}
