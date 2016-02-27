using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Servicevertrag zur Übertragung von Spieldaten zwischen Server und Client.
    /// </summary>
    public interface IConnectionCallback
    {
        #region Player Controlling

        /// <summary>
        /// Informiert den Client darüber, dass er vom Server getrennt wurde.
        /// </summary>
        /// <param name="reason">Der Grund dafür.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Disconnect(string reason);

        /// <summary>
        /// Informiert den Client darüber, dass seine Position verändert wurde.
        /// </summary>
        /// <param name="planet">Die Id des Planeten.</param>
        /// <param name="globalPosition">Die Position des Blocks.</param>
        /// <param name="blockPosition">Die Position innerhalb des Blocks.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetPosition(int planet, Index3 globalPosition, Vector3 blockPosition);

        /// <summary>
        /// Informiert den Client darüber, dass sein Winkel verändert wurde.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetAngle(float value);

        /// <summary>
        /// Informiert den Client, dass der Status des Flugmodus verändert wurde.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetFlyMode(bool value);

        /// <summary>
        /// Informiert den Client, dass sein Kopfbewegungsvektor verändert wurde.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetHead(Vector2 value);

        /// <summary>
        /// Informiert den Client, dass seine Höhe geändert wurde.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetHeight(float value);

        /// <summary>
        /// Informiert den Client, dass sein Bewegungsvektor geändert wurde.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetMove(Vector2 value);

        /// <summary>
        /// Informiert den Client, dass sein Stand auf dem Boden geändert wurde.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetOnGround(bool value);

        /// <summary>
        /// Informiert den Client, dass sein Radius verändert wurde.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetRadius(float value);

        /// <summary>
        /// Informiert den Client, dass seine Kopfposition verändert wurde.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetTilt(float value);

        #endregion

        #region Player Management

        /// <summary>
        /// Informiert den Client über einen neuen Spieler.
        /// </summary>
        /// <param name="client">Informationen zum neuen Spieler.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendPlayerJoin(ClientInfo client);

        /// <summary>
        /// Informiert den Client über das Verlassen eines Spielers.
        /// </summary>
        /// <param name="client">Die Guid des alten Spielers.</param>
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
        /// <param name="id">Die Guid, über die die Entität identifiziert werden kann</param>
        /// <param name="fullName">Voller name des Typs der Entität</param>
        /// <param name="data">Daten der Entität</param>
        /// <param name="index">Posistion der neuen Entität</param>
        /// <param name="assemblyName">Name der Assembly der Entität.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendEntityInsert(PlanetIndex3 index, Guid id, string assemblyName, string fullName, byte[] data);

        /// <summary>
        /// Informiert den Player über das entfernen einer Entität
        /// </summary>
        /// <param name="id">Die Guid, über die die Entität identifiziert werden kann</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendEntityRemove(Guid id);

        /// <summary>
        /// Informiert den Player über den Wechsel einer Entität in einen anderen Chunk
        /// </summary>
        /// <param name="id">Die Guid, über die die Entität identifiziert werden kann</param>
        /// <param name="index">Die neue Position</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendEntityMove(Guid id, PlanetIndex3 index);

        /// <summary>
        /// Informiert den Player über veränderte Entitäten-Daten
        /// </summary>
        /// <param name="id">Die Guid, über die die Entität identifiziert werden kann</param>
        /// <param name="data">Die neuen Daten</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SendEntityUpdate(Guid id, byte[] data);

        #endregion
    }
}