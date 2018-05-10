using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    [TestClass]
    class MSTest_Node
    {
        [TestMethod]
        public void MSTest_connect_nodes()
        {
            Node baseNode = new Node();
            Node nbNode = new Node();
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
        public void MSTest_contested_node()
        {
            Player P = new Player();
            Pack pack = new Pack("pack", 2);
            Node N = new Node();
            P.location = N;
            pack.location = N;
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
