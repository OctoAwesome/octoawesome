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

        public Client()
        {
            callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
        }

        [OperationBehavior]
        public void Connect(string playername)
        {
            callback.Relocation(1, 2, 3);
        }

        [OperationBehavior]
        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public void Jump()
        {
            throw new NotImplementedException();
        }
    }
}
