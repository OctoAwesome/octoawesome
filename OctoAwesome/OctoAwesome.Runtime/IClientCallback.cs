using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    public interface IClientCallback
    {
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Disconnect();

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Relocation(int x, int y, int z);
    }
}
