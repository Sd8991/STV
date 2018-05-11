using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_Bridge
    {
        [TestMethod]
        public void MSTest_create_bridge()
        {
            Bridge bridge = new Bridge("bridgeID");
            Assert.IsNotNull(bridge);
            Assert.IsTrue(bridge.id == "bridgeID");
        }

        [TestMethod]
        public void MSTest_connect_to_same_zone()
        {
            Bridge bridge = new Bridge("bridgeID");
            Node node1 = new Node("node1");
            bridge.connectToNodeOfSameZone(node1);
            Assert.IsTrue(bridge.GetFromNodes.Contains(node1));
        }

        [TestMethod]
        public void MSTest_connect_to_next_zone()
        {
            Bridge bridge = new Bridge("bridgeID");
            Node node1 = new Node("node1");
            bridge.connectToNodeOfNextZone(node1);
            Assert.IsTrue(bridge.neighbors.Contains(node1));
        }
    }
}
