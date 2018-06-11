using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    public class UI
    {
        //char[] drawPlot = new char[200, ]
        List<Tuple<int, int>> positions;
        public UI(Dungeon d)
        {
            positions = new List<Tuple<int, int>>();
        }

        public void drawDungeon(Dungeon d, Player p)
        {
            int h = 1;
            for (int i = 1; i < d.zone[1].Count; i++)
            {
                if (d.zone[1][i - 1].depth == d.zone[1][i].depth)
                    h++;
                else h = 1;
                var cur = Tuple.Create(d.zone[1][i].depth, h);
                if (!positions.Contains(cur))
                    positions.Add(cur);
            }
            positions.Sort();
            for (int y = 1; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    foreach (Tuple<int, int> pos in positions)
                    {
                        var test = Tuple.Create(x, y);
                        if (test.Item1 == pos.Item1 && test.Item2 == pos.Item2)
                            Console.Write(x + "," + y);
                        else Console.Write("       ");
                    }
                }
                Console.Write("\n");
            }
        }

        public void drawUI(Dungeon d, Player p)
        {
            Console.WriteLine("\n**Name: " + p.name + " -- Player HP: " + p.HP + " -- ATK: " + p.AttackRating + " -- Accelerated: " + p.accelerated);
            Console.WriteLine("**Current Zone: " + p.zone + " -- KillPoints: " + p.KillPoint);
            int totalPackHP = 0;
            bool packsAlerted = false;
            foreach (Pack pack in p.location.packs)
            {
                packsAlerted = pack.rAlert;
                totalPackHP += pack.CurrentHP();
            }
            Console.WriteLine("**Packs in Zone: " + p.location.packs.Count + " -- Total HP: " + totalPackHP + " -- Alerted: " + packsAlerted);
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
