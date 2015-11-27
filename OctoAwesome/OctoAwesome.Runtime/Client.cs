using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Runtime
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Client : IClient
    {
        public IClientCallback Callback { get; private set; }

        public Guid ConnectionId { get; private set; }

        public string Playername { get; private set; }

        public IPlayerController ActorHost { get; private set; }

        public Client()
        {
            Callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            ConnectionId = Guid.NewGuid();
        }

        public void SetActorHost(ActorHost host)
        {
            ActorHost = host;

            host.OnPositionChanged += (p) => { Callback.SetPosition(p.Planet, p.GlobalBlockIndex, p.BlockPosition); };
            host.OnRadiusChanged += (v) => { Callback.SetRadius(v); };
            host.OnAngleChanged += (v) => { Callback.SetAngle(v); };
            host.OnHeightChanged += (v) => { Callback.SetHeight(v); };
            host.OnOnGroundChanged += (v) => { Callback.SetOnGround(v); };
            host.OnFlyModeChanged += (v) => { Callback.SetFlyMode(v); };
            host.OnTiltChanged += (v) => { Callback.SetTilt(v); };
            host.OnMoveChanged += (v) => { Callback.SetMove(v); };
            host.OnHeadChanged += (v) => { Callback.SetHead(v); };
        }

        [OperationBehavior]
        public Guid Connect(string playername)
        {
            Playername = playername;
            Server.Instance.Register(this);
            return ConnectionId;
        }

        [OperationBehavior]
        public void Disconnect()
        {
            Server.Instance.Deregister(this);
        }

        [OperationBehavior]
        public void Jump()
        {
            ActorHost.Jump();
        }

        [OperationBehavior]
        public void SetFlyMode(bool value)
        {
            ActorHost.FlyMode = value;
        }

        [OperationBehavior]
        public void SetHead(Vector2 value)
        {
            ActorHost.Head = value;
        }

        [OperationBehavior]
        public void SetMove(Vector2 value)
        {
            ActorHost.Move = value;
        }

        [OperationBehavior]
        public void Apply(Index3 blockIndex, InventorySlot tool, OrientationFlags orientation)
        {
            ActorHost.Apply(blockIndex, tool, orientation);
        }

        [OperationBehavior]
        public void Interact(Index3 blockIndex)
        {
            ActorHost.Interact(blockIndex);
        }
    }
}
