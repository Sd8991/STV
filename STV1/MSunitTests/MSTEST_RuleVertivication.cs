using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
	[TestClass]
	public class MSTEST_RuleVertivication
	{
		static List<ReplayGamePlay> loadSavedGamePLays(string[] files)
		{
			List<ReplayGamePlay> plays = new List<ReplayGamePlay>();
			foreach (string play in files)
			{
				plays.Add(new ReplayGamePlay(play));
			}
			return plays;
		}
		static string[] files = new[] {"Test.dat"};//TODO: fill this with real files
		
		[TestMethod]
		public void test_player_hp_never_Negative()
		{
			List<ReplayGamePlay> plays = loadSavedGamePLays(files);
			foreach (ReplayGamePlay gp in plays)
			{
				Assert.IsTrue(gp.replay(new Always(G => G.player.HP >= 0)));
			}
		}
	}
}
