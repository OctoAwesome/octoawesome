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
        public const int HEAD_LENGTH = 8;


        public PackageType Type { get; set; }
        public ushort Command { get; set; }

        public byte[] Payload { get; set; }


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

        public byte[] NextSubPackage()
        {

        }

        public void WriteHead()
        {
            byte[] header = new byte[HEAD_LENGTH];

            switch (Type)
            {
                case PackageType.Normal:
                    header[0] = (byte)Type;
                    header[1] = (byte)(Command >> 8);
                    header[2] = (byte)(Command & 0xFF);
                    //fixed(var ptr = header)
                    //(*(short*)(&ptr[1])) = Command;
                    break;
                case PackageType.Subhead:
                    break;
                case PackageType.Subcontent:
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
