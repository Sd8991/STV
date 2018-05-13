using System;
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
			Assert.IsFalse(d.startNode is Bridge || d.exitNode is Bridge);
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
			Assert.IsTrue(avrgConnectivity > 3);

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
			Assert.IsTrue(entrynode.neighbors.Count == 2 && entrynode.neighbors.Contains(exitnode) == false);
			Assert.IsTrue(exitnode.neighbors.Count == 2 && exitnode.neighbors.Contains(entrynode) == false);
			List<Node> nodes1 = entrynode.neighbors[0].neighbors;
			List<Node> nodes2 = entrynode.neighbors[1].neighbors;
			Assert.IsTrue(nodes1.Count == 3 && nodes1.Contains(entrynode) && nodes1.Contains(exitnode));
			Assert.IsTrue(nodes2.Count == 3 && nodes2.Contains(entrynode) && nodes2.Contains(exitnode));
			List<Node> nodes3 = exitnode.neighbors[0].neighbors;
			List<Node> nodes4 = exitnode.neighbors[1].neighbors;
			Assert.IsTrue(nodes3.Count == 3 && nodes3.Contains(entrynode) && nodes3.Contains(exitnode));
			Assert.IsTrue(nodes4.Count == 3 && nodes4.Contains(entrynode) && nodes4.Contains(exitnode));
		}
		#endregion

		#region ConnectToRandomNodes() tests
		[TestMethod]
		public void MSTest_no_valid_nodes()
		{
			List<Node> graph = new List<Node>();
			for (int i = 0; i < 5; i++)
			{
				Node neighbor = new Node("neighbor " + i);
				for (int j = 0; j < graph.Count; j++)
				{
					neighbor.connect(graph[j]);
				}
				graph.Add(neighbor);
			}
			Node[] toNodes = new Node[graph.Count];
			for (int i = 0; i < graph.Count; i++)
			{
				toNodes[i] = graph[i];
			}
			Node thisnode = new Node();
			Dungeon d = new Dungeon();
			d.connectToRandomNodes(thisnode, toNodes, true);
			Assert.IsTrue(thisnode.neighbors.Count == 0);
		}
		[TestMethod]
		public void MSTest_no_tonodes()
		{
			Node[] toNodes = new Node[3];
			Node thisnode = new Node();
			Dungeon d = new Dungeon();
			d.connectToRandomNodes(thisnode, toNodes, true);
			Assert.IsTrue(thisnode.neighbors.Count == 0);
		}
		[TestMethod]
		public void MSTest_half_filled_tonodes()
		{
			Node[] toNodes = new Node[4];
			toNodes[0] = new Node();
			toNodes[3] = new Node();
			Node thisnode = new Node();
			Dungeon d = new Dungeon();
			d.connectToRandomNodes(thisnode, toNodes, true);
			Assert.IsTrue(thisnode.neighbors.Count >= 1 && thisnode.neighbors.Count <= 2);
		}
		[TestMethod]
		public void MSTest_single_valid_node()
		{
			Node[] toNodes = new Node[1];
			toNodes[0] = new Node();
			Node thisnode = new Node();
			Dungeon d = new Dungeon();
			d.connectToRandomNodes(thisnode, toNodes, true);
			Assert.IsTrue(thisnode.neighbors.Count == 1);
		}
		#endregion

		#region RandomConnection() tests
		[TestMethod]
		public void MSTest_RandomConnection_to_empty_graph()
		{
			Node node = new Node();
			List<Node> emptygraph = new List<Node>();
			Dungeon d = new Dungeon();
			Node result = d.randomConnection(node, emptygraph, true);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void MSTest_RandomConnection_thisnode_max_neighbors()
		{
			Node maxConnectionNode = new Node("maxConnectionNode");
			List<Node> graph = new List<Node>();
			Node graphnode = new Node();
			graph.Add(graphnode);
			for (int i = 0; i < 4; i++)
			{
				Node neighbor = new Node("neighbor " + i);
				maxConnectionNode.connect(neighbor);
			}
			Dungeon d = new Dungeon();
			Node result = d.randomConnection(maxConnectionNode, graph, true);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void MSTest_RandomConnection_Connect_to_same_node()
		{
			Node thisnode = new Node();
			List<Node> graph = new List<Node>();
			graph.Add(thisnode);
			Dungeon d = new Dungeon();
			Node result = d.randomConnection(thisnode, graph, true);
			Assert.IsNull(result);
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void MSTest_RandomConnection_randomnode_max_neighbors()
		{
			List<Node> graph = new List<Node>();
			for (int i = 0; i < 5; i++)
			{
				Node neighbor = new Node("neighbor " + i);
				for (int j = 0; j < graph.Count; j++)
				{
					neighbor.connect(graph[j]);
				}
				graph.Add(neighbor);
			}
			Node testnode = new Node("test");
			Dungeon d = new Dungeon();
			Node result = d.randomConnection(testnode, graph, true);
		}

		[TestMethod]
		public void MSTest_RandomConnection_connect_single_node_graph()
		{
			Node thisnode = new Node();
			List<Node> graph = new List<Node>();
			graph.Add(thisnode);
			Node testnode = new Node("test");
			Dungeon d = new Dungeon();
			Node result = d.randomConnection(testnode, graph, true);
			Assert.AreEqual(result, thisnode);
			Assert.IsTrue(testnode.neighbors.Contains(thisnode) && thisnode.neighbors.Contains(testnode));
		}

		[TestMethod]
		public void MSTest_RandomConnection_connect_multiple_node_graph()
		{
			Node node1 = new Node();
			Node node2 = new Node();
			Node node3 = new Node();
			node2.connect(node1);
			node2.connect(node3);
			List<Node> graph = new List<Node>();
			graph.Add(node1);
			graph.Add(node2);
			graph.Add(node3);
			Node testnode = new Node("test");
			Dungeon d = new Dungeon();
			Node result = d.randomConnection(testnode, graph, true);
			Assert.IsTrue(testnode.neighbors.Contains(result) && result.neighbors.Contains(testnode));
		}

		[TestMethod]
		public void MSTest_RandomConnection_connect_bridge_single_node_graph()
		{
			Node thisnode = new Node();
			List<Node> graph = new List<Node>();
			graph.Add(thisnode);
			Bridge testnode = new Bridge("test");
			Dungeon d = new Dungeon();
			Node result = d.randomConnection(testnode, graph, true);
			Assert.AreEqual(result, thisnode);
			Assert.IsTrue(testnode.GetFromNodes.Contains(thisnode) && testnode.neighbors.Contains(thisnode) && thisnode.neighbors.Contains(testnode));
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
			Assert.IsTrue(bridge.GetFromNodes.Contains(node) && bridge.neighbors.Contains(node) && node.neighbors.Contains(bridge));
		}

		[TestMethod]
		public void MSTest_connectbrige_nextzone()
		{
			Bridge bridge = new Bridge("1");
			Node node = new Node();
			Dungeon d = new Dungeon();
			d.connectBridge(bridge, node, false);
			Assert.IsTrue(bridge.GetToNodes.Contains(node) && bridge.neighbors.Contains(node) && node.neighbors.Contains(bridge));
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
			d.disconnect(bridge);
			Assert.IsTrue(bridge.GetFromNodes.Count == 2);
			Assert.IsTrue(bridge.neighbors.Contains(node1) && bridge.neighbors.Contains(node2));
			Assert.IsTrue(!bridge.neighbors.Contains(node3) && !bridge.neighbors.Contains(node4));
			Assert.IsTrue(!node3.neighbors.Contains(bridge) && !node4.neighbors.Contains(bridge));
		}
		[TestMethod]
		public void MSTest_level()
		{
			Node node = new Node();
			Bridge bridge = new Bridge("bridge 4");
			Dungeon d = new Dungeon();
			uint nodeLevel = d.level(node);
			uint bridgeLevel = d.level(bridge);

			Assert.IsTrue(nodeLevel == 0 && bridgeLevel == 4);
		}
		[TestMethod]
		public void MSTest_shortestpath()
		{
			Node node1 = new Node();
			Node node2 = new Node();
			Node node3 = new Node();
			Node node4 = new Node();
			Node node5 = new Node();
			node2.connect(node1);
			node2.connect(node3);
			node4.connect(node1);
			node4.connect(node5);
			node5.connect(node3);
			Dungeon d = new Dungeon();

			List<Node> sp = d.shortestpath(node1, node3);
			List<Node> expectedSp = new List<Node>();
			expectedSp.Add(node2);
			expectedSp.Add(node3);
			Assert.IsTrue(sp.Count == 2);
			Assert.IsTrue(sp[0] == expectedSp[0]);
			Assert.IsTrue(sp[1] == expectedSp[1]);
		}
	}
}
