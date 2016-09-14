using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Tests
{
    [TestClass]
    public class SettingsManagerTests
    {
        [TestMethod]
        public void ReadWrite()
        {
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
