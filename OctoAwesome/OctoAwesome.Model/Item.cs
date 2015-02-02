using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OctoAwesome.Model
{
    public abstract class Item
    {
        public Vector2 Position { get; set; }
    }
}
