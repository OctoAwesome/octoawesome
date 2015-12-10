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
        /// <param name="planet"></param>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <param name="blockX"></param>
        /// <param name="blockY"></param>
        /// <param name="blockZ"></param>
        void SendBlockRemove(int planet, int chunkX, int chunkY, int chunkZ, int blockX, int blockY, int blockZ);

        /// <summary>
        /// Informiert den Client über das Einfügen eines Blocks
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <param name="blockX"></param>
        /// <param name="blockY"></param>
        /// <param name="blockZ"></param>
        /// <param name="fullName"></param>
        /// <param name="metaData"></param>
        void SendBlockInsert(int planet, int chunkX, int chunkY, int chunkZ, int blockX, int blockY, int blockZ, string fullName, int metaData);

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
        void SendEntityInsert(int planet, int chunkX, int chunkY, int chunkZ, Guid id, string fullName, byte[] data);

        /// <summary>
        /// Informiert den Player über das entfernen einer Entität
        /// </summary>
        /// <param name="id"></param>
        void SendEntityRemove(Guid id);

        /// <summary>
        /// Informiert den Player über den Wechsel einer Entität in einen anderen Chunk
        /// </summary>
        /// <param name="id"></param>
        /// <param name="planet"></param>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        void SendEntityMove(Guid id, int planet, int chunkX, int chunkY, int chunkZ);

        /// <summary>
        /// Informiert den Player über veränderte Entitäten-Daten
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        void SendEntityUpdate(Guid id, byte[] data);

        #endregion
    }
}
