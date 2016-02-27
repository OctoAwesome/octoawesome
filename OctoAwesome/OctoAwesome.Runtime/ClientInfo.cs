using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Datenstruktur eines Clients.
    /// </summary>
    [DataContract]
    public class ClientInfo
    {
        /// <summary>
        /// Die eindeutige Guid des Clients.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Spielername des Clients.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}