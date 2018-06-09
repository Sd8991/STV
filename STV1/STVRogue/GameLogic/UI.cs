using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    public class UI
    {
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

        public void drawUI(Dungeon d)
        {

        }
    }
}
