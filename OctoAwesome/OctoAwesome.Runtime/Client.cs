using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Service-Klasse zur Übertragung von Spieldaten zwischen Client und Server.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Client : IConnection
    {
        /// <summary>
        /// Der Callback für den Client zur bidirektionalen Kommunikation.
        /// </summary>
        public IConnectionCallback Callback { get; private set; }

        /// <summary>
        /// Die Guid des Clients.
        /// </summary>
        public Guid ConnectionId { get; private set; }

        /// <summary>
        /// Der Spielername des Clients.
        /// </summary>
        public string Playername { get; private set; }

        /// <summary>
        /// Der ActorHost dieses Clients.
        /// </summary>
        public ActorHost ActorHost { get; private set; }

        /// <summary>
        /// Alle abonnierten Chunks.
        /// </summary>
        public List<PlanetIndex3> SubscripedChunks { get; private set; }

        /// <summary>
        /// Erzeugt eine Instanz der Klasse Client.
        /// </summary>
        public Client()
        {
            Callback = OperationContext.Current.GetCallbackChannel<IConnectionCallback>();
            ConnectionId = Guid.NewGuid();
            SubscripedChunks = new List<PlanetIndex3>();
        }

        /// <summary>
        /// Setzt den ActorHost für diesen Client.
        /// </summary>
        /// <param name="host">Der neue ActorHost.</param>
        public void SetActorHost(ActorHost host)
        {
            ActorHost = host;

            host.OnPositionChanged += (p) =>
            {
                try
                {
                    Callback.SetPosition(p.Planet, p.GlobalBlockIndex, p.BlockPosition);
                }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnRadiusChanged += (v) =>
            {
                try
                {
                    Callback.SetRadius(v);
                }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnAngleChanged += (v) =>
            {
                try
                {
                    Callback.SetAngle(v);
                }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnHeightChanged += (v) =>
            {
                try
                {
                    Callback.SetHeight(v);
                }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnOnGroundChanged += (v) =>
            {
                try
                {
                    Callback.SetOnGround(v);
                }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnFlyModeChanged += (v) =>
            {
                try
                {
                    Callback.SetFlyMode(v);
                }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnTiltChanged += (v) =>
            {
                try
                {
                    Callback.SetTilt(v);
                }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnMoveChanged += (v) =>
            {
                try
                {
                    Callback.SetMove(v);
                }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnHeadChanged += (v) =>
            {
                try
                {
                    Callback.SetHead(v);
                }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
        }

        /// <summary>
        /// Verbindet den Player mit dem Server. Muss vor der weiteren Benutzung der Verbindung aufgerufen werden.
        /// </summary>
        /// <param name="playername">Der Name des Spielers.</param>
        /// <returns></returns>
        [OperationBehavior]
        public ConnectResult Connect(string playername)
        {
            Playername = playername;
            Server.Instance.Join(this);

            ConnectResult result = new ConnectResult
            {
                Id = ConnectionId,
                OtherClients = Server.Instance.Clients
            };
            return result;
        }

        /// <summary>
        /// Verlässt den Server.
        /// </summary>
        /// <param name="reason">Der Grund des Verlassens.</param>
        [OperationBehavior]
        public void Disconnect(string reason)
        {
            Server.Instance.Leave(this);
            //Callback.Disconnect(reason);
        }

        /// <summary>
        /// Lässt den Spieler hüpfen.
        /// </summary>
        [OperationBehavior]
        public void Jump()
        {
            ActorHost.Jump();
        }

        /// <summary>
        /// Setzt den Flugmodus.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationBehavior]
        public void SetFlyMode(bool value)
        {
            ActorHost.FlyMode = value;
        }

        /// <summary>
        /// Setzt die Kopfbewegung.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationBehavior]
        public void SetHead(Vector2 value)
        {
            ActorHost.Head = value;
        }

        /// <summary>
        /// Setzt den Bewegungsvektor.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationBehavior]
        public void SetMove(Vector2 value)
        {
            ActorHost.Move = value;
        }

        /// <summary>
        /// Setzt den Sprintmodus.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationBehavior]
        public void SetSprint(bool value)
        {
            ActorHost.Sprint = value;
        }

        /// <summary>
        /// Sezt den ?.
        /// </summary>
        /// <param name="value">Der neue Wert.</param>
        [OperationBehavior]
        public void SetCrouch(bool value)
        {
            ActorHost.Crouch = value;
        }

        /// <summary>
        /// Wendet ein Werkzeug auf einen Block an.
        /// </summary>
        /// <param name="blockIndex">Position des Blocks.</param>
        /// <param name="definitionName">Name der Definition des Werkzeugs.</param>
        /// <param name="orientation">Orientierung.</param>
        [OperationBehavior]
        public void Apply(Index3 blockIndex, string definitionName, OrientationFlags orientation)
        {
            var definition =
                DefinitionManager.GetBlockDefinitions()
                    .SingleOrDefault(d => d.GetType().FullName.Equals(definitionName));
            if (definition == null) return;

            InventorySlot slot = new InventorySlot() {Amount = 2, Definition = definition};

            ActorHost.Apply(blockIndex, slot, orientation);
        }

        /// <summary>
        /// Interagiert mit einem Block.
        /// </summary>
        /// <param name="blockIndex">Die Position des Blocks.</param>
        [OperationBehavior]
        public void Interact(Index3 blockIndex)
        {
            ActorHost.Interact(blockIndex);
        }

        /// <summary>
        /// Deabonniert einen Chunk.
        /// </summary>
        /// <param name="index">Position des Chunks.</param>
        [OperationBehavior]
        public void UnsubscribeChunk(PlanetIndex3 index)
        {
            Server.Instance.UnsubscribeChunk(ConnectionId, index);
        }
    }
}