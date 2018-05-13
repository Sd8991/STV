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
		public Predicates predicates = new Predicates();
		public uint difficultyLevel;
		/* a constant multiplier that determines the maximum number of monster-packs per node: */
		public uint M;
		public int maxConnectivity = 4;
		public Dictionary<int, List<Node>> zone;
		Random r;

		/// <summary>
		/// empty constructor for testing lose methods
		/// </summary>
		public Dungeon()
		{
			RandomGenerator.initializeWithSeed(1);
			r = RandomGenerator.rnd;
			zone = new Dictionary<int, List<Node>>();
		}

		/* To create a new dungeon with the specified difficult level and capacity multiplier */
		public Dungeon(uint level, uint nodeCapacityMultiplier)
		{
			Logger.log($"Creating a dungeon of difficulty level {level}, node capacity multiplier {nodeCapacityMultiplier}.");
			difficultyLevel = level;
			M = nodeCapacityMultiplier;
			r = RandomGenerator.rnd;
			int seed = (int)level;
			bool ValidGraph = false;
			while (!ValidGraph)
			{
				zone = new Dictionary<int, List<Node>>();
				startNode = new Node("startNode");
				exitNode = new Node("exitNode");

				Node[] bridges = new Node[level + 2];
				bridges[0] = startNode;
				for (int i = 1; i <= level; i++)
				{
					bridges[i] = new Bridge($"bridge {i}");
				}
				bridges[level + 1] = exitNode;

				GenerateDungeonGraph(bridges);
				zone[1].Add(startNode);
				ValidGraph = true; //predicates.isValidDungeon(startNode,exitNode,difficultyLevel);
				if (ValidGraph)
					Logger.log("Valid dungeonGraph");
				else
				{
					Logger.log("Invalid dungeonGraph");
					RandomGenerator.initializeWithSeed(seed);
					r = RandomGenerator.rnd;
					seed++;
				}
			}
		}

		#region DungeonGraph generation
		//builds the dungeon graph by greating subgraphs between the nodes in de brdiges array (which include the start- and exitNode)
		public void GenerateDungeonGraph(Node[] bridges)
		{
			int l = bridges.Length;
			if (l < 2) //sanity check, need atleast 2 nodes to make a valid graph
				throw new Exception("the given Node[] bridges is to short, needs to contain atleast 2 nodes");

			for (int i = 1; i < l; i++)
			{
				GenerateSubGraph(bridges[i - 1], bridges[i], i);
			}

			//TODO: populate dungeon with monsters/packs
			//TODO: populate dungeon with items
		}

		//generate a fully connected subgraph
		public void GenerateSubGraph(Node entryNode, Node endNode, int level, int minNodes = 2, int maxNodes = 10)
		{
			//sanity checks
			if (minNodes < 2)
				throw new Exception("minimum amount of nodes needs to be atleast 2");
			if (minNodes > maxNodes)
				throw new Exception("minNodes (" + minNodes + ") can't be larger than maxNodes (" + maxNodes + ")");


			int n = r.Next(minNodes, maxNodes);
			Node[] newNodes = new Node[n];
			Node[] connectedNodes = new Node[n];
			for (int i = 0; i < n; i++)
			{
				newNodes[i] = new Node("n-"+level+"."+i);//TODO: define ID generation
			}

			//connect the next new node to the already connected nodes (to guarrantee that the graph is full connected)
			for (int i = 0; i < n; i++)
			{
				connectToRandomNodes(newNodes[i], connectedNodes, true);
				connectedNodes[i] = newNodes[i];
			}

			//connect the start and exit node of this subgraph
			connectToRandomNodes(entryNode, connectedNodes, false);
			connectToRandomNodes(endNode, connectedNodes, true);
			List<Node> thisZone = new List<Node>();
			for (int i = 0; i < connectedNodes.Length; i++)
			{
				thisZone.Add(connectedNodes[i]);
			}
			thisZone.Add(endNode);
			zone.Add(level, thisZone);

			correctBridgesSubGraph(entryNode, endNode, connectedNodes);
			//TODO: check for subgraph bridges
		}

		public void correctBridgesSubGraph(Node startnode, Node endnode, Node[] subgraph)
		{
			Predicates p = new Predicates();
			Node[] local = new Node[subgraph.Length + 2];
			for (int i = 0; i < subgraph.Length; i++)
			{
				local[i] = subgraph[i];
			}
			local[subgraph.Length] = startnode;
			local[subgraph.Length + 1] = endnode;

			for (int i = 0; i < local.Length; i++)
			{
				if(p.isBridge(startnode,endnode, local[i]))
				{
					List<Node> around = local[i].neighbors;
					foreach (Node a in around)
						a.neighbors.Remove(local[i]);
					bool solved = false;
					for (int j = 0; j < around.Count; j++)
					{
						if (solved)
							break;
						if (around[j].neighbors.Count >= 3)
							continue;
						for (int k = around.Count-1; k >= j; k--)
						{
							if (around[k].neighbors.Count >= 3)
								continue;
							if (!p.isReachable(around[j], around[k]))
							{
								around[j].connect(around[k]);
								solved = true;
								break;
							}
						}
					}
					if (!solved)
						throw new GameCreationException("could not fix a bridge in an subgraph");
					foreach (Node a in around)
						a.neighbors.Add(local[i]);
				}
			}
		}
		

		public void connectToRandomNodes(Node thisNode, Node[] toNodes, bool sameZone)
		{
			List<Node> validnodes = new List<Node>();	//only use nodes that can still get more connections	
			foreach (Node n in toNodes)
			{
				if (n == null)
					continue;
				if(n.neighbors.Count < 4)
					validnodes.Add(n);//only use nodes that can make a new connection
			}
			if(validnodes.Count == 0)
				return; // no nodes to connect to
			
			int nConnections = 2;
			if( thisNode.GetType() != typeof(Bridge) )
			{
				int actualMaxConnections = Math.Min(validnodes.Count, 2);//correct the maximum number of connections (can't make more connections than nodes available)
				nConnections = r.Next(1,actualMaxConnections);//atleast 1 so the node is actualy getting connected to the rest of the graph
			}
			for (int i = 0; i < nConnections; i++)
			{
				Node chosenNode = randomConnection(thisNode,validnodes,sameZone);
				if(chosenNode!=null)
					validnodes.Remove(chosenNode);//remove so there can't be made a second connection between thisNode and the chosenNode
			}
		}

		public Node randomConnection(Node thisNode, List<Node> toNodes, bool sameZone)
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
					throw new Exception("toNodes contained a node with atleast maxconnectivity neighbors");
			}

			//if thisNode is a bridge, connect to the correct side, else use normal connect
			if(thisNode.GetType() == typeof(Bridge))
				connectBridge((thisNode as Bridge), toNodes[index], sameZone);
			else
				thisNode.connect(toNodes[index]);

			return toNodes[index];//return chosen node
		}

		// connects the correct side of the bridge to a given node
		public void connectBridge(Bridge thisBrige, Node toNode, bool sameZone)
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

		public List<Node> constructPath(Node target, Dictionary<Node,Node> meta)
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
			for (int i = 0; i < b.GetFromNodes.Count; i++)
			{
				b.disconnect(b.GetFromNodes[i]);
			}
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
        public Random rnd = RandomGenerator.rnd;
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
                if (contested(player))
                    attackPack.Attack(player);
            }
        }
    }

    public class Bridge : Node
    {
        List<Node> fromNodes = new List<Node>();
		public List<Node> GetFromNodes {get {return fromNodes;}}
        List<Node> toNodes = new List<Node>();
		public List<Node> GetToNodes { get { return toNodes; } }
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
