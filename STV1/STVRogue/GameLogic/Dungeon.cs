using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Dungeon
    {
        public Node startNode;
        public Node exitNode;
        public uint difficultyLevel;
        /* a constant multiplier that determines the maximum number of monster-packs per node: */
        public uint M;
		public int maxConnectivity = 4;

		private Random r;
		
        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier)
        {
            Logger.log($"Creating a dungeon of difficulty level {level}, node capacity multiplier {nodeCapacityMultiplier}.");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;
			r = new Random();
			startNode = new Node("startNode");
			exitNode  = new Node("exitNode");

			Node[] bridges = new Node[level+2];
			bridges[0] = startNode;
			for (int i = 1; i <= level; i++)
			{
				bridges[i] = new Bridge($"bridge {i}");
			}
			bridges[level + 1] = exitNode;
			GenerateDungeonGraph(bridges);
        }

		#region DungeonGraph generation
		//builds the dungeon graph by greating subgraphs between the nodes in de brdiges array (which include the start- and exitNode)
		private void GenerateDungeonGraph(Node[] bridges)
		{
			int l = bridges.Length;
			if(l < 2) //sanity check, need atleast 2 nodes to make a valid graph
				throw new Exception("the given Node[] bridges is to short, needs to contain atleast 2 nodes");

			for (int i = 1; i < l; i++)
			{
				GenerateSubGraph(bridges[i-1],bridges[i]);
			}

			//TODO: establish guarentee that average connectivity is below 3.0
			//TODO: populate dungeon with monsters/packs
			//TODO: populate dungeon with items
		}

		//generate a fully connected subgraph
		private void GenerateSubGraph(Node entryNode, Node endNode, int minNodes = 1, int maxNodes = 10)
		{
			//sanity checks
			if(minNodes < 1)
				throw new Exception("minimum amount of nodes needs to be atleast 1");
			if(minNodes > maxNodes)
				throw new Exception("minNodes ("+minNodes+") can't be larger than maxNodes ("+maxNodes+")");


			int n = r.Next(minNodes,maxNodes);
			Node[] newNodes = new Node[n];
			Node[] connectedNodes = new Node[n];
			for (int i = 0; i < n; i++)
			{
				newNodes[i] = new Node();//TODO: define ID generation
			}

			//connect the next new node to the already connected nodes (to guarrantee that the graph is full connected)
			for (int i = 0; i < n; i++)
			{
				connectToRandomNodes(newNodes[i], connectedNodes, maxConnectivity, true);
				connectedNodes[i] = newNodes[i];
			}

			int endConnections = maxConnectivity;
			if(endNode.GetType() == typeof(Bridge))//make sure a bridge has atleast 1 connection left to connect to the next zone
				endConnections --;

			//connect the start and exit node of this subgraph
			connectToRandomNodes(entryNode, connectedNodes, maxConnectivity, false);
			connectToRandomNodes(endNode, connectedNodes, endConnections, true);
		}

		private void connectToRandomNodes(Node thisNode, Node[] toNodes, int maxConnections, bool sameZone)
		{
			List<Node> validnodes = new List<Node>();	//only use nodes that can still get more connections	
			foreach (Node n in toNodes)
			{
				if(n.neighbors.Count < 4)
					validnodes.Add(n);//only use nodes that can make a new connection
			}
			if(validnodes.Count == 0)
				return; // no nodes to connect to
			
			int actualMaxConnections = Math.Min(validnodes.Count, maxConnections);//correct the maximum number of connections (can't make more connections than nodes available)
			int nConnections = r.Next(1,actualMaxConnections);//atleast 1 so the node is actualy getting connected to the rest of the graph
			for (int i = 0; i < nConnections; i++)
			{
				Node chosenNode = randomConnection(thisNode,validnodes,sameZone);
				if(chosenNode!=null)
					validnodes.Remove(chosenNode);//remove so there can't be made a second connection between thisNode and the chosenNode
			}
		}

		private Node randomConnection(Node thisNode, List<Node> toNodes, bool sameZone)
		{
			int l = toNodes.Count;
			if(l == 0)
				return null;// no nodes to connect to (this is a valid situation)

			if(thisNode.neighbors.Count >= maxConnectivity)
				return null;// thisNode is not allowed to have more neigbours
			
			int index = 0;

			if(l == 1)
			{
				if(thisNode == toNodes[0])
				{
					Logger.log("tried to connect to itself");//should not happen
					return null;//no valid nodes to connect to
				}
			}
			else
			{
				index = r.Next(l - 1);
				if(toNodes[index].neighbors.Count >=maxConnectivity)
					throw new Exception("toNodes contained a node whit atleast maxconnectivity neighbors");
			}

			//if thisNode is a bridge, connect to the correct side, else use normal connect
			if(thisNode.GetType() == typeof(Bridge))
				connectBridge((thisNode as Bridge), toNodes[index], sameZone);
			else
				thisNode.connect(toNodes[index]);

			return toNodes[index];//return chosen node
		}

		// connects the correct side of the bridge to a given node
		private void connectBridge(Bridge thisBrige, Node toNode, bool sameZone)
		{
			if(sameZone)
				thisBrige.connectToNodeOfSameZone(toNode);
			else
				thisBrige.connectToNodeOfNextZone(toNode);
		}
		#endregion

        /* Return a shortest path between node u and node v */
        public List<Node> shortestpath(Node u, Node v) 
		{
			//Breadth First Search
			Queue<Node> q = new Queue<Node>();
			List<Node> seen = new List<Node>();//nodes that are already visited
			Dictionary<Node,Node> meta = new Dictionary<Node, Node>();
			meta[u] = null;
			q.Enqueue(u);

			while (q.Count != 0 )
			{
				Node subtree_root = q.Dequeue();
				if(subtree_root == v) 
					return constructPath(v,meta);//found destination, return path

				foreach (Node n in subtree_root.neighbors)//add all not fisited neighbors to the queue
				{
					if(seen.Contains(n))
						continue;//allready found this node, don't add again
					if(!q.Contains(n))
					{
						meta[n] = subtree_root;
						q.Enqueue(n);
					}
				}
				seen.Add(subtree_root);
			}
			//q.count == 0, so all accesble nodes from u have been fisited but v was not among them.
			throw new Exception("no path found");//TODO: decide whether "no path" is a valid outcome
		}

		private List<Node> constructPath(Node target, Dictionary<Node,Node> meta)
		{
			List<Node> path = new List<Node>();
			while(true)
			{
				Node row = meta[target];
				if(row != null)
				{
					path.Add(target);
					target = row;
				}
				else
				{
					break;
				}
			}
			path.Reverse();
			return path;
		}


        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");
            throw new NotImplementedException();
        }

        /* To calculate the level of the given node. */
        public uint level(Node d) 
		{
			if(d.GetType() == typeof(Bridge))
				return uint.Parse(d.id.Split()[1]);//given node is a bridge, return it's level
			else
				return 0;
			/*
			//Breadth First Search
			Queue q = new Queue();
			List<Node> seen = new List<Node>();//nodes that are already visited
			List<Node> bridgesFound = new List<Node>(2);//can't be more than 2 bridges in a level/zone
			q.Enqueue(d);

			while(q.Count != 0)
			{
				Node subtree = q.Dequeue();
				if(subtree == startNode)
					return 1;//node in same zone as startNode, therefor level 1

				if(subtree.GetType() == typeof(Bridge))
				{
					if(!bridgesFound.Contains(subtree))
					{
						bridgesFound.Add(subtree);//found new bridge
						if(bridgesFound.Count == 2)
							break;//found 2 unique bridges
					}
				}
				else
				{
					foreach (Node n in subtree.neighbors)
					{
						if(seen.Contains(n))
							continue;//allready found this node, don't add again
						if(!q.Contains(n))	
							q.Enqueue(n);
					}
					seen.Add(subtree);
				}
			}

			if(bridgesFound.Count != (1 || 2))
				throw new Exception("something went horribly wrong");
			//not same zone as start node and only 1 bridge, therefor endzone
			if(bridgesFound.Count == 1)
				return uint.Parse(bridgesFound[0].id.Split[1]);
			else
			{
				uint bridge1 = uint.Parse(bridgesFound[0].id.Split[1]);
				uint bridge2 = uint.Parse(bridgesFound[1].id.Split[1]);
				if(bridge1<bridge2)
					return bridge2;
				else
					return bridge1;
			}*/
		}
    }

    public class Node
    {
        public Random rnd = new Random();
        public String id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();

        public Node() { }
        public Node(String id) { this.id = id; }

        /* To connect this node to another node. */
        public void connect(Node nd)
        {
            neighbors.Add(nd); nd.neighbors.Add(this);
        }

        /* To disconnect this node from the given node. */
        public void disconnect(Node nd)
        {
            neighbors.Remove(nd); nd.neighbors.Remove(this);
        }

        public bool contested(Player player)
        {
            return (player.location == this && packs.Count > 0);
        }

        /* Execute a fight between the player and the packs in this node.
         * Such a fight can take multiple rounds as describe in the Project Document.
         * A fight terminates when either the node has no more monster-pack, or when
         * the player's HP is reduced to 0. 
         */
        public void fight(Player player)
        {
            /*Possibly to do:
             - Attack after failed flee attempt? (currently present)*/
            //throw new NotImplementedException(); //still missing node contest check
            while (contested(player))
            {                
                Pack targetPack = packs[rnd.Next(packs.Count - 1)];
                Monster targetMon = targetPack.members[rnd.Next(targetPack.members.Count - 1)];
                player.Attack(targetMon);
                if (targetPack.members == null) packs.Remove(targetPack);
                Pack attackPack = packs[rnd.Next(packs.Count - 1)];
                double fleeCheck = rnd.NextDouble();
                if (fleeCheck <= (1 - attackPack.CurrentHP() / attackPack.startingHP) / 2)
                {
                    Logger.log("A pack tries to flee");
                    foreach (Node n in attackPack.location.neighbors)
                    {
                        attackPack.move(n);
                        if (n.packs.Contains(attackPack))
                        {
                            Logger.log("A pack flees!");
                            packs.Remove(attackPack);
                            if(contested(player)) attackPack = packs[rnd.Next(packs.Count - 1)];
                            break;
                        }                        
                    }                  
                }
                attackPack.Attack(player);
            }
        }
    }

    public class Bridge : Node
    {
        List<Node> fromNodes = new List<Node>();
        List<Node> toNodes = new List<Node>();
        public Bridge(String id) : base(id) { }

        /* Use this to connect the bridge to a node from the same zone. */
        public void connectToNodeOfSameZone(Node nd)
        {
            base.connect(nd);
            fromNodes.Add(nd);
        }

        /* Use this to connect the bridge to a node from the next zone. */
        public void connectToNodeOfNextZone(Node nd)
        {
            base.connect(nd);
            toNodes.Add(nd);
        }
    }
}
