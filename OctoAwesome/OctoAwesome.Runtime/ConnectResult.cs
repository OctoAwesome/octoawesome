using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Ergebnis einer Verbindung mit einem Server.
    /// </summary>
    [DataContract]
    public class ConnectResult
    {
        /// <summary>
        /// Die neu zugewiesenene Guid der Verbindung.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Alle anderen Clients, die sich bereits auf dem Server befinden.
        /// </summary>
        [DataMember]
        public IEnumerable<ClientInfo> OtherClients { get; set; }
    }
}