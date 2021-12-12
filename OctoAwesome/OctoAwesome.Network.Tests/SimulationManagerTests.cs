using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using NUnit.Framework;
using OctoAwesome.Notifications;

namespace OctoAwesome.Network.Tests
{
    [TestOf(typeof(SimulationManager))]
    public class SimulationManagerTests
    {
        private SimulationManager simulationManager;

        public SimulationManagerTests()
        {
            simulationManager = new SimulationManager(new Settings(), new UpdateHub());
        }

        [Test]
        public void StartStopTest()
        {
            simulationManager.Start();
            simulationManager.Stop();
        }

        [Test]
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
