using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Entities
{
    public interface IControllable
    {
        Coordinate Position { get; }
        IController Controller { get; }
        void Register(IController controller);
        void Reset();
    }
}
