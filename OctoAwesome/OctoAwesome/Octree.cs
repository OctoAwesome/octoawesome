namespace OctoAwesome
{
    /// <summary>
    /// Für Chunks optimierter Octree
    /// </summary>
    /// <typeparam name="T">Type der Daten die gespeichert werden sollen</typeparam>
    public class Octree<T>
    {
        #region public properties

        /// <summary>
        /// Bounding Box des Octrees
        /// </summary>
        public OctreeBounds Box { get { return box; } }

        /// <summary>
        /// Erster OctreeNode
        /// </summary>
        public OctreeNode<T> Root { get { return root; } }

        /// <summary>
        /// Inhalte wurden geändert
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Octree hat Unterelemente
        /// </summary>
        public bool HasChildren { get { return root.HasChildren; } }

        /// <summary>
        /// Maximale Tiefe des Octree. Ist immer zwischen 0 und 7
        /// </summary>
        public byte Depth { get { return depth; } }

        #endregion

        #region protected fields

        protected OctreeBounds box;

        protected readonly byte depth;

        protected OctreeNode<T> root;

        protected IChunk chunk;

        #endregion

        #region constructors

        /// <summary>
        /// Erzeugt einen neuen leeren octree
        /// </summary>
        /// <param name="box">Die Box des octrees, MUSS quatratisch sein</param>
        /// <param name="depth">Tiefe des octrees: Maximaler wert ist 7 = 256 Blöcke pro Seite</param>
        /// <param name="startValue">Start value des root node</param>
        public Octree(OctreeBounds box, byte depth, T startValue = default(T))
        {
            this.box = box;
            this.depth = depth <= 7 ? depth : (byte)7;
            this.root = new OctreeNode<T>(this, box, 0, startValue);
            this.IsDirty = false;
        }

        /// <summary>
        /// Erzeugt einen neuen octree auf basis eines existierenden Chunks.
        /// </summary>
        /// <param name="chunk">Der Chunk</param>
        /// <returns>Der Octree</returns>
        public static Octree<T> CreateFromChunk(IChunk chunk)
        {
            var octree = new Octree<T>(new OctreeBounds(Byte3.Zero, Chunk.CHUNKSIZE - Byte3.One), Chunk.LimitX);
            octree.chunk = chunk;
            return octree;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Resetted den Oktree mit dem angegeben Wert
        /// </summary>
        /// <param name="value"></param>
        public void ResetWithValue(T value)
        {
            this.root.ResetWithValue(value);
        }

        /// <summary>
        /// Fügt einen Wert an der angegebenen Position hinzu
        /// </summary>
        /// <param name="pos">Die Position</param>
        /// <param name="value">Der Wert</param>
        public void Add(Index3 pos, T value)
        {
            this.root.Add(pos, value);
        }

        /// <summary>
        /// Fügt einen Wert an der angegebenen Position hinzu
        /// </summary>
        /// <param name="pos">Die Position</param>
        /// <param name="value">Der Wert</param>
        public void Add(Byte3 pos, T value)
        {
            this.root.Add(pos, value);
        }

        /// <summary>
        /// Löscht ein Wert an der angegebenen Position
        /// </summary>
        /// <param name="pos"></param>
        public void Remove(Index3 pos)
        {
            this.root.Remove(pos);
        }

        /// <summary>
        /// Gibt den Wert an der angegebenen Position zurück
        /// </summary>
        /// <param name="pos">Die Position</param>
        /// <returns>Der Wert</returns>
        public T Get(Index3 pos)
        {
            return this.root.Get(pos);
        }

        /// <summary>
        /// Gibt den Wert an der angegebenen Position zurück
        /// </summary>
        /// <param name="pos">Die Position</param>
        /// <returns>Der Wert</returns>
        public T Get(Byte3 pos)
        {
            return this.root.Get(pos);
        }

        public override string ToString()
        {
            return "Has schildrens" + this.HasChildren.ToString() + "; Min: " + this.box.Min.ToString();
        }
        #endregion
    }
}
