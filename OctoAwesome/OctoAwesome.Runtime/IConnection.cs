using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Servicevertrag zur Übertragung von Spieldaten vom Client zum Server.
    /// </summary>
    [ServiceContract(CallbackContract = typeof (IConnectionCallback), SessionMode = SessionMode.Required)]
    public interface IConnection
    {
        #region Connection Management

        /// <summary>
        /// Verbindet den Player mit dem Server. Muss vor der weiteren Benutzung der Verbindung aufgerufen werden.
        /// </summary>
        /// <param name="playername">Der Name des Spielers.</param>
        /// <returns></returns>
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        ConnectResult Connect(string playername);

        /// <summary>
        /// Verlässt den Server.
        /// </summary>
        /// <param name="reason">Der Grund des Verlassens.</param>
        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void Disconnect(string reason);

        #endregion

        #region Player Controlling

        /// <summary>
        /// Lässt den Spieler hüpfen.
        /// </summary>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Jump();

        /// <summary>
        /// Setzt den Flugmodus.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetFlyMode(bool value);

        /// <summary>
        /// Setzt die Kopfbewegung.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetHead(Vector2 value);

        /// <summary>
        /// Setzt den Bewegungsvektor.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetMove(Vector2 value);

        /// <summary>
        /// Setzt den Sprintmodus.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetSprint(bool value);

        /// <summary>
        /// Sezt den ?.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void SetCrouch(bool value);

        /// <summary>
        /// Wendet ein Werkzeug auf einen Block an.
        /// </summary>
        /// <param name="blockIndex">Position des Blocks.</param>
        /// <param name="definitionName">Name der Definition des Werkzeugs.</param>
        /// <param name="orientation">Orientierung.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Apply(Index3 blockIndex, string definitionName, OrientationFlags orientation);

        /// <summary>
        /// Interagiert mit einem Block.
        /// </summary>
        /// <param name="blockIndex">Die Position des Blocks.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Interact(Index3 blockIndex);

        #endregion

        #region Subscription

        /// <summary>
        /// Deabonniert einen Chunk.
        /// </summary>
        /// <param name="index">Position des Chunks.</param>
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void UnsubscribeChunk(PlanetIndex3 index);

        #endregion
    }
}