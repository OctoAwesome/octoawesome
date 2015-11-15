using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctoAwesome.Basics;

namespace OctoAwesome.Tests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void CheckInventory()
        {
            PlayerInventory inv = new PlayerInventory(null, 2, 2);
            Assert.AreEqual(4, inv.RemainingSlots);

            Assert.AreEqual(true, inv.AddItem(new PickaxeDefinition(), 2));

            Assert.AreEqual(2, inv.RemainingSlots);

            Assert.AreEqual(true, inv.AddItem(new PickaxeDefinition(), 1, 1, 1));

            Assert.AreEqual(1, inv.RemainingSlots);

            Assert.AreEqual(true, inv.RemoveItem(new PickaxeDefinition(), 2));

            Assert.AreEqual(1, inv.Count(new PickaxeDefinition()));

            Assert.AreEqual(true, inv.RemoveItem(new PickaxeDefinition(), 1, 1, 1));

            Assert.AreEqual(false, inv.AddItem(new PickaxeDefinition(), 5));

        }

    }
}
