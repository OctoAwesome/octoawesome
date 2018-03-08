using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class SimulationManagerTests
    {
        private SimulationManager simulationManager;

        public SimulationManagerTests()
        {
            simulationManager = new SimulationManager(new Settings());
        }

        [TestMethod]
        public void StartStopTest()
        {
            simulationManager.Start();
            simulationManager.Stop();
        }

        [TestMethod]
        public void RuntimeTest()
        {
            var reset = new ManualResetEvent(false);
            var timer = new System.Timers.Timer
            {
                Interval = 30000
            };

            timer.Elapsed += (s, e) => reset.Set();


            simulationManager.Start();
            timer.Start();

            reset.WaitOne();

            simulationManager.Stop();
            
        }
    }
}
