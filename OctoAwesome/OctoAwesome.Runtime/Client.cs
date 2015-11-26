using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Client : IClient
    {
        private IClientCallback callback;

        public Guid ConnectionId { get; private set; }

        public string Playername { get; private set; }

        public Client()
        {
            callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            ConnectionId = Guid.NewGuid();
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
            throw new NotImplementedException();
        }
    }
}
