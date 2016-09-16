using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace OctoTest
{

    public enum FlagBytes
    {
        Version
        
    }

    public class FileHeaderStream : FileStream
    {
        private const int TypeBytes = 16;
        private const int HeaderBytes = 32;
        private const int FlagBytes = HeaderBytes - TypeBytes;



        /// <summary>
        /// Typ der geöffneten Datei.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Flags der geöffneten Datei.
        /// </summary>
        private byte[] Flags { get; set; }


        /// <summary>
        /// Der Konstrukor erstellt einen neuen FileHeaderStream, welcher mit unseren FileHeadern arbeiten kann.
        /// </summary>
        /// <param name="path">Pfad zur Datei</param>
        /// <param name="mode">Modus des Öffnens</param>
        /// <param name="access">Modus des Zugriffs</param>
        public FileHeaderStream(string path, FileMode mode, FileAccess access) : base(path, mode, access)
        {

            Flags = new byte[FlagBytes];
            Type = "";

            if (mode == FileMode.Open || mode == FileMode.Append || mode == FileMode.OpenOrCreate)
            {
                // Wenn eine Datei bereits existiert, dann können wir diese öffnen und den Header lesen.
                ReadHeader();
                return;
            }
            if (mode == FileMode.Create || mode == FileMode.CreateNew)
            {
                return;
            }

            throw new NotImplementedException("Truncate brauchen wa nicht.");
        }

        /// <summary>
        /// Schreibt den Header an den Anfang des Streams. Muss auf jeden Fall vorm Erstellen des BW geschrieben werden.
        /// </summary>
        /// <param name="type">Typ der Datei</param>
        /// <param name="flagBytes">Flags</param>
        public void WriteHeader(string type, byte[] flagBytes)
        {
            
            if (type.Length > TypeBytes)
            {
                throw new ArgumentException("type is too long.");
            }
            if (flagBytes.Length > FlagBytes)
            {
                throw new ArgumentException("flagBytes is too long");
            }


            byte[] headerByteArray = new byte[HeaderBytes];


            Encoding.ASCII.GetBytes(type, 0, type.Length, headerByteArray, 0);
            Array.ConstrainedCopy(flagBytes,0, headerByteArray, TypeBytes, flagBytes.Length);

            Write(headerByteArray, 0, headerByteArray.Length);
            Flush();
        }

        /// <summary>
        /// Ließt den Header des Streams.
        /// </summary>
        private void ReadHeader()
        {
            byte[] retBytes = new byte[HeaderBytes];
            Read(retBytes, 0, HeaderBytes);


            // Wir wissen nicht die absolute Länge unseres Headers, also müssen wa da etwas tricksen. 
            // Wir entfernen also alle Null-Terminationen am Ende des Strings und kommen somit auf 
            // einen "normalen" String, den man vernünftig vergleichen kann.

            
            Type = Encoding.ASCII.GetString(retBytes, 0, TypeBytes).TrimEnd('\0');
            Array.ConstrainedCopy(retBytes, TypeBytes, Flags, 0, FlagBytes);
        }

        /// <summary>
        /// Gibt ein bestimmtes Flag aus dem Flags-Array zurück.
        /// </summary>
        /// <param name="flagByte"></param>
        /// <returns>Flag-Byte</returns>
        public byte Flag(FlagBytes flagByte)
        {
            return Flag((int) flagByte);
        }

        /// <summary>
        /// Gibt ein bestimmtes Flag aus dem Flags-Array zurück.
        /// </summary>
        /// <param name="index">Index des Arrays</param>
        /// <returns>Flag-Byte</returns>
        public byte Flag(int index)
        {
            if (index < 0 || index >= FlagBytes)
            {
                throw new ArgumentOutOfRangeException();     
            }

            return Flags[index];
        }

    }
}