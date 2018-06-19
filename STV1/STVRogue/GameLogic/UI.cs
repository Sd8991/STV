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
        string mapRow;
        bool safe;

        public UI(Dungeon d, Player p)
        {
            discoveredNodes.Add(p.location);
        }

        public void drawDungeon(Dungeon d, Player p)
        {
            drawPlot = new char[31, 24];
            for (int x = 0; x < 31; x++)
            {
                drawPlot[x, 0] = '='; //upper boundary
                drawPlot[x, 23] = '='; //lower boundary
            }

            //player location node
            for (int x = 10; x <= 17; x++)
            {
                for (int y = 9; y <= 14; y++)
                {
                    if (!(x > 10 && x < 17 && y > 9 && y < 14))
                        drawPlot[x, y] = '#';
                    if (x == 13 && y == 11)
                        drawPlot[x, y] = 'P';
                }
                for (int i = 0; i < p.location.neighbors.Count; i++)
                    drawPlot[16, 10 + i] = (i + 1).ToString()[0];

                if (p.location.id == "startNode")
                {
                    drawPlot[11, 13] = 'S';
                }
                else if (p.location is Bridge)
                {
                    drawPlot[11, 13] = 'B';
                    drawPlot[12, 13] = p.location.id.Split(' ')[1][0];
                }
                else
                {
                    for (int j = 0; j < p.location.id.Count(); j++)
                        drawPlot[11 + j, 13] = p.location.id[j];
                }
            }
            //

            //neighbour 1 (LB)
            if (p.location.neighbors.Count >= 1)
            {
                for (int x = 2; x <= 9; x++)
                {
                    for (int y = 2; y <= 7; y++)
                    {
                        if (!(x > 2 && x < 9 && y > 2 && y < 7))
                            drawPlot[x, y] = '#';
                    }
                    if (p.location.neighbors[0].packs.Count > 0)
                        drawPlot[5, 5] = '!';

                    drawPlot[3, 3] = '1';

                    for (int i = 0; i < p.location.neighbors[0].neighbors.Count; i++)
                        drawPlot[8, 3 + i] = (i + 1).ToString()[0];

                    if (p.location.neighbors[0].id == "startNode")
                    {
                        drawPlot[3, 6] = 'S';
                    }
                    else if (p.location.neighbors[0] is Bridge)
                    {
                        drawPlot[3, 6] = 'B';
                        drawPlot[4, 6] = p.location.neighbors[0].id.Split(' ')[1][0];
                    }
                    else
                    {
                        for (int j = 0; j < p.location.neighbors[0].id.Count(); j++)
                            drawPlot[3 + j, 6] = p.location.neighbors[0].id[j];
                    }
                }
            }
            //

            //neighbour 2 (RB)
            if (p.location.neighbors.Count >= 2)
            {
                for (int x = 18; x <= 25; x++)
                {
                    for (int y = 2; y <= 7; y++)
                    {
                        if (!(x > 18 && x < 25 && y > 2 && y < 7))
                            drawPlot[x, y] = '#';
                    }
                    if (p.location.neighbors[1].packs.Count > 0)
                        drawPlot[21, 5] = '!';

                    drawPlot[19, 3] = '2';

                    for (int i = 0; i < p.location.neighbors[1].neighbors.Count; i++)
                        drawPlot[24, 3 + i] = (i + 1).ToString()[0];

                    if (p.location.neighbors[1].id == "startNode")
                    {
                        drawPlot[19, 6] = 'S';
                    }
                    else if (p.location.neighbors[1] is Bridge)
                    {
                        drawPlot[19, 6] = 'B';
                        drawPlot[20, 6] = p.location.neighbors[1].id.Split(' ')[1][0];
                    }
                    else
                    {
                        for (int j = 0; j < p.location.neighbors[1].id.Count(); j++)
                            drawPlot[19 + j, 6] = p.location.neighbors[1].id[j];
                    }
                }
            }
            //

            //neighbour 3 (LO)
            if (p.location.neighbors.Count >= 3)
            {
                for (int x = 2; x <= 9; x++)
                {
                    for (int y = 15; y <= 20; y++)
                    {
                        if (!(x > 2 && x < 9 && y > 15 && y < 20))
                            drawPlot[x, y] = '#';
                    }
                    if (p.location.neighbors[2].packs.Count > 0)
                        drawPlot[5, 18] = '!';

                    drawPlot[3, 16] = '3';

                    for (int i = 0; i < p.location.neighbors[2].neighbors.Count; i++)
                        drawPlot[8, 16 + i] = (i + 1).ToString()[0];

                    if (p.location.neighbors[2].id == "startNode")
                    {
                        drawPlot[3, 19] = 'S';
                    }
                    else if (p.location.neighbors[2] is Bridge)
                    {
                        drawPlot[16, 19] = 'B';
                        drawPlot[17, 19] = p.location.neighbors[2].id.Split(' ')[1][0];
                    }
                    else
                    {
                        for (int j = 0; j < p.location.neighbors[2].id.Count(); j++)
                            drawPlot[3 + j, 19] = p.location.neighbors[2].id[j];
                    }
                }
            }
            //

            //neighbour 4 (RO)
            if (p.location.neighbors.Count == 4)
            {
                for (int x = 18; x <= 25; x++)
                {
                    for (int y = 15; y <= 20; y++)
                    {
                        if (!(x > 18 && x < 25 && y > 15 && y < 20))
                            drawPlot[x, y] = '#';
                    }
                    if (p.location.neighbors[3].packs.Count > 0)
                        drawPlot[21, 18] = '!';

                    drawPlot[19, 16] = '4';

                    for (int i = 0; i < p.location.neighbors[3].neighbors.Count; i++)
                        drawPlot[24, 16 + i] = (i + 1).ToString()[0];

                    if (p.location.neighbors[3].id == "startNode")
                    {
                        drawPlot[19, 19] = 'S';
                    }
                    else if (p.location.neighbors[3] is Bridge)
                    {
                        drawPlot[19, 19] = 'B';
                        drawPlot[20, 6] = p.location.neighbors[3].id.Split(' ')[1][0];
                    }
                    else
                    {
                        for (int j = 0; j < p.location.neighbors[3].id.Count(); j++)
                            drawPlot[19 + j, 19] = p.location.neighbors[3].id[j];
                    }
                }
            }
            //

            for (int j = 0; j < 24; j++)
            {
                mapRow = "";
                for (int i = 0; i < 31; i++)
                {
                    mapRow += drawPlot[i, j];
                }
                Console.WriteLine(mapRow);
            }

        }

        public void drawUI(Dungeon d, Player p)
        {
            Logger.log("Name: " + p.name + " -- Player HP: " + p.HP + " -- ATK: " + p.AttackRating + " -- Accelerated: " + p.accelerated);
            Logger.log("Current Zone: " + p.zone + "Current Node: " + p.location.id + " -- KillPoints: " + p.KillPoint);
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
            Console.WriteLine();
            if (p.location.contested(p))
            {
                Logger.log("YOU ARE IN COMBAT! DEFEAT OR ROUT THE MONSTERS!");
                Logger.log("Enemies: ");
                foreach (Pack pack in p.location.packs)
                    foreach (Monster m in pack.members)
                        Logger.log(m.name + " (" + m.id + "), HP: " + m.HP);
            }
            else
            {
                safe = true;
                foreach (Node nb in p.location.neighbors)
                    if (nb.contested(p))
                    {
                        Logger.log("This room seems to be safe, but you feel an evil presence in a nearby room...");
                        safe = false;
                        break;
                    }
            }
            if (safe)
            {
                Logger.log("This room seems to be safe...");
            }

            Logger.log("Choose your action: ");
        }
    }
}
