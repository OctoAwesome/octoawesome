using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class Package
    {
        /// <summary>
        /// Bytesize of Header
        /// </summary>
        public const int HEAD_LENGTH = 3;
        public const int SUB_HEAD_LENGTH = HEAD_LENGTH + 10;
        public const int SUB_CONTENT_HEAD_LENGTH = 9;

        public static ulong NextUid => nextUid++;

        private static ulong nextUid;

        public PackageType Type { get; set; }
        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public ulong Uid { get; set; }

        private byte[] header;

        public Package(ushort command, int size, PackageType type = 0) : this()
        {
            Type = type;
            Command = command;
            Payload = new byte[size];

        }

        public Package()
        {
        }
        public Package(byte[] data) : this(0, data.Length)
        {
        }

        public void SerializePackage(OctoNetworkStream stream)
        {
            if (Type == PackageType.Normal || Type == PackageType.None)
            {
                WriteHead(ref header);
                stream.Write(header, 0, header.Length);
                stream.Write(Payload, 0, Payload.Length);
            }
            else
            {
                SerializeSubPackages(stream);
            }
        }

        public void SerializeSubPackages(OctoNetworkStream stream)
        {
            var firstPackage = stream.Length - SUB_HEAD_LENGTH;
            var contentPackage = stream.Length - SUB_CONTENT_HEAD_LENGTH;

            do
            {
                WriteHead(ref header);
                header[8] = (byte)(count >> 8);
                header[9] = (byte)(count & 0xFF);
                stream.Write(header, 0, header.Length);
            } while (true);
        }

        public void WriteHead(ref byte[] buffer, int offset = 0)
        {
            byte[] header, bytes;
            int index = offset;
            header = buffer;

            switch (Type)
            {
                case PackageType.Normal:
                    header = new byte[HEAD_LENGTH];
                    header[index] = (byte)Type;
                    header[index++] = (byte)(Command >> 8);
                    header[index++] = (byte)(Command & 0xFF);
                    break;
                case PackageType.Subhead:
                    header = new byte[SUB_HEAD_LENGTH];
                    header[index] = (byte)Type;
                    header[index++] = (byte)(Command >> 8);
                    header[index++] = (byte)(Command & 0xFF);
                    bytes = BitConverter.GetBytes(NextUid);
                    Array.Copy(bytes, 0, header, index++, bytes.Length);
                    break;
                case PackageType.Subcontent:
                    header = new byte[SUB_CONTENT_HEAD_LENGTH];
                    header[index] = (byte)Type;
                    bytes = BitConverter.GetBytes(Uid);
                    Array.Copy(bytes, 0, header, index++, bytes.Length);
                    break;
                case PackageType.None:
                default:
                    break;
            }
        }

        public enum PackageType : byte
        {
            None,
            Normal,
            Subhead,
            Subcontent
        }
    }
}
