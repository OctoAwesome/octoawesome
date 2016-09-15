using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Tests
{
    [TestClass]
    public class SettingsManagerTests
    {
        [TestMethod]
        public void ReadWrite()
        {
            SettingsManager.DEBUG = true;

            string[] testArray = new string[] {"foo", "bar"};
            SettingsManager.Set("foo", testArray);
            
            string[] newArray = SettingsManager.GetArray<string>("foo");

            Assert.IsTrue(testArray.SequenceEqual(newArray));


            int[] testArrayInt = new int[] { 3,5,333,456,3457};
            SettingsManager.Set("fooInt", testArrayInt);

            int[] newArrayInt = SettingsManager.GetArray<int>("fooInt");

            Assert.IsTrue(testArray.SequenceEqual(newArray));


            bool[] testArrayBool = new bool[] { true, false};
            SettingsManager.Set("fooBool", testArrayBool);

            bool[] newArrayBool = SettingsManager.GetArray<bool>("fooBool");

            Assert.IsTrue(testArray.SequenceEqual(newArray));


            String inputString = "randomStringWith§$%&/()=Charakters";
            SettingsManager.Set("inputString", inputString);


            Assert.AreEqual(inputString, SettingsManager.Get<string>("inputString"));


            int inputInt = new Random().Next();
            SettingsManager.Set("inputInt", inputInt);

            Assert.AreEqual(inputInt, SettingsManager.Get<int>("inputInt"));


            bool inputBool = true;
            SettingsManager.Set("inputBool", inputBool);

            Assert.AreEqual(inputBool, SettingsManager.Get<bool>("inputBool"));



        }
    }
}
