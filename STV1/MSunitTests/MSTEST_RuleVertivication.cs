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
		static string[] files = new[] {
			//"FullPlayThrough.dat",
			//"PlayerDied.dat",
			//"PickUpUsePotion.dat",
			//"UsePotionInCombat.dat",
			//"CombatMultipleEnemies.dat",
			//"NoCombat.dat",
			//"AllValidUsesOfCrystal.dat",
			"TriedToFleeToFullNode.dat"
		};
		
		[TestMethod]
		public void test_player_hp_never_Negative()
		{
			List<ReplayGamePlay> plays = loadSavedGamePLays(files);
			foreach (ReplayGamePlay gp in plays)
			{
				Assert.IsTrue(gp.replay(new Always(G => G.player.HP >= 0)), gp.FileName + " failed");
			}
		}

		[TestMethod]
		public void test_node_never_over_capacity()
		{
			List<ReplayGamePlay> plays = loadSavedGamePLays(files);
			foreach (ReplayGamePlay gp in plays)
			{
				Always al = new Always(G => Specification.AllNodes(G, (N => N.nMonsters() <= G.dungeon.M * (G.dungeon.level(N) + 1))));
				Assert.IsTrue(gp.replay(al),gp.FileName + " failed");
			}
		}

		[TestMethod]
		public void test_packs_never_leave_zone()
		{
			List<ReplayGamePlay> plays = loadSavedGamePLays(files);
			foreach (ReplayGamePlay gp in plays)
			{
				Always al = new Always(G => Specification.AllPacks(G, (P => (G.dungeon.zone[P.zone]).Contains(P.location))));
				Assert.IsTrue(gp.replay(al), gp.FileName + " failed");
			}
		}

		[TestMethod]
		public void test_end_zone()
		{
			List<ReplayGamePlay> plays = loadSavedGamePLays(files);
			foreach (ReplayGamePlay gp in plays)
			{
				Assert.IsTrue(gp.replay(new PackMovementLastZone()), gp.FileName + " failed");
			}
		}

		[TestMethod]
		public void test_alerted()
		{
			List<ReplayGamePlay> plays = loadSavedGamePLays(files);
			foreach (ReplayGamePlay gp in plays)
			{
				Assert.IsTrue(gp.replay(new Allerted()), gp.FileName + " failed");
			}
		}
	}
}
