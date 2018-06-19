using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

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
				res = res && AllNodesInzone(zone, p);
			}
			return res;
		}

		public static bool AllNodesInzone(List<Node> zone, Predicate<Node> p)
		{
			bool res = true;
			foreach (Node node in zone)
			{
				res = res && p(node);
			}
			return res;
		}

		public static bool AllPacksInNode(Node n, Predicate<Pack> p)
		{
			bool res = true;
			foreach(Pack pack in n.packs)
			{
				res = res && p(pack);
				if (res == false)
					Logger.log($"id:{pack.id} location:{pack.location.id} zone:{pack.zone}");
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
		public Unless(Predicate<Game> p, Predicate<Game> q) { this.p = p; this.q = q; history = new List<bool>(); }

		public override bool Test(Game g)
		{
			bool verdict;
			if (history.Count >= 1)
			{
				bool previous = history.Last();
				verdict = !previous || (previous && (p(g) || q(g)));
			}
			else
				verdict = p(g) || q(g);
			history.Add(p(g) && !q(g));
			return verdict;
		}
	}

	public class Allerted : Specification
	{
		Predicate<Node> pNodes = (N => Specification.AllPacksInNode(N, (P => P.rAlert)));
		Unless unless;
		int zone;

		public Allerted()
		{
			zone = 1;
			Predicate<Game> p = (G => G.player.inCombat || Specification.AllNodesInzone(G.dungeon.zone[zone], pNodes));
			Predicate<Game> q = (G => G.player.zone != zone);
			unless = new Unless(p, q);
		}

		public override bool Test(Game g)
		{
			bool res = unless.Test(g);
			zone = g.player.zone;
			return res;
		}
	}

	public class PackMovementLastZone : Specification
	{
		Unless unless;
		Predicate<Game> playerLastZone = (Q => Q.dungeon.zone[Q.dungeon.zone.Count].Contains(Q.player.location));
		Predicate<Pack> validPos = (P => (P.location == previousPos[P]) || (P.location == allowedPos[P]));
		static Dictionary<Pack, Node> previousPos, allowedPos, currentPos;

		public PackMovementLastZone()
		{
			Predicate<Game> p = (P => playerLastZone(P) && allPacksOk());
			Predicate<Game> q = (Q => !playerLastZone(Q));
			unless = new Unless(p, q);
		}
		
		private bool allPacksOk()
		{
			bool res = true;
			foreach (Pack pack in currentPos.Keys)
			{
				res = res && validPos(pack);
			}
			return res;
		}

		private Dictionary<Pack, Node> fillPos(Game g)
		{
			Dictionary<Pack,Node> res = new Dictionary<Pack, Node>();
			List<Node> zone = g.dungeon.zone[g.dungeon.zone.Count()];
			foreach (Node n in zone)
			{
				foreach (Pack pack in n.packs)
				{
					res[pack] = n;
				}
			}
			return res;
		}
		private void fillAllowed(Game g)
		{
			allowedPos = new Dictionary<Pack, Node>();
			foreach (Pack pack in previousPos.Keys)
			{
				Node next = g.dungeon.shortestpath(previousPos[pack], g.player.location).First();
				allowedPos[pack] = next;
			}
		}

		private void setup(Game g)
		{
			previousPos = fillPos(g);
			fillAllowed(g);
		}

		public override bool Test(Game g)
		{
			currentPos = fillPos(g);
			if (previousPos == null) setup(g);
			bool res = unless.Test(g);
			previousPos = currentPos;
			fillAllowed(g);
			return res;
		}
	}
}
