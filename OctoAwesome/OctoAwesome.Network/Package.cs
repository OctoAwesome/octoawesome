using System;

namespace OctoAwesome.Network
{
    public class Package
    {
        /// <summary>
        /// Bytesize of Header
        /// </summary>
        public const int HEAD_LENGTH = sizeof(ushort) + sizeof(int) + sizeof(uint);

        public static uint NextUId => nextUid++;
        private static uint nextUid; 
        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public uint UId { get; set; }

        public bool IsComplete => internalOffset == Payload.Length;

        private int internalOffset;

        public Package(ushort command, int size) : this()
        {
            Command = command;
            Payload = new byte[size];
        }

        public Package() 
            => UId = NextUId;

        public Package(byte[] data) : this(0, data.Length)
        {
        }

        public bool TryDeserializeHeader(byte[] buffer)
        {
            if (buffer.Length < HEAD_LENGTH)
                return false;

            Command = (ushort)((buffer[0] << 8) | buffer[1]);
            Payload = new byte[BitConverter.ToInt32(buffer, 2)];
            UId = BitConverter.ToUInt32(buffer, 6);
            internalOffset = 0;
            return true;
        }

        public void DeserializePayload(byte[] buffer, int offset, int count)
        {
            Buffer.BlockCopy(buffer, offset, Payload, internalOffset, count);
            internalOffset += count;
        }
        public void DeserializePackage(byte[] buffer)
        {
            TryDeserializeHeader(buffer);
            Buffer.BlockCopy(buffer, HEAD_LENGTH, Payload, 0, Payload.Length);
            internalOffset = Payload.Length;
        }

        public int SerializePackage(byte[] buffer)
        {
            buffer[0] = (byte)(Command >> 8);
            buffer[1] = (byte)(Command & 0xFF);
            var bytes = BitConverter.GetBytes(Payload.Length);
            Buffer.BlockCopy(bytes, 0, buffer, 2, 4);
            bytes = BitConverter.GetBytes(UId);
            Buffer.BlockCopy(bytes, 0, buffer, 6, 4);
            Buffer.BlockCopy(Payload, 0, buffer, HEAD_LENGTH, Payload.Length); //Payload.Serialize();
            return Payload.Length + HEAD_LENGTH;
        }
        
    }
}
