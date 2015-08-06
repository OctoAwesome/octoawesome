using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class Pickaxe : IItem
    {
        public Coordinate? Position { get; set; }

        public IEnumerable<IResource> Resources
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Hit(IItem item)
        {
            throw new NotImplementedException();
        }
    }
}
