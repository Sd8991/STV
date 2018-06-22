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
        public void MSTest_fight_multiple_packs()
        {
            Game game = new Game();
            Utils.RandomGenerator.initializeWithSeed(0);
            Random r = Utils.RandomGenerator.rnd;
            game.player = new Player();
            game.dungeon = new Dungeon();
            Pack pack = new Pack("pack", 2);
            pack.members[0].HP = 5;
            pack.members[1].HP = 5;
            pack.startingHP = 10;
            Node fightNode = new Node(0);
            game.player.dungeon = game.dungeon;
            game.player.zone = 1;
            game.dungeon.zone.Add(1, new List<Node>());
            fightNode.packs.Add(pack);
            game.dungeon.zone[1].Add(fightNode);
            pack.dungeon = game.dungeon;
            game.player.location = fightNode;
            pack.location = fightNode;
            while (game.player.location.contested(game.player))
            {
                game.update(new AttackCommand(pack.members[0].id));
            }
            Assert.IsTrue(!fightNode.contested(game.player));
            Assert.IsTrue(fightNode.packs.Count == 0);
            Assert.IsTrue(game.player.KillPoint == 2);
        }

        [TestMethod]
        public void MSTest_combat_pack_not_flees_does_attack_dies()
        {
            Game game = new Game();
            game.dungeon = new Dungeon();
            Utils.RandomGenerator.initializeWithSeed(0);
            Random r = Utils.RandomGenerator.rnd;
            game.player = new Player();
            Pack pack = new Pack("pack", 1);
            pack.members[0].HP = 7;
            pack.startingHP = 7;
            Node fightNode = new Node(0);
            Node retreatNode = new Node(0);
            game.player.location = fightNode;
            game.player.dungeon = game.dungeon;
            game.dungeon.zone.Add(1, new List<Node>());
            pack.location = game.player.location;
            game.dungeon.zone[1].Add(fightNode);
            game.dungeon.zone[1].Add(retreatNode);
            game.player.zone = 1;
            fightNode.packs.Add(pack);
            fightNode.connect(retreatNode);
            while (fightNode.contested(game.player))
            {
                game.update(new AttackCommand(pack.members[0].id));
            }
            Assert.IsTrue(!fightNode.contested(game.player));
            Assert.IsTrue(fightNode.packs.Count == 0);
            Assert.IsTrue(game.player.KillPoint == 1);
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
            Dungeon dungeon = new Dungeon(5);
            Utils.RandomGenerator.initializeWithSeed(1);
            Random r = Utils.RandomGenerator.rnd;           
            Pack pack = new Pack("pack", 1);
            Pack pack2 = new Pack("pack2", 1);
            List<Command> Queue = new List<Command>();
            Queue.Add(new AttackCommand(pack.members[0].id));
            Queue.Add(new MoveCommand(0));
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
