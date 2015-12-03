using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IChunkConnection
    {
        [OperationContract]
        Stream GetChunk(int planet, int x, int y, int z);
    }
}
