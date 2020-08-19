using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Definitions.Items
{
    public class Hand : Item
    {
        public Hand(HandDefinition handDefinition) : base(handDefinition)
        {

        }
        public override void Hit(IItem item)
        {
        }
    }
}
