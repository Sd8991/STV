using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
	abstract class Specification
	{
		public abstract bool Test(Game g);
	}

	class Always : Specification
	{
		private Predicate<Game> p;
		public Always(Predicate<Game> p) { this.p = p; }
		public override bool Test(Game g){ return p(g); }
	}
	class Unless : Specification
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
				verdict = !previous || (previous && (p(g) || q(g));
			}
			else
				verdict = p(g) && !q(g);
			history.Add(p(g) && !q(g));
			return verdict;
		}
	}
}
