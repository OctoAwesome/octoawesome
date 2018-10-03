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
        private readonly ManualResetEventSlim manualReset;
        private bool alreadyDeserialized;

        public Awaiter()
        {
            manualReset = new ManualResetEventSlim(false);
        }

        public ISerializable WaitOn()
        {
            if (!alreadyDeserialized)
                manualReset.Wait();

            return Serializable;
        }

        public void SetResult(ISerializable serializable)
        {
            Serializable = serializable;
            manualReset.Set();
            alreadyDeserialized = true;
        }

        public void SetResult(byte[] bytes, IDefinitionManager definitionManager)
        {
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            {
                Serializable.Deserialize(reader, definitionManager);
            }
            manualReset.Set();
            alreadyDeserialized = true;
        }
    }
}
