using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
	public abstract class Specification
	{
		public abstract bool Test(Game g);

		public static bool AllNodes(Game g, Predicate<Node> p)
		{
			bool res = true;
			foreach (int key in g.dungeon.zone.Keys)
			{
				List<Node> zone = g.dungeon.zone[key];
				foreach (Node node in zone)
				{
					res = res && p(node);
				}
			}
			return res;
		}

		private static bool AllPacksInNode(Node n, Predicate<Pack> p)
		{
			bool res = true;
			foreach(Pack pack in n.packs)
			{
				res = res && p(pack);
			}
			return res;
		}

		public static bool AllPacks(Game g, Predicate<Pack> p)
		{
			return AllNodes(g, (N => AllPacksInNode(N, p)));
		}
	}

	public class Always : Specification
	{
		private Predicate<Game> p;
		public Always(Predicate<Game> p) { this.p = p; }
		public override bool Test(Game g){ return p(g); }
	}
	public class Unless : Specification
	{
		private Predicate<Game> p;
		private Predicate<Game> q;
		List<bool> history;
		public Unless(Predicate<Game> p, Predicate<Game> q) { this.p = p; this.q = q; }

		public override bool Test(Game g)
		{
			bool verdict;
			if (history.Count >= 1)
			{
				bool previous = history.Last();
				verdict = !previous || (previous && (p(g) || q(g)));
			}
			else
				verdict = p(g) && !q(g);
			history.Add(p(g) && !q(g));
			return verdict;
		}
	}
}
