using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MTTest_Node
    {
        [TestMethod]
        public void MSTest_create_node_with_id()
        {
            Node node = new Node("nodeID");
            Assert.IsTrue(node.id == "nodeID");
        }

        [TestMethod]
        public void MSTest_connect_nodes()
        {
            Node baseNode = new Node("baseNode");
            Node nbNode = new Node("nbNode");
            baseNode.connect(nbNode);
            Assert.IsTrue(baseNode.neighbors.Contains(nbNode) && nbNode.neighbors.Contains(baseNode));
        }

        [TestMethod]
        public void MSTest_disconnect_nodes()
        {
            Node baseNode = new Node();
            Node nbNode = new Node();
            baseNode.neighbors.Add(nbNode);
            baseNode.neighbors.Add(baseNode);
            baseNode.disconnect(nbNode);
            Assert.IsFalse(baseNode.neighbors.Contains(nbNode) && nbNode.neighbors.Contains(baseNode));
        }

        [TestMethod]
        public void MSTest_disconnect_not_connected_nodes()
        {
            Node node1 = new Node();
            Node node2 = new Node();
            node1.disconnect(node2);
        }

        [TestMethod]
        public void MSTest_contested_node()
        {
            Player P = new Player();
            Pack pack = new Pack("pack", 2);
            Node N = new Node();
            N.packs.Add(pack);
            P.location = N;
            Assert.IsTrue(N.contested(P));
        }

        [TestMethod]
        public void MSTest_not_contested_node()
        {
            Player P = new Player();
            Pack pack = new Pack("pack", 2);
            Node N = new Node();
            P.location = N;
            Assert.IsFalse(N.contested(P));
        }
    }
}
