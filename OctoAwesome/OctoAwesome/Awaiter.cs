using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public class Awaiter
    {
        public ISerializable Serializable { get; set; }
        public bool Timeouted { get; private set; }
        private readonly ManualResetEventSlim manualReset;
        private bool alreadyDeserialized;


        public Awaiter()
        {
            manualReset = new ManualResetEventSlim(false);
        }

        public ISerializable WaitOn()
        {
            if (!alreadyDeserialized)
                Timeouted = !manualReset.Wait(3000);

            return Serializable;
        }

        public void SetResult(ISerializable serializable)
        {
            Serializable = serializable;
            manualReset.Set();
            alreadyDeserialized = true;
        }

        public void SetResult(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            {
                Serializable.Deserialize(reader);
            }
            manualReset.Set();
            alreadyDeserialized = true;
        }
    }
}
