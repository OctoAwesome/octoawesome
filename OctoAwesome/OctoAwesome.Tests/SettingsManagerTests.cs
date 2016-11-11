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
            Settings settings = new Settings();

            settings.DEBUG = true;

            string[] testArray = new string[] {"foo", "bar"};
            settings.Set("foo", testArray);
            
            string[] newArray = settings.GetArray<string>("foo");

            Assert.True(testArray.SequenceEqual(newArray));


            int[] testArrayInt = new int[] { 3,5,333,456,3457};
            settings.Set("fooInt", testArrayInt);

            int[] newArrayInt = settings.GetArray<int>("fooInt");

            Assert.True(testArray.SequenceEqual(newArray));


            bool[] testArrayBool = new bool[] { true, false};
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
        public void NullTest()
        {
            Settings settings = new Settings();

            settings.DEBUG = true;

            int test = settings.Get<int>("foobarnotset");
            Console.WriteLine(test);
        }
    }
}
