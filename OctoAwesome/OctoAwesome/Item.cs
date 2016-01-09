using System.Collections.Generic;

namespace OctoAwesome
{
    public abstract class Item : IItem
    {
        public int Condition { get; set; }

        public Coordinate? Position { get; set; }

        public List<IResource> Resources { get; private set; }

        public Item()
        {
            Resources = new List<IResource>();
            Condition = 99;
        }

        public abstract void Hit(IItem item);
    }
}