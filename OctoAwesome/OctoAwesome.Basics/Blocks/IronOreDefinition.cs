using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    class IronOreDefinition : IResourceDefinition
    {
        public Bitmap Icon
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return "Iron Ore"; }
        }

        public int StackLimit
        {
            get { return 50; }
        }

        public IResource GetInstance()
        {
            throw new NotImplementedException();
        }

        public Type GetResourceType()
        {
            throw new NotImplementedException();
        }
    }
}