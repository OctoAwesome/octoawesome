using System;
using System.IO;
using OctoAwesome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctoTest;

namespace OctoAwesome.Tests
{
    [TestClass]
    public class FileHeaderStreamTest
    {
        [TestMethod]
        public void ReadWrite()
        {
            String testType = "OCTOFOO";
            String testString = "Ich bin eine Datei.";
            byte[] testFlags = new byte[] {2};

            FileHeaderStream os = new FileHeaderStream("foo.txt", FileMode.Create, FileAccess.Write);
            os.WriteHeader(testType, testFlags);
            StreamWriter sw = new StreamWriter(os);
            sw.Write(testString);
            sw.Flush();
            sw.Close();
            os.Close();


            FileHeaderStream os2 = new FileHeaderStream("foo.txt", FileMode.Open, FileAccess.Read);
            
            Assert.AreEqual(testType, os2.Type);
            Assert.AreEqual(testFlags[0], os2.Flag(FlagBytes.Version));


            StreamReader sr = new StreamReader(os2);
            string line = sr.ReadLine();
            Console.WriteLine(line);
            sr.Close();

            Assert.AreEqual(line, testString);

            os2.Close();
        }
    }
}