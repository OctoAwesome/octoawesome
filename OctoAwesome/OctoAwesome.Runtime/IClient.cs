using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    [ServiceContract(CallbackContract = typeof(IClientCallback), SessionMode = SessionMode.Required)]
    public interface IClient
    {
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        Guid Connect(string playername);

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void Disconnect();

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Jump();

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void SetFlyMode(bool value);

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void SetHead(Vector2 value);

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void SetMove(Vector2 value);

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void Apply(Index3 blockIndex, InventorySlot tool, OrientationFlags orientation);

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void Interact(Index3 blockIndex);
    }
}
