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
		public Random r;


		/// <summary>
		/// empty constructor for testing lose methods
		/// </summary>
		public Dungeon()
		{
			RandomGenerator.initializeWithSeed(1);
			r = RandomGenerator.rnd;
			zone = new Dictionary<int, List<Node>>();
		}

        // To create a consistent dummy dungeon to test Pack movement on
        public Dungeon(uint M)
        {
            this.M = M;
            Node N0 = new Node("N0");
            Node N1 = new Node("N1");
            Node N2 = new Node("N2");
            Node N3 = new Node("N3");      
            N0.connect(N1);
            N0.connect(N2);
            N1.connect(N2);
            List<Node> nodes = new List<Node>();
            nodes.Add(N0);
            nodes.Add(N1);
            nodes.Add(N2);
            nodes.Add(N3);
            zone = new Dictionary<int, List<Node>>();
            zone.Add(1, nodes);
        }

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier, int seed = 11)
        {
            Logger.log($"Creating a dungeon of difficulty level {level}, node capacity multiplier {nodeCapacityMultiplier}.");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;
			RandomGenerator.initializeWithSeed(seed);
            r = RandomGenerator.rnd;
			//int seed = (int)level;
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
				/*else
				{
					Logger.log("Invalid dungeonGraph");
					RandomGenerator.initializeWithSeed(seed);
					r = RandomGenerator.rnd;
					seed++;
				}*/
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
		public void GenerateSubGraph(Node entryNode, Node endNode, int level, int minNodes = 2, int maxNodes = 20)
		{
			//sanity checks
			if (minNodes < 2)
				throw new Exception("minimum amount of nodes needs to be atleast 2");
			if (minNodes > maxNodes)
				throw new Exception("minNodes (" + minNodes + ") can't be larger than maxNodes (" + maxNodes + ")");
			int min = Math.Max(1, minNodes / 2);
			int max = Math.Max(min, maxNodes / 2);

			int n1 = r.Next(min, max);
			int n2 = r.Next(min, max);

			Node[] path1 = genSubPath(n1, entryNode, endNode);
			Node[] path2 = genSubPath(n2, entryNode, endNode);

			Node[] subgraph = new Node[n1+n2];
			Array.Copy(path1, subgraph, path1.Length);
			Array.Copy(path2, 0, subgraph, path1.Length, path2.Length);
			int total = 2 + n1 + n2;
			float avr = (8 + n1 * 2 + n2 * 2) / total;

			int additonalConnections = (int)((3.0f - avr) * total);

			for (int i = 0; i < additonalConnections; i++)
			{
				int rc = r.Next(total - 2);
				if (subgraph[rc].neighbors.Count < 4)
					randomConnection(subgraph[rc], subgraph);
			}

			zone[level] = new List<Node>();
			for (int i = 0; i < subgraph.Length; i++)
			{
				zone[level].Add(subgraph[i]);
			}
			zone[level].Add(endNode);
		}

		public Node[] genSubPath(int n,Node entryNode, Node endNode)
		{
			Node[] path = new Node[n];
			path[0] = new Node();
			for (int i = 1; i < n; i++)
			{
				path[i] = new Node();
				path[i].connect(path[i - 1]);
			}

			if (entryNode.GetType() == typeof(Bridge))
				connectBridge((entryNode as Bridge), path[0], false);
			else
				entryNode.connect(path[0]);

			if (endNode.GetType() == typeof(Bridge))
				connectBridge((endNode as Bridge), path[n-1], true);
			else
				endNode.connect(path[n-1]);
			return path;
		}
		
		public void randomConnection(Node thisNode, Node[] toNodes)
		{
			int l = toNodes.Length;
			if (l == 0)
				return; // no nodes to connect to (this is a valid situation)

			if (thisNode.neighbors.Count >= maxConnectivity)
				return;// thisNode is not allowed to have more neigbours
			
			int index = 0;
			index = r.Next(l - 1);
			if (toNodes[index] == thisNode)
				return;
			if (toNodes[index].neighbors.Count >= maxConnectivity)
				return;
			thisNode.connect(toNodes[index]);
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
		}
    }

    public class Node
    {
        public Random rnd;
        public String id;
        public int seed;
        public bool withSeed;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();

        public Node() { }
        public Node(String id) { this.id = id; }

        public Node(int seed) { this.seed = seed; withSeed = true; }

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
            bool playerishere = (player.location == this);
            bool hasPacks = (packs.Count > 0);
            return (playerishere && hasPacks);
        }

        /* Execute a fight between the player and the packs in this node.
         * Such a fight can take multiple rounds as describe in the Project Document.
         * A fight terminates when either the node has no more monster-pack, or when
         * the player's HP is reduced to 0. 
         */
        public void fight(Player player, int seed, bool withSeed)
        {
            triggerAlert(player.dungeon.zone[player.zone]);
            if (withSeed) RandomGenerator.initializeWithSeed(seed);
            rnd = RandomGenerator.rnd;
            /*Possibly to do:
             - Attack after failed flee attempt? (currently present)*/
            //throw new NotImplementedException(); //still missing node contest check
            while (contested(player))
            {
                player.GetNextCommand();
                if (!contested(player)) break;
                Pack attackPack = packs[rnd.Next(packs.Count - 1)];
                double fleeCheck = rnd.NextDouble();
                double packHP = attackPack.CurrentHP();
                double fleeTreshold = (1 - (packHP / attackPack.startingHP)) / 2;
                if (fleeCheck <= fleeTreshold)
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

        public void triggerAlert(List<Node> nodes)
        {
            foreach (Node n in nodes)
                foreach (Pack p in n.packs)
                    p.rAlert = true;
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
