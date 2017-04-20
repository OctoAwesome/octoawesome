namespace OctoAwesome
{
    /// <summary>
    /// Repräsentiert einen Karten-Abschnitt innerhalb des Planeten.
    /// </summary>
    public sealed class Chunk : IChunk
    {
        #region public properties

        /// <summary>
        /// Zweierpotenz der Chunkgrösse. Ausserdem gibt es die Anzahl Bits an,
        /// die die X-Koordinate im Array <see cref="Blocks"/> verwendet.
        /// </summary>
        public const int LimitX = 5;
        /// <summary>
        /// Zweierpotenz der Chunkgrösse. Ausserdem gibt es die Anzahl Bits an,
        /// die die Y-Koordinate im Array <see cref="Blocks"/> verwendet.
        /// </summary>
        public const int LimitY = 5;
        /// <summary>
        /// Zweierpotenz der Chunkgrösse. Ausserdem gibt es die Anzahl Bits an,
        /// die die Z-Koordinate im Array <see cref="Blocks"/> verwendet.
        /// </summary>
        public const int LimitZ = 5;

        /// <summary>
        /// Größe eines Chunks in Blocks in X-Richtung.
        /// </summary>
        public const int CHUNKSIZE_X = 1 << LimitX;

        /// <summary>
        /// Größe eines Chunks in Blocks in Y-Richtung.
        /// </summary>
        public const int CHUNKSIZE_Y = 1 << LimitY;

        /// <summary>
        /// Größe eines Chunks in Blocks in Z-Richtung.
        /// </summary>
        public const int CHUNKSIZE_Z = 1 << LimitZ;

        /// <summary>
        /// Grösse eines Chunk als <see cref="Index3"/>
        /// </summary>
        public static readonly Index3 CHUNKSIZE = new Index3(CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z);

        /// <summary>
        /// Array, das alle Blöcke eines Chunks enthält. Jeder eintrag entspricht einer Block-ID.
        /// Der Index ist derselbe wie bei <see cref="MetaData"/> und <see cref="Resources"/>.
        /// </summary>
        public ushort[] Blocks
        {
            get { return this.GetBlocksAsArray(); }
        }

        /// <summary>
        /// Default Block
        /// WARNING: on set, reset the octree
        /// </summary>
        public ushort DefaultBlock
        {
            get
            {
                return this.defaultBlock;
            }
            set
            {
                this.defaultBlock = value;
                this.blockTree.ResetWithValue(value);
            }
        }

        /// <summary>
        /// Array, das die Metadaten zu den Blöcken eines Chunks enthält.
        /// Der Index ist derselbe wie bei <see cref="Blocks"/> und <see cref="Resources"/>.
        /// </summary>
        public int[] MetaData
        {
            get { return this.GetMetasAsArray(); }
        }

        /// <summary>
        /// Verzweigtes Array, das die Ressourcen zu den Blöcken eines Chunks enthält.
        /// Der Index der ersten Dimension ist derselbe wie bei <see cref="Blocks"/> und <see cref="Resources"/>.
        /// </summary>
        public ushort[][] Resources
        {
            get { return this.GetResourcesAsArray(); }
        }

        /// <summary>
        /// Chunk Index innerhalb des Planeten.
        /// </summary>
        public Index3 Index { get; private set; }

        /// <summary>
        /// Referenz auf den Planeten.
        /// </summary>
        public int Planet { get; private set; }

        /// <summary>
        /// Ein Counter, der jede Veränderung durch SetBlock gemacht wird. Kann 
        /// dazu verwendet werden herauszufinden, ob es Änderungen gab.<para/>
        /// </summary>
        public int ChangeCounter { get; set; }

        #endregion

        #region private fields

        private Octree<ushort> blockTree;
        private Octree<int> metaTree;
        private Octree<ushort[]> resourceTree;
        private ushort defaultBlock;

        #endregion

        #region constructors

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse Chunk
        /// </summary>
        /// <param name="pos">Position des Chunks</param>
        /// <param name="planet">Index des Planeten</param>
        public Chunk(Index3 pos, int planet)
        {
            Index = pos;
            Planet = planet;
            ChangeCounter = 0;
            this.blockTree = Octree<ushort>.CreateFromChunk(this);
            this.metaTree = Octree<int>.CreateFromChunk(this);
            this.resourceTree = Octree<ushort[]>.CreateFromChunk(this);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunkgs</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
        public ushort GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Block-ID der angegebenen Koordinate</returns>
        public ushort GetBlock(int x, int y, int z)
        {
            var index = GetLocalPosition(x, y, z);
            return this.blockTree.Get(index);
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="block">Die neue Block-ID.</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        public void SetBlock(Index3 index, ushort block, int meta = 0)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="block">Die neue Block-ID</param>
        /// <param name="meta">(Optional) Die Metadaten des Blocks</param>
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            var index = GetLocalPosition(x, y, z);
            this.blockTree.Add(index, block);
            if (meta != 0)
            {
                this.metaTree.Add(index, meta);
            }
            if (this.blockTree.IsDirty)
            {
                ChangeCounter++;
                this.blockTree.IsDirty = false;
            }
        }

        /// <summary>
        /// Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
        public int GetBlockMeta(int x, int y, int z)
        {
            var index = GetLocalPosition(x, y, z);
            return this.metaTree.Get(index);
        }

        /// <summary>
        /// Ändert die Metadaten des Blockes an der angegebenen Koordinate. 
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">Die neuen Metadaten</param>
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            var index = GetLocalPosition(x, y, z);
            this.metaTree.Add(index, meta);
            if (this.metaTree.IsDirty)
            {
                ChangeCounter++;
                this.metaTree.IsDirty = false;
            }
        }

        /// <summary>
        /// Liefert alle Ressourcen im Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Ein Array aller Ressourcen des Blocks</returns>
        public ushort[] GetBlockResources(int x, int y, int z)
        {
            var index = GetLocalPosition(x, y, z);
            return this.resourceTree.Get(index);
        }

        /// <summary>
        /// Ändert die Ressourcen des Blocks an der angegebenen Koordinate
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="resources">Ein <see cref="ushort"/>-Array, das alle Ressourcen enthält</param>
        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            var index = GetLocalPosition(x, y, z);
            this.resourceTree.Add(index, resources);
            if (this.resourceTree.IsDirty)
            {
                ChangeCounter++;
                this.resourceTree.IsDirty = false;
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Liefert den Index des Blocks im abgeflachten Block-Array der angegebenen 3D-Koordinate zurück. Sollte die Koordinate ausserhalb
        /// der Chunkgrösse liegen, wird dies gewrapt.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate</param>
        /// <param name="y">Y-Anteil der Koordinate</param>
        /// <param name="z">Z-Anteil der Koordinate</param>
        /// <returns>Index innerhalb des flachen Arrays</returns>
        private int GetFlatIndex(int x, int y, int z)
        {
            return ((z & (CHUNKSIZE_Z - 1)) << (LimitX + LimitY))
                   | ((y & (CHUNKSIZE_Y - 1)) << LimitX)
                   | ((x & (CHUNKSIZE_X - 1)));
        }

        /// <summary>
        /// Sicherstellen, dass Koordinaten lokal ist
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private Index3 GetLocalPosition(int x, int y, int z)
        {
            var index = new Index3(
                x & 0x1f,
                y & 0x1f,
                z & 0x1f
            );
            return index;
        }

        /// <summary>
        /// Gibt die Blockdaten als array zurück
        /// TODO: Serialize umstellen
        /// </summary>
        /// <returns></returns>
        private ushort[] GetBlocksAsArray()
        {
            var array = new ushort[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];
            for (int x = 0; x < CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < CHUNKSIZE_Y; y++)
                {
                    for (int z = 0; z < CHUNKSIZE_Z; z++)
                    {
                        var index = GetFlatIndex(x, y, z);
                        array[index] = GetBlock(x, y, z);
                    }
                }
            }
            return array;
        }

        /// <summary>
        /// Gibt die Metadaten als array zurück
        /// TODO: Serialize umstellen
        /// </summary>
        /// <returns></returns>
        private int[] GetMetasAsArray()
        {
            var array = new int[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];
            for (int x = 0; x < CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < CHUNKSIZE_Y; y++)
                {
                    for (int z = 0; z < CHUNKSIZE_Z; z++)
                    {
                        var index = GetFlatIndex(x, y, z);
                        array[index] = GetBlockMeta(x, y, z);
                    }
                }
            }
            return array;
        }

        /// <summary>
        /// Gibt die Blockdaten als array zurück
        /// TODO: Als Octree Sinnvoll? Serialize umstellen
        /// </summary>
        /// <returns></returns>
        private ushort[][] GetResourcesAsArray()
        {
            var array = new ushort[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z][];
            for (int x = 0; x < CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < CHUNKSIZE_Y; y++)
                {
                    for (int z = 0; z < CHUNKSIZE_Z; z++)
                    {
                        var index = GetFlatIndex(x, y, z);
                        array[index] = GetBlockResources(x, y, z);
                    }
                }
            }
            return array;
        }
        #endregion
    }
}
