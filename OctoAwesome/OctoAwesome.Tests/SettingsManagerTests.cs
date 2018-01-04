using OctoAwesome.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace OctoAwesome.Tests
{
    public class SettingsManagerTests
    {
        [Fact]
        public void ReadWrite()
        {
            Settings settings = new Settings(true);

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


            Assert.Equal(inputString, settings.Get<string>("inputString"));


            int inputInt = new Random().Next();
            settings.Set("inputInt", inputInt);

            Assert.Equal(inputInt, settings.Get<int>("inputInt"));


            bool inputBool = true;
            settings.Set("inputBool", inputBool);

            Assert.Equal(inputBool, settings.Get<bool>("inputBool"));
        }

        [Fact]
        public void UnsetTest()
        {
            Settings settings = new Settings(true);

            int testInt = settings.Get<int>("foobarnotset");
            Assert.Equal(0, testInt);

            string testString = settings.Get<string>("foobarnotset");
            Assert.Equal(null, testString);

            int testIntDefault = settings.Get("foobarnotset", 42);
            Assert.Equal(42, testIntDefault);

            string testStringDefault = settings.Get("foobarnotset", "ABC");
            Assert.Equal("ABC", testStringDefault);
        }

        [Fact]
        public void DeleteTest()
        {
            Settings settings = new Settings(true);

            settings.Set("test", 1);
            int test1 = settings.Get<int>("test");
            Assert.Equal(1, test1);

            settings.Delete("test");
            int test2 = settings.Get<int>("test");
            Assert.Equal(0, test2);
        }
    }
}
