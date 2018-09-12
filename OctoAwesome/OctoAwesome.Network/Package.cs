using System;

namespace OctoAwesome.Network
{
    public class Package
    {
        /// <summary>
        /// Bytesize of Header
        /// </summary>
        public const int HEAD_LENGTH = 6;

        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public Package(ushort command, int size) : this()
        {
            Command = command;
            Payload = new byte[size];
        }

        public Package()
        {
        }
        public Package(byte[] data) : this(0, data.Length)
        {
        }

        public void DeserializePackage(byte[] buffer)
        {
            Command = (ushort)((buffer[0] << 8) | buffer[1]);
            Payload = new byte[BitConverter.ToInt32(buffer, 2)];
            Buffer.BlockCopy(buffer, HEAD_LENGTH, Payload, 0, Payload.Length); //Payload.Deserialize()
        }

        public int SerializePackage(byte[] buffer)
        {
            buffer[0] = (byte)(Command >> 8);
            buffer[1] = (byte)(Command & 0xFF);
            var bytes = BitConverter.GetBytes(Payload.Length);
            Buffer.BlockCopy(bytes, 0, buffer, 2, 4);
            Buffer.BlockCopy(Payload, 0, buffer, HEAD_LENGTH, Payload.Length); //Payload.Serialize();
            return Payload.Length + HEAD_LENGTH;
        }




    }
}
