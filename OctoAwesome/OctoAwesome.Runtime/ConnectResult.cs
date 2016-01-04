using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OctoAwesome.Runtime
{
    [DataContract]
    public class ConnectResult
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public IEnumerable<ClientInfo> OtherClients { get; set; }
    }
}