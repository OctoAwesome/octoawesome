using Microsoft.Xna.Framework;
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
        void SetPosition(Index3 globalPosition, Vector3 blockPosition);
    }
}
