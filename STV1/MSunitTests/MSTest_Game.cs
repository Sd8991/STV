using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
	[TestClass]
	public class MSTest_Game
	{
		[TestMethod]
		public void MSTest_PopulateDungeon_controll()
		{
			uint level = 4, m = 4;
			int monsters = 20;
			Dungeon dungeon = new Dungeon(level,m);
			Game game = new Game();
			game.PopulateDungeon(monsters, dungeon);
			int l = dungeon.zone.Count +1;
			for (int i = 1; i < l; i++)
			{
				int monstersAllowedThisZone = (2 * i * monsters) / (int)((dungeon.difficultyLevel + 2) * (dungeon.difficultyLevel + 1));
				//List<Pack> packs = new List<Pack>();
				int monstersInThisZone = 0;
				foreach (Node n in dungeon.zone[i])
					foreach (Pack p in n.packs)
						monstersInThisZone = p.members.Count;
				//assert for every zone number of monsters is correct
				Assert.IsTrue(monstersInThisZone <= monstersAllowedThisZone,"count: " + monstersInThisZone + ", allowed: " + monstersAllowedThisZone);
			}
		}

		[TestMethod]
		public void MSTest_PopulateDungeon_gameConstrucor()
		{
			uint level = 8, m = 5;
			uint monsters = 40;
			Game game = new Game(level, m, monsters);

			int l = game.dungeon.zone.Count + 1;
			for (int i = 1; i < l; i++)
			{
				int monstersAllowedThisZone = (int)((2 * i * monsters) / ((game.dungeon.difficultyLevel + 2) * (game.dungeon.difficultyLevel + 1)));
				//List<Pack> packs = new List<Pack>();
				int monstersInThisZone = 0;
				foreach (Node n in game.dungeon.zone[i])
					foreach (Pack p in n.packs)
						monstersInThisZone = p.members.Count;
				//assert for every zone number of monsters is correct
				Assert.IsTrue(monstersInThisZone <= monstersAllowedThisZone, "count: " + monstersInThisZone + ", allowed: " + monstersAllowedThisZone);
			}
		}
	}
}
