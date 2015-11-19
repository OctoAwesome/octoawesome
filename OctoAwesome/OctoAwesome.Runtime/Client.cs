using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    internal class Client : IClient
    {
        [OperationBehavior]
        public void Connect(string playername)
        {
            try
            {
                // Test
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
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
