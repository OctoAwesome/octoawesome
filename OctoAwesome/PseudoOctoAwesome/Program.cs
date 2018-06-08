using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoOctoAwesome
{
    class Program
    {
        static void Main(string[] args)
        {
            var buffer = new byte[10];

            var stream = new MemoryStream();

            stream.Write(buffer, 0, 100);
        }
    }
}
