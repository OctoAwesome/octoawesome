using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    [ServiceContract(CallbackContract = typeof (IConnectionCallback), SessionMode = SessionMode.Required)]
    public interface IConnection
    {
        #region Connection Management

        [OperationContract(IsInitiating = true, IsTerminating = false)]
        ConnectResult Connect(string playername);

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void Disconnect(string reason);

        #endregion

        #region Player Controlling

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Jump();

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetFlyMode(bool value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetHead(Vector2 value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetMove(Vector2 value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Apply(Index3 blockIndex, string definitionName, OrientationFlags orientation);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Interact(Index3 blockIndex);

        #endregion

        #region Subscription

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void UnsubscribeChunk(PlanetIndex3 index);

        #endregion
    }
}