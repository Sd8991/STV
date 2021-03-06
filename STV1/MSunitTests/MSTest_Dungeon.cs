﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
	[TestClass]
	public class MSTest_Dungeon
	{
		[TestMethod]
		public void MSTest_Dungeonconstructor()
		{
			Dungeon d = new Dungeon(3, 2);
			Predicates p = new Predicates();
			Assert.IsTrue(p.isReachable(d.startNode, d.exitNode));
            //Assert.IsFalse(d.startNode is Bridge || d.exitNode is Bridge);
            Assert.IsFalse(d.startNode is Bridge);
            Assert.IsFalse(d.exitNode is Bridge);
			List<Node> nodes = p.reachableNodes(d.startNode);
			int totalConnectivityDegree = 0;
			foreach (Node nd in nodes)
			{
				foreach (Node nd2 in nd.neighbors)
				{
					// check that each connection is bi-directional
					Assert.IsTrue(nd2.neighbors.Contains(nd));
				}
				// check the connectivity degree
				Assert.IsFalse(nd.neighbors.Count > 4);
				totalConnectivityDegree += nd.neighbors.Count;
				// check bridge
				Boolean isBridge_ = p.isBridge(d.startNode, d.exitNode, nd);
				Assert.IsFalse(nd is Bridge && !isBridge_);
				Assert.IsFalse(!(nd is Bridge) && isBridge_);
			}
			float avrgConnectivity = (float)totalConnectivityDegree / (float)nodes.Count;
			Assert.IsTrue(avrgConnectivity > 3,"avrage: " + avrgConnectivity);

			//Assert.IsTrue(p.isValidDungeon(d.startNode, d.exitNode, 1));
		}

		#region GenerateDungeonGraph()
		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void MSTest_GenerateDungeonGraph_bridgesArray_to_short()
		{
			Node[] bridges = new Node[1];
			bridges[0] = new Bridge("1");
			Dungeon d = new Dungeon();
			d.GenerateDungeonGraph(bridges);
		}
		[TestMethod]
		public void MSTest_GenerateDungeonGraph_start_exit_and_bridge()
		{
			Node[] bridges = new Node[3];
			bridges[0] = new Node();
			bridges[1] = new Bridge("bridge");
			bridges[2] = new Node();
			Dungeon d = new Dungeon();
			d.GenerateDungeonGraph(bridges);
			int nNeigborsExpected = 4;
			if ((bridges[1] as Bridge).GetFromNodes.Count == 1)
				nNeigborsExpected--;
			if ((bridges[1] as Bridge).GetToNodes.Count == 1) 
				nNeigborsExpected--;
			Assert.IsTrue(bridges[1].neighbors.Count == nNeigborsExpected);
			Predicates p = new Predicates();
			Assert.IsTrue(p.isReachable(bridges[0], bridges[2]));
		}

		#endregion

		#region GenerateSubGraph()
		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void MSTest_GenerateSubGraph_minimum_to_low()
		{
			Node entrynode = new Node();
			Node exitnode = new Node();
			Dungeon d = new Dungeon();
			d.GenerateSubGraph(entrynode, exitnode, 1, 0, 10);
		}
		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void MSTest_GenerateSubGraph_maximum_to_low()
		{
			Node entrynode = new Node();
			Node exitnode = new Node();
			Dungeon d = new Dungeon();
			d.GenerateSubGraph(entrynode, exitnode, 1, 2, 1);
		}
		

		[TestMethod]
		public void MSTest_GenerateSubGraph_2_node_graph()
		{
			Bridge entrynode = new Bridge("1");
			Bridge exitnode = new Bridge("2");
			Dungeon d = new Dungeon();
			d.GenerateSubGraph(entrynode, exitnode, 1, 2, 2);
			Assert.IsTrue(entrynode.neighbors.Count == 2);
            Assert.IsTrue(entrynode.neighbors.Contains(exitnode) == false);
			Assert.IsTrue(exitnode.neighbors.Count == 2);
            Assert.IsTrue(exitnode.neighbors.Contains(entrynode) == false);
			List<Node> nodes1 = entrynode.neighbors[0].neighbors;
			List<Node> nodes2 = entrynode.neighbors[1].neighbors;
			Assert.IsTrue(nodes1.Count >= 2);
            Assert.IsTrue(nodes1.Contains(entrynode));
            Assert.IsTrue(nodes1.Contains(exitnode), $"count {nodes1.Count}, {nodes1.Contains(entrynode)}, {nodes1.Contains(exitnode)}");
            Assert.IsTrue(nodes2.Count >= 2);
            Assert.IsTrue(nodes2.Contains(entrynode));
            Assert.IsTrue(nodes2.Contains(exitnode));
			List<Node> nodes3 = exitnode.neighbors[0].neighbors;
			List<Node> nodes4 = exitnode.neighbors[1].neighbors;
			Assert.IsTrue(nodes3.Count >= 2);
            Assert.IsTrue(nodes3.Contains(entrynode));
            Assert.IsTrue(nodes3.Contains(exitnode));
			Assert.IsTrue(nodes4.Count >= 2);
            Assert.IsTrue(nodes4.Contains(entrynode));
            Assert.IsTrue(nodes4.Contains(exitnode));
        }
		#endregion

		#region RandomConnection() tests
		[TestMethod]
		public void MSTest_RandomConnection_to_empty_graph()
		{
			Node node = new Node();
			Node[] emptygraph = new Node[0];
			Dungeon d = new Dungeon();
			d.randomConnection(node, emptygraph);
			Assert.IsTrue(node.neighbors.Count == 0);
		}

		[TestMethod]
		public void MSTest_RandomConnection_thisnode_max_neighbors()
		{
			Node maxConnectionNode = new Node("maxConnectionNode");
			Node[] graph = new Node[5];
			Node graphnode = new Node();
			graph[4] = graphnode;
			for (int i = 0; i < 4; i++)
			{
				Node neighbor = new Node("neighbor " + i);
				maxConnectionNode.connect(neighbor);
			}
			Dungeon d = new Dungeon();
			d.randomConnection(maxConnectionNode, graph);
			Assert.IsTrue(maxConnectionNode.neighbors.Count == 4);
		}

		[TestMethod]
		public void MSTest_RandomConnection_Connect_to_same_node()
		{
			Node thisnode = new Node();
			Node[] graph = new Node[1];
			graph[0] = thisnode;
			Dungeon d = new Dungeon();
			d.randomConnection(thisnode, graph);
			Assert.IsFalse(thisnode.neighbors.Contains(thisnode));
		}

		[TestMethod]
		public void MSTest_RandomConnection_randomnode_max_neighbors()
		{
			Node[] graph = new Node[5];
			for (int i = 0; i < 5; i++)
			{
				graph[i] = new Node("neighbor " + i);
			}

			for (int i = 0; i < 5; i++)
			{
				for (int j = 4; j > i; j--)
				{
					graph[i].connect(graph[j]);
				}
			}
			Node testnode = new Node("test");
			Dungeon d = new Dungeon();
			d.randomConnection(testnode, graph);
			Assert.IsTrue(testnode.neighbors.Count == 0);
		}

		[TestMethod]
		public void MSTest_RandomConnection_connect_single_node_graph()
		{
			Node thisnode = new Node();
			Node[] graph = new Node[1];
			graph[0] = thisnode;
			Node testnode = new Node("test");
			Dungeon d = new Dungeon();
			d.randomConnection(testnode, graph);
			Assert.IsTrue(testnode.neighbors.Contains(thisnode));
            Assert.IsTrue(thisnode.neighbors.Contains(testnode));
		}

		[TestMethod]
		public void MSTest_RandomConnection_connect_multiple_node_graph()
		{
			Node node1 = new Node();
			Node node2 = new Node();
			Node node3 = new Node();
			node2.connect(node1);
			node2.connect(node3);
			Node[] graph = new Node[3];
			graph[0] = node1;
			graph[1] = node2;
			graph[2] = node3;
			Node testnode = new Node("test");
			Dungeon d = new Dungeon();
			d.randomConnection(testnode, graph);
			Assert.IsTrue(testnode.neighbors.Count == 1);
			Node result = testnode.neighbors[0];
			Assert.IsTrue(result.neighbors.Contains(testnode));
		}
		#endregion

		#region ConnectBridge() tests
		[TestMethod]
		public void MSTest_connectbrige_samezone()
		{
			Bridge bridge = new Bridge("1");
			Node node = new Node();
			Dungeon d = new Dungeon();
			d.connectBridge(bridge, node, true);
			Assert.IsTrue(bridge.GetFromNodes.Contains(node));
            Assert.IsTrue(bridge.neighbors.Contains(node));
            Assert.IsTrue(node.neighbors.Contains(bridge));
		}

		[TestMethod]
		public void MSTest_connectbrige_nextzone()
		{
			Bridge bridge = new Bridge("1");
			Node node = new Node();
			Dungeon d = new Dungeon();
			d.connectBridge(bridge, node, false);
			Assert.IsTrue(bridge.GetToNodes.Contains(node));
            Assert.IsTrue(bridge.neighbors.Contains(node));
            Assert.IsTrue(node.neighbors.Contains(bridge));
		}
		#endregion

		[TestMethod]
		public void MSTest_disconnectBridge()
		{
			Bridge bridge = new Bridge("bridge 4");
			Node node1 = new Node();
			Node node2 = new Node();
			Node node3 = new Node();
			Node node4 = new Node();            
            bridge.connectToNodeOfNextZone(node1);
			bridge.connectToNodeOfNextZone(node2);
			bridge.connectToNodeOfSameZone(node3);
			bridge.connectToNodeOfSameZone(node4);           
            Dungeon d = new Dungeon();
            Player P = new Player();
            P.location = bridge;
            P.dungeon = d;
			d.zone = new Dictionary<int, List<Node>>();
			d.zone[4] = new List<Node>();
			d.zone[4].Add(bridge);
			d.zone[4].Add(node3);
			d.zone[4].Add(node4);
			Item x = new Crystal("cryst1");
            P.bag.Add(x);
            P.use(x);
            Assert.IsTrue(bridge.GetFromNodes.Count == 2);
			Assert.IsTrue(bridge.neighbors.Contains(node1));
            Assert.IsTrue(bridge.neighbors.Contains(node2));
			Assert.IsTrue(!bridge.neighbors.Contains(node3));
            Assert.IsTrue(!bridge.neighbors.Contains(node4));
			Assert.IsTrue(!node3.neighbors.Contains(bridge));
            Assert.IsTrue(!node4.neighbors.Contains(bridge));
		}
		[TestMethod]
		public void MSTest_level()
		{
			Node node = new Node();
			Bridge bridge = new Bridge("bridge 4");
			Dungeon d = new Dungeon();
			uint nodeLevel = d.level(node);
			uint bridgeLevel = d.level(bridge);

			Assert.IsTrue(nodeLevel == 0);
            Assert.IsTrue(bridgeLevel == 4);
		}
		[TestMethod]
		public void MSTest_shortestpath()
		{
			Node node1 = new Node("1_1");
			Node node2 = new Node("1_2");
			Node node3 = new Node("1_3");
			Node node4 = new Node("1_4");
			Node node5 = new Node("1_5");
			node2.connect(node1);
			node2.connect(node3);
			node4.connect(node1);
			node4.connect(node5);
			node5.connect(node3);
			Dungeon d = new Dungeon();
			d.zone = new Dictionary<int, List<Node>>();
			d.zone[1] = new List<Node>();
			d.zone[1].Add(node1);
			d.zone[1].Add(node2);
			d.zone[1].Add(node3);
			d.zone[1].Add(node4);
			d.zone[1].Add(node5);

			List<Node> sp = d.shortestpath(node1, node3);
			List<Node> expectedSp = new List<Node>();
			expectedSp.Add(node2);
			expectedSp.Add(node3);
			Assert.IsTrue(sp.Count == 2);
			Assert.IsTrue(sp[0] == expectedSp[0]);
			Assert.IsTrue(sp[1] == expectedSp[1]);
		}

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MSTest_no_shortest_path()
        {
            Dungeon d = new Dungeon(5);
            List<Node> sp = d.shortestpath(d.zone[1][0], d.zone[1][3]);
        }

        [TestMethod]
        public void MSTest_Distribute_Potions()
        {
            Game game = new Game();
            RandomGenerator.initializeWithSeed(1);
            Random r = RandomGenerator.rnd;
            Dungeon d = new Dungeon(5);
			d.r = r;
			game.r = r;
            Player P = new Player();
            P.dungeon = d;
            game.dungeon = d;
            game.dungeon.zone = d.zone;
            uint monsterHP = 300;
            uint newPotionHP = game.DistributePotions(P, monsterHP);
			Assert.IsTrue(newPotionHP <= 0.8f * monsterHP);
        }
	}
}
