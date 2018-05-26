using System;
using System.Collections.Generic;
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
            Assert.IsTrue(baseNode.neighbors.Contains(nbNode));
            Assert.IsTrue(nbNode.neighbors.Contains(baseNode));
        }

        [TestMethod]
        public void MSTest_disconnect_nodes()
        {
            Node baseNode = new Node();
            Node nbNode = new Node();
            baseNode.neighbors.Add(nbNode);
            baseNode.neighbors.Add(baseNode);
            baseNode.disconnect(nbNode);
            Assert.IsFalse(baseNode.neighbors.Contains(nbNode));
            Assert.IsFalse(nbNode.neighbors.Contains(baseNode));
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

        [TestMethod]
        public void MSTest_combat_pack_not_flees_does_attack_dies()
        {
            Utils.RandomGenerator.initializeWithSeed(0);
            Random r = Utils.RandomGenerator.rnd;
            Player P = new Player();
            Pack pack = new Pack("pack", 1);
            pack.members[0].HP = 7;
            pack.startingHP = 7;
            Node fightNode = new Node(0);
            Node retreatNode = new Node(0);
            fightNode.packs.Add(pack);
            fightNode.connect(retreatNode);
            P.location = fightNode;
            pack.location = fightNode;
            fightNode.fight(P, 0, true);
            Assert.IsTrue(!fightNode.contested(P));
            Assert.IsTrue(fightNode.packs.Count == 0);
            Assert.IsTrue(P.KillPoint == 1);
        }

        [TestMethod]
        public void MSTest_combat_pack_flees()
        {
            Dungeon dungeon = new Dungeon(5);
            Utils.RandomGenerator.initializeWithSeed(1);
            Random r = Utils.RandomGenerator.rnd;
            Player P = new Player();
            Pack pack = new Pack("pack", 1);
            pack.members[0].HP = 6;
            pack.startingHP = 100;
            P.dungeon = dungeon;
            pack.dungeon = dungeon;
            P.location = dungeon.zone[1][0];
            pack.location = dungeon.zone[1][0];
            dungeon.zone[1][0].packs.Add(pack);
            dungeon.zone[1][0].fight(P, 1, true);
            Assert.IsTrue(!dungeon.zone[1][0].contested(P));
            Assert.IsTrue(P.KillPoint == 0);
            Assert.IsTrue(dungeon.zone[1][1].packs.Contains(pack));
        }

        [TestMethod]
        public void MSTest_combat_pack_flees_still_contested()
        {
            //To do:
            // - Hook up TestPlayer.GetNextCommand() to the combat in a way that allows combat involving a regular player to still work           
            Dungeon dungeon = new Dungeon(5);
            Utils.RandomGenerator.initializeWithSeed(1);
            Random r = Utils.RandomGenerator.rnd;           
            Pack pack = new Pack("pack", 1);
            Pack pack2 = new Pack("pack2", 1);
            List<Command> Queue = new List<Command>();
            Queue.Add(new AttackCommand(pack.members[0]));
            Queue.Add(new MoveCommand(dungeon.zone[1][1]));
            TestPlayer P = new TestPlayer(Queue);
            pack.members[0].HP = 6;
            pack2.members[0].HP = 1;
            pack.startingHP = 100;
            pack2.startingHP = 100;
            P.dungeon = dungeon;
            pack.dungeon = dungeon;
            pack2.dungeon = dungeon;
            P.location = dungeon.zone[1][0];
            pack.location = dungeon.zone[1][0];
            pack2.location = dungeon.zone[1][0];
            dungeon.zone[1][0].packs.Add(pack);
            dungeon.zone[1][0].packs.Add(pack2);
            dungeon.zone[1][0].fight(P, 1, true);
            Assert.IsTrue(dungeon.zone[1][1].packs.Count == 1);
            Assert.IsTrue(pack2.location == dungeon.zone[1][0]);
        }
    }
}
