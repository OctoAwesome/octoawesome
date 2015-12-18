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

        /// <summary>
        /// Informiert den Client über das Entfernen eines einzelnen Blocks
        /// </summary>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendBlockRemove(PlanetIndex3 chunkIndex, Index3 blockIndex);

        /// <summary>
        /// Informiert den Client über das Einfügen eines Blocks
        /// </summary>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendBlockInsert(PlanetIndex3 chunkIndex, Index3 blockIndex, string fullName, int metaData);

        /// <summary>
        /// Informiert den Player über das einfügen einer Entität (Items, player,...)
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <param name="id"></param>
        /// <param name="fullName"></param>
        /// <param name="data"></param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendEntityInsert(PlanetIndex3 index, Guid id, string assemblyName, string fullName, byte[] data);

        /// <summary>
        /// Informiert den Player über das entfernen einer Entität
        /// </summary>
        /// <param name="id"></param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendEntityRemove(Guid id);

        /// <summary>
        /// Informiert den Player über den Wechsel einer Entität in einen anderen Chunk
        /// </summary>
        /// <param name="id"></param>
        /// <param name="planet"></param>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendEntityMove(Guid id, PlanetIndex3 index);

        /// <summary>
        /// Informiert den Player über veränderte Entitäten-Daten
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendEntityUpdate(Guid id, byte[] data);

        #endregion
    }
}
