﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Runtime
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Client : IConnection
    {
        public IConnectionCallback Callback { get; private set; }

        public Guid ConnectionId { get; private set; }

        public string Playername { get; private set; }

        public IPlayerController ActorHost { get; private set; }

        public Client()
        {
            Callback = OperationContext.Current.GetCallbackChannel<IConnectionCallback>();
            ConnectionId = Guid.NewGuid();
        }

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
                try { Callback.SetRadius(v); }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnAngleChanged += (v) =>
            {
                try { Callback.SetAngle(v); }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnHeightChanged += (v) =>
            {
                try { Callback.SetHeight(v); }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnOnGroundChanged += (v) =>
            {
                try { Callback.SetOnGround(v); }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnFlyModeChanged += (v) =>
            {
                try { Callback.SetFlyMode(v); }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnTiltChanged += (v) =>
            {
                try { Callback.SetTilt(v); }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnMoveChanged += (v) =>
            {
                try { Callback.SetMove(v); }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
            host.OnHeadChanged += (v) =>
            {
                try { Callback.SetHead(v); }
                catch (Exception ex)
                {
                    Disconnect(ex.Message);
                }
            };
        }

        [OperationBehavior]
        public Guid Connect(string playername)
        {
            Playername = playername;
            Server.Instance.Join(this);
            return ConnectionId;
        }

        [OperationBehavior]
        public void Disconnect(string reason)
        {
            Server.Instance.Leave(this);
            //Callback.Disconnect(reason);
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
