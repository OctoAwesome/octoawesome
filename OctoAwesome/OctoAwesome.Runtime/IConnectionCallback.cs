using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    public interface IConnectionCallback
    {
        #region Player Controlling

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Disconnect(string reason);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetPosition(int planet, Index3 globalPosition, Vector3 blockPosition);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetAngle(float value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetFlyMode(bool value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetHead(Vector2 value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetHeight(float value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetMove(Vector2 value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetOnGround(bool value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetRadius(float value);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetTilt(float value);

        #endregion

        #region Player Management

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendPlayerJoin(ClientInfo client);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendPlayerLeave(Guid client);

        #endregion

        #region Subscription

        void SendBlockRemove();

        void SendBlockInsert();

        void SendEntityInsert();

        void SendEntityRemove();

        void SendEntityUpdate();

        #endregion
    }
}
