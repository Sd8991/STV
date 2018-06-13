using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Pack
    {
        public String id;
        public List<Monster> members = new List<Monster>();
        public int startingHP = 0;
        public Node location;
        public Dungeon dungeon;
        public Random rnd;
        public bool rAlert = false;
        public bool rLastZone = false;

        public Pack(String id, uint n)
        {
            this.id = id;
            for (int i = 0; i < n; i++)
            {
                Monster m = new Monster("" + id + "_" + i, this);
                members.Add(m);
                startingHP += m.HP;
            }
        }

        public int CurrentHP()
        {
            int currentHP = 0;
            foreach (Monster m in members)
            {
                currentHP += m.HP;
            }
            return currentHP;
        }

        public void Attack(Player p)
        {
            foreach (Monster m in members)
            {
                m.Attack(p);
                if (p.HP == 0) break;
            }
        }

        /* Move the pack to an adjacent node. */
        public void move(Node u)
        {
            if (u == location)
            {
                Logger.log("Pack" + id + " doesn't move.");
                return;
            }
            if (!location.neighbors.Contains(u)) throw new ArgumentException();
            int capacity = (int) (dungeon.M * (dungeon.level(u) + 1));
            // count monsters already in the node:
            foreach (Pack Q in u.packs) {
                capacity = capacity - Q.members.Count;
            }
            // capacity now expresses how much space the node has left
            if (members.Count > capacity)
            {
                Logger.log("Pack " + id + " is trying to move to a full node " + u.id + ", but this would cause the node to exceed its capacity. Rejected.");
                return;
            }
            location = u;
            u.packs.Add(this);          
        }

        /* Move the pack one node further along a shortest path to u. */
        public void moveTowards(Node u)
        {
            List<Node> path = dungeon.shortestpath(location, u);
            move(path[0]);
        }

        public bool rZone(Node u)
        {
            if (u is Bridge)    // <---
            {
                if (!(u as Bridge).GetFromNodes.Contains(location)) return false;
            }
            if (location is Bridge) // --->
            {
                if (!(location as Bridge).GetToNodes.Contains(u)) return false;
            }
            return true;
        }

        public Node chooseDestination(Player p, Random rnd)
        {
            List<Node> dest = new List<Node>();
            if (!rLastZone) dest.Add(location); //pack can't stand still if rLastZone is active
            if (!rLastZone && !rAlert)          //free movement if none of the rules apply
                foreach (Node n in location.neighbors) if (rZone(n)) dest.Add(n);
            else dest.Add(dungeon.shortestpath(location, p.location)[0]); //add direction of player if either rule applies
            int destIndex = rnd.Next(dest.Count - 1);
            return dest[destIndex];
        }
    }
}
