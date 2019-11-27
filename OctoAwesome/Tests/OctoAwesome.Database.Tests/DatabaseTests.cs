using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Database.Tests
{
    [TestOf(typeof(Database<>))]
    public class DatabaseTests
    {
        [TestCase("test.key", "test.value", TestName = "Default integration test")]
        public void DefaultIntegrationTest(string keyPath, string valuePath)
        {
            var temp = Path.GetTempPath();

            var keyFile = new FileInfo(Path.Combine(temp, keyPath));
            var valueFile = new FileInfo(Path.Combine(temp, valuePath));

            var database = new Database<IdTag>(keyFile, valueFile);
            

            try
            {
                database.Open();
                database.AddOrUpdate(new IdTag(42), new Value(Encoding.UTF8.GetBytes("Hello World 0")));
                database.AddOrUpdate(new IdTag(45), new Value(Encoding.UTF8.GetBytes("Hello World 1")));
                database.AddOrUpdate(new IdTag(47), new Value(Encoding.UTF8.GetBytes("Hello World 2")));
                var value0 = database.GetValue(new IdTag(42));
                var value2 = database.GetValue(new IdTag(45));
                var value3 = database.GetValue(new IdTag(47));
            }
            finally
            {
                database.Dispose();
                keyFile.Delete();
                valueFile.Delete();
            }
        }
    }
}
