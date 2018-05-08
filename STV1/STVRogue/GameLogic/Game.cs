using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Game
    {
        public Player player;
        public Dungeon dungeon;

        /* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */
        public Game(uint difficultyLevel, uint nodeCapcityMultiplier, uint numberOfMonsters)
        {
            Logger.log("Creating a game of difficulty level " + difficultyLevel + ", node capacity multiplier "
                       + nodeCapcityMultiplier + ", and " + numberOfMonsters + " monsters.");
            player = new Player();
        }

		private void PopulateDugeon(Dictionary<int,List<Node>> zones, int monsters)
		{
			int monstersLeft = monsters;
			int l = zones.Count - 1;

			Random r = RandomGenerator.rnd;
			int rMax = dungeon.M;

			for (int i = 0; i < l; i++)
			{
				List<Node> curZone = zones[i];
				int monstersThisZone = (2*i*monsters)/((dungeon.difficultyLevel+2)*(dungeon.difficultyLevel+1));
				monstersLeft -= monstersThisZone;
				int j = 1;
				string idPrefix = "Pack-" + i + ".";
				while(monsters>0)
				{
					uint nPack = r.Next(0,rMax);
					Pack pack = new Pack(idPrefix + j,nPack);
					j++;
					monsters-nPack;

				}

			}
		}

        /*
         * A single update turn to the game. 
         */
        public Boolean update(Command userCommand)
        {
            Logger.log("Player does " + userCommand);
            return true;
        }
    }

    public class GameCreationException : Exception
    {
        public GameCreationException() { }
        public GameCreationException(String explanation) : base(explanation) { }
    }
}
