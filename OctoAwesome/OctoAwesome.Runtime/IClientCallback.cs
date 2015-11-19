using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    internal interface IClientCallback
    {
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Relocation(int x, int y, int z);
    }
}
