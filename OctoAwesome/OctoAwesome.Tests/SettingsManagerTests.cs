using OctoAwesome.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace OctoAwesome.Tests
{
    public class SettingsManagerTests
    {
        [Test]
        public void ReadWrite()
        {
            Settings settings = new Settings();

            string[] testArray = new string[] { "foo", "bar" };
            settings.Set("foo", testArray);

            string[] newArray = settings.GetArray<string>("foo");

            Assert.True(testArray.SequenceEqual(newArray));


            int[] testArrayInt = new int[] { 3, 5, 333, 456, 3457 };
            settings.Set("fooInt", testArrayInt);

            int[] newArrayInt = settings.GetArray<int>("fooInt");

            Assert.True(testArray.SequenceEqual(newArray));


            bool[] testArrayBool = new bool[] { true, false };
            settings.Set("fooBool", testArrayBool);

            bool[] newArrayBool = settings.GetArray<bool>("fooBool");

            Assert.True(testArray.SequenceEqual(newArray));


            String inputString = "randomStringWith§$%&/()=Charakters";
            settings.Set("inputString", inputString);


            Assert.AreEqual(inputString, settings.Get<string>("inputString"));


            int inputInt = new Random().Next();
            settings.Set("inputInt", inputInt);

            Assert.AreEqual(inputInt, settings.Get<int>("inputInt"));


            bool inputBool = true;
            settings.Set("inputBool", inputBool);

            Assert.AreEqual(inputBool, settings.Get<bool>("inputBool"));
        }

        [Test]
        public void UnsetTest()
        {
            Settings settings = new Settings();

            int testInt = settings.Get<int>("foobarnotset");
            Assert.AreEqual(0, testInt);

            string testString = settings.Get<string>("foobarnotset");
            Assert.AreEqual(null, testString);

            int testIntDefault = settings.Get("foobarnotset", 42);
            Assert.AreEqual(42, testIntDefault);

            string testStringDefault = settings.Get("foobarnotset", "ABC");
            Assert.AreEqual("ABC", testStringDefault);
        }

        [Test]
        public void DeleteTest()
        {
            Settings settings = new Settings();

            settings.Set("test", 1);
            int test1 = settings.Get<int>("test");
            Assert.AreEqual(1, test1);

            settings.Delete("test");
            int test2 = settings.Get<int>("test");
            Assert.AreEqual(0, test2);
        }
    }
}
