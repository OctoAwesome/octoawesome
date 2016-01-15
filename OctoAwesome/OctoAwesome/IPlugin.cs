using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IPlugin
    {
        void OnLoaded(ActionManager manager);
    }
}
