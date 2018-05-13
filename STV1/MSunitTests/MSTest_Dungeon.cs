using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_Dungeon
    {
        [TestMethod]
        public void MSTest_single_valid_node()
        {
            Node[] toNodes = new Node[1];
            toNodes[0] = new Node();
            Node thisnode = new Node();
            Dungeon d = new Dungeon();
            d.connectToRandomNodes(thisnode, toNodes, true);
            Assert.IsTrue(thisnode.neighbors.Count == 0);
        }
    }
}
