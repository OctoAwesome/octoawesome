using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Node des für Chunks optimierten Octrees
    /// </summary>
    /// <typeparam name="T">Type der Daten die gespeichert werden sollen</typeparam>
    public class OctreeNode<T>
    {
        #region public properties

        /// <summary>
        /// Bounding Box der Node
        /// </summary>
        public OctreeBounds Box { get { return box; } }

        /// <summary>
        /// OctreeNode hat Unterelemente
        /// </summary>
        public bool HasChildren { get { return children != null; } }

        /// <summary>
        /// Liste der child-nodes, hat entweder eine Länge von 8 oder ist null
        /// </summary>
        public IList<OctreeNode<T>> Children { get { return children; } }

        /// <summary>
        /// Aktuelle Tiefe der node
        /// </summary>
        public byte Depth { get { return depth; } }

        #endregion

        #region protected fields

        protected OctreeBounds box;

        protected Octree<T> octree;

        protected readonly byte depth;

        protected T value;

        protected OctreeNode<T>[] children;

        protected OctreeNode<T> parent;

        #endregion

        #region constructors

        /// <summary>
        /// Erstellt einen neuen Node
        /// </summary>
        /// <param name="octree">Der Octree</param>
        /// <param name="box">Bounding Box der Node</param>
        /// <param name="depth">Aktuelle Tiefe des Nodes</param>
        /// <param name="startValue">Wert des Nodes</param>
        public OctreeNode(Octree<T> octree, OctreeBounds box, byte depth, T startValue)
        {
            this.octree = octree;
            this.box = box;
            this.depth = depth;
            this.value = startValue;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Resetted den OktreeNode mit dem angegeben Wert
        /// </summary>
        /// <param name="value"></param>
        public void ResetWithValue(T value)
        {
            if (this.HasChildren)
            {
                this.children = null;
            }
            this.value = value;
        }

        /// <summary>
        /// Fügt einen Wert an der angegebenen Position hinzu
        /// </summary>
        /// <param name="pos">Die Position</param>
        /// <param name="value">Der Wert</param>
        public void Add(Index3 pos, T value)
        {
            this.Add((Byte3)pos, value);
        }

        /// <summary>
        /// Fügt einen Wert an der angegebenen Position hinzu
        /// </summary>
        /// <param name="pos">Die Position</param>
        /// <param name="value">Der Wert</param>
        public void Add(Byte3 pos, T value)
        {
            /*if (this.Box.Contains(pos) == false)
            {
                throw new ArgumentOutOfRangeException("pos", "pos is out of octree node");
            }*/

            //is same value and no childs exits, skip 
            if (!this.HasChildren && value.Equals(this.value)) return;

            //Has not children and the value is differnet, split the node
            if (!this.HasChildren && !value.Equals(this.value))
            {
                this.Split();
            }

            //is pre-last node, set value direct
            if (this.depth == octree.Depth - 1)
            {
                var added = false;
                for (byte i = 0; i < 8; i++)
                {
                    if (pos == this.children[i].box.Min)
                    {
                        this.children[i].value = value;
                        added = true;
                        this.octree.IsDirty = true;
                        break;
                    }
                }
                if (!added)
                    throw new InvalidOperationException("Value is no added on pos " + pos);
                Reduce();
            }
            //Add value to child
            else if (this.depth < octree.Depth - 1)
            {
                for (byte i = 0; i < 8; i++)
                {
                    if (this.children[i].Box.Contains(pos) == true)
                    {
                        this.children[i].Add(pos, value);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Löscht ein Wert an der angegebenen Position
        /// </summary>
        /// <param name="pos"></param>
        public void Remove(Index3 pos)
        {
            //Add a empty block
            this.octree.Add(pos, default(T));
        }

        /// <summary>
        /// Gib den Wert der angegeben Position zurükt.
        /// </summary>
        /// <param name="pos">Die Position</param>
        /// <returns>Der Wert</returns>
        public T Get(Index3 pos)
        {
            return this.Get((Byte3)pos);
        }

        /// <summary>
        /// Gib den Wert der angegeben Position zurükt.
        /// </summary>
        /// <param name="pos">Die Position</param>
        /// <returns>Der Wert</returns>
        public T Get(Byte3 pos)
        {
            /*if (this.Box.Contains(pos) == false)
            {
                //throw new ArgumentOutOfRangeException("pos", "pos is out of octree node");
                return default(T);
            }*/

            if (this.HasChildren)
            {
                for (byte i = 0; i < 8; i++)
                {
                    if (this.depth < octree.Depth - 1)
                    {
                        if (this.children[i].Box.Contains(pos) == true)
                        {
                            return this.children[i].Get(pos);
                        }
                    }
                    else
                    {
                        if (pos == this.children[i].box.Min)
                        {
                            return this.children[i].value;
                        }
                    }
                }
            }
            return this.value;
        }

        /// <summary>
        /// Bereinigt den node, wenn alle Unterelemente identisch sind.
        /// Bei einer Änderung wird die Methode im Elternelement aufgerufen.
        /// </summary>
        public void Reduce()
        {
            if (this.HasChildren != true) return;
            bool isSame = true;
            T childValue = default(T);
            for (byte i = 0; i < 8; i++)
            {
                //Children is not empty
                if (this.children[i].HasChildren)
                {
                    isSame = false;
                    break;
                }
                //get value of first child
                if (i == 0)
                {
                    childValue = this.children[i].value;
                    continue;
                }
                //check is the same value
                if (this.children[i].value.Equals(childValue) == false)
                {
                    isSame = false;
                    break;
                }
            }
            //is all the same, delete childs
            if (isSame)
            {
                this.children = null;
                this.octree.IsDirty = true;
                this.value = childValue;
                if (this.parent != null)
                    this.parent.Reduce();
            }
        }

        public override string ToString()
        {
            return "Has children: " + this.HasChildren.ToString() + "; Value: " + this.value.ToString() + "; Min: " + box.Min;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Teilt den Node
        /// </summary>
        private void Split()
        {
            this.children = new OctreeNode<T>[8];
            var offset = (((box.Max + Byte3.One) - box.Min) / 2);

            for (byte i = 0; i < 8; ++i)
            {
                OctreeBounds newBox;
                if (Box.Max.X - Box.Min.X > 1)
                {
                    Byte3 min;
                    //obere teil
                    if (i == 0)
                        min = new Byte3(box.Min.X, box.Min.Y, (byte)(box.Min.Z + offset.Z));
                    else if (i == 1)
                        min = new Byte3((byte)(box.Min.X + offset.X), box.Min.Y, (byte)(box.Min.Z + offset.Z));
                    else if (i == 2)
                        min = new Byte3((byte)(box.Min.X + offset.X), (byte)(box.Min.Y + offset.Y), (byte)(box.Min.Z + offset.Z));
                    else if (i == 3)
                        min = new Byte3(box.Min.X, (byte)(box.Min.Y + offset.Y), (byte)(box.Min.Z + offset.Z));

                    //untere teil
                    else if (i == 4)
                        min = new Byte3(box.Min.X, box.Min.Y, box.Min.Z);
                    else if (i == 5)
                        min = new Byte3((byte)(box.Min.X + offset.X), box.Min.Y, box.Min.Z);
                    else if (i == 6)
                        min = new Byte3((byte)(box.Min.X + offset.X), (byte)(box.Min.Y + offset.Y), box.Min.Z);
                    else
                        min = new Byte3(box.Min.X, (byte)(box.Min.Y + offset.Y), box.Min.Z);

                    newBox = new OctreeBounds(min, min + (offset - Byte3.One));
                }
                else
                {
                    Byte3 min;
                    //obere teil
                    if (i == 0)
                        min = new Byte3(box.Min.X, box.Min.Y, (byte)(box.Min.Z + 1));
                    else if (i == 1)
                        min = new Byte3((byte)(box.Min.X + 1), box.Min.Y, (byte)(box.Min.Z + 1));
                    else if (i == 2)
                        min = new Byte3((byte)(box.Min.X + 1), (byte)(box.Min.Y + 1), (byte)(box.Min.Z + 1));
                    else if (i == 3)
                        min = new Byte3(box.Min.X, (byte)(box.Min.Y + 1), (byte)(box.Min.Z + 1));

                    //untere teil
                    else if (i == 4)
                        min = new Byte3(box.Min.X, box.Min.Y, box.Min.Z);
                    else if (i == 5)
                        min = new Byte3((byte)(box.Min.X + 1), box.Min.Y, box.Min.Z);
                    else if (i == 6)
                        min = new Byte3((byte)(box.Min.X + 1), (byte)(box.Min.Y + 1), box.Min.Z);
                    else
                        min = new Byte3(box.Min.X, (byte)(box.Min.Y + 1), box.Min.Z);

                    newBox = new OctreeBounds(min, min + Byte3.One);
                }

                children[i] = new OctreeNode<T>(this.octree, newBox, (byte)(this.depth + 1), this.value);
                children[i].parent = this;
            }

        }
        #endregion
    }
}
