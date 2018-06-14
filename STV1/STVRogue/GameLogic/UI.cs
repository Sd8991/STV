using System;
using System.Collections.Generic;
using STVRogue.Utils;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    public class UI
    {
        char[,] drawPlot;
        List<Node> discoveredNodes = new List<Node>();
        public Dictionary<int, int> heightCounter = new Dictionary<int, int>();
        public Dictionary<Tuple<int, int>, List<Node>> DepthHeight = new Dictionary<Tuple<int, int>, List<Node>>();

        public UI(Dungeon d, Player p)
        {
            p.location.depth = 0;
            p.location.height = 0;
            heightCounter[p.location.depth] = p.location.height;
            try { DepthHeight[new Tuple<int, int>(p.zone, p.location.depth)].Add(p.location); }
            catch { DepthHeight.Add(new Tuple<int, int>(p.zone, p.location.depth), new List<Node>()); DepthHeight[new Tuple<int, int>(p.zone, p.location.depth)].Add(p.location); }
            discoveredNodes.Add(p.location);
            /*foreach (Node nb in p.location.neighbors)
            {
                nb.depth = p.location.depth + 1;
                discoveredNodes.Add(nb);
                try { nb.height = heightCounter[nb.depth]; }
                catch { heightCounter[nb.depth] = 0; nb.height = heightCounter[nb.depth]; }
                try { DepthHeight[new Tuple<int, int>(p.zone, nb.depth)].Add(nb); }
                catch { DepthHeight.Add(new Tuple<int, int>(p.zone, nb.depth), new List<Node>()); DepthHeight[new Tuple<int, int>(p.zone, nb.depth)].Add(nb); }
                heightCounter[nb.depth] += 1;
            }*/
            drawPlot = new char[200, 51];
            for (int i = 0; i < 200; i++)
            {
                drawPlot[i, 0] = '#';
                drawPlot[i, 50] = '#';
            }
        }

        public void drawDungeon(Dungeon d, Player p)
        {
            foreach (Node nb in p.location.neighbors)
            {
                nb.depth = Math.Min(nb.depth, p.location.depth + 1);
                if (!discoveredNodes.Contains(nb))
                    discoveredNodes.Add(nb);
                try { nb.height = heightCounter[nb.depth]; }
                catch { heightCounter[nb.depth] = 0; nb.height = heightCounter[nb.depth]; }
                try { DepthHeight[new Tuple<int, int>(p.zone, nb.depth)].Add(nb); }
                catch { DepthHeight.Add(new Tuple<int, int>(p.zone, nb.depth), new List<Node>()); DepthHeight[new Tuple<int, int>(p.zone, nb.depth)].Add(nb); }
                heightCounter[nb.depth] += 1;
            }

            foreach (Node node in discoveredNodes)
            {
                char nodeNr;
                for (int y = 0; y <= 8; y++)
                    for (int x = 0; x <= 8; x++)
                        if (y > 0 && y < 8 && x > 0 && x < 8)
                        {
                            if (node.id == "startNode")
                                nodeNr = 'S';
                            else nodeNr = node.id.Split('_')[1][0];
                            if (x == 1 && y == 1)
                                drawPlot[x + (9 + 3) * node.depth, y + 3 + 10 * node.height] = nodeNr;
                            if (p.location == node)
                                drawPlot[4 + (9 + 5) * node.depth, 5 + 3 + 13 * node.height] = 'P';
                        }
                        else
                            drawPlot[x + (9 + 3) * node.depth , y + 3 + 10 * node.height] = '*';
                int nbC = 0;
                foreach (Node nb in node.neighbors)
                {
                    if (nb.id == "startNode")
                        nodeNr = 'S';
                    else if (nb.id.Contains("bridge"))
                        nodeNr = 'B';
                    else nodeNr = nb.id.Split('_')[1][0];
                    drawPlot[7 + (9 + 3) * node.depth, nbC * 2 + 4 + 10 * node.height] = nodeNr;
                    nbC++;
                }
                if (node.packs.Count > 0)
                    drawPlot[4 + (9 + 3) * node.depth, 4 * 2 + 4 + 10 * node.height] = '!';
            }
            for (int y = 0; y < 51; y++)
                for (int x = 0; x < 200; x++)
                    Console.Write(drawPlot[x, y]);
        }

        public void drawUI(Dungeon d, Player p)
        {
            Logger.log("Name: " + p.name + " -- Player HP: " + p.HP + " -- ATK: " + p.AttackRating + " -- Accelerated: " + p.accelerated);
            Logger.log("Current Zone: " + p.zone + " -- KillPoints: " + p.KillPoint);
            int totalPackHP = 0;
            bool packsAlerted = false;
            foreach (Pack pack in p.location.packs)
            {
                packsAlerted = pack.rAlert;
                totalPackHP += pack.CurrentHP();
            }
            Logger.log("Packs in Zone: " + p.location.packs.Count + " -- Total HP: " + totalPackHP + " -- Alerted: " + packsAlerted);
            Console.Write("** Items: ");
            int counter = 0;
            if (p.bag.Count == 0) Console.Write("Empty");
            foreach (Item i in p.bag)
            {
                if (counter == 2)
                {
                    Console.Write("\n");
                    counter = 0;
                }
                if (i is HealingPotion)
                {
                    HealingPotion I = (HealingPotion)i;
                    Console.Write(i.id + "(" + I.HPvalue + "), ");
                }
                if (i is Crystal)
                {
                    Crystal I = (Crystal)i;
                    Console.Write(i.id + ", ");
                }
                counter++;
            }
            Logger.log("Choose your action: ");
        }
    }
}
