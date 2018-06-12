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

        List<Tuple<int, int>> positions;
        public UI(Dungeon d, Player p)
        {
            p.location.depth = 0;
            discoveredNodes.Add(p.location);
            foreach (Node nb in p.location.neighbors)
            {
                nb.depth = p.location.depth + 1;
                discoveredNodes.Add(nb);
            }
            drawPlot = new char[200, 51];
            for (int i = 0; i < 200; i++)
            {
                drawPlot[i, 0] = '#';
                drawPlot[i, 50] = '#';
            }
            positions = new List<Tuple<int, int>>();
        }

        public void drawDungeon(Dungeon d, Player p)
        {
            foreach (Node node in discoveredNodes)
            {
                for (int y = 0; y <= 6; y++)
                    for (int x = 0; x <= 6; x++)
                        if (y > 0 && y < 6 && x > 0 && x < 6)
                        { }
                        else drawPlot[x, y] = '*';


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
            Console.Write("**Items: ");
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

        }
    }
}
