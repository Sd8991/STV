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

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier)
        {
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;
            throw new NotImplementedException();
        }

        /* Return a shortest path between node u and node v */
        public List<Node> shortestpath(Node u, Node v) { throw new NotImplementedException(); }


        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");
            throw new NotImplementedException();
        }

        /* To calculate the level of the given node. */
        public uint level(Node d) { throw new NotImplementedException(); }
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
