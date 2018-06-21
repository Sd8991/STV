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
        public UI ui;
        public Player player;
        public Dungeon dungeon;
        public uint potionHP;
		public Random r;
		uint difficultyLevel, nodeCapcityMultiplier, numberOfMonsters;
		int seed;
		public int GetSeed { get { return seed; } }
		public uint getDificultitylevel { get { return difficultyLevel; } }
		public uint getNodeCapcityMultiplier { get { return nodeCapcityMultiplier; } }
		public uint getNumberOfMonsters { get { return numberOfMonsters; } }

		/* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */
		/// <summary>
		/// empty constructor to test methods
		/// </summary>
		public Game() { }
        public Game(uint difficultyLevel, uint nodeCapcityMultiplier, uint numberOfMonsters, int seed = 11)
        {
            Logger.log("Creating a game of difficulty level " + difficultyLevel + ", node capacity multiplier "
                       + nodeCapcityMultiplier + ", and " + numberOfMonsters + " monsters.");
			this.difficultyLevel = difficultyLevel;
			this.nodeCapcityMultiplier = nodeCapcityMultiplier;
			this.numberOfMonsters = numberOfMonsters;
			this.seed = seed;
            player = new Player();
			dungeon = new Dungeon(difficultyLevel, nodeCapcityMultiplier,seed);
			r = dungeon.r;
            int monsterHP = 0;
			PopulateDungeon((int)numberOfMonsters, ref monsterHP);
            DistributePotions(player, (uint)monsterHP);
            player.location = dungeon.startNode;
            player.dungeon = dungeon;
            ui = new UI(dungeon, player);
        }

		public void PopulateDungeon(int monsters, ref int monsterHP)
		{
			PopulateDungeon(monsters, ref monsterHP, dungeon);
		}

		public void PopulateDungeon(int monsters, ref int monsterHP, Dungeon dungeon)
		{
			int monstersLeft = monsters;
			int l = dungeon.zone.Count+1;

			for (int i = 1; i < l; i++)
			{
				int rMax = (int)dungeon.M + 1;
				List<Node> curZone = dungeon.zone[i];
				int monstersThisZone = (2*i*monsters)/(int)((dungeon.difficultyLevel+2)*(dungeon.difficultyLevel+1));
				if(monstersThisZone>monstersLeft)
				{
					monstersThisZone = monstersLeft;
					monstersLeft = 0;
				}
				else
				{
					monstersLeft -= monstersThisZone;
				}
				//if (monstersThisZone > monstersLeft)
				//	monstersThisZone = monstersLeft;
				//monstersLeft -= monstersThisZone;
				int j = 1;
				string idPrefix = "Pack-" + i + ".";
				while(monstersThisZone > 0)
				{
					if (rMax > monstersThisZone)
						rMax = monstersThisZone;
					int nPack = r.Next(1,rMax);
					int index = r.Next(curZone.Count);
					if (curZone[index] == dungeon.exitNode)
						continue;//not allowed to drop in exitnode, skip

					int capacity = (int)(dungeon.M * (dungeon.level(curZone[index]) + 1));
					// count monsters already in the node:
					foreach (Pack Q in curZone[index].packs)
					{
						capacity = capacity - Q.members.Count;
					}

					if(capacity >= nPack)//if there is enough space, make and drop new pack
					{
						Pack pack = new Pack(idPrefix + j,(uint)nPack);
						j++;
						monstersThisZone -= nPack;
                        pack.location = curZone[index];
                        pack.dungeon = dungeon;
						pack.zone = i;
						curZone[index].packs.Add(pack);
                        monsterHP += pack.startingHP; 
					}
				}
			}
		}

        public uint DistributePotions(Player P, uint monsterHP)
        {
            DistributePotions(P, monsterHP, dungeon);
            return this.potionHP;
        }

        public void DistributePotions(Player P, uint monsterHP, Dungeon D)
        {
            this.potionHP = (uint)P.HP;
            HealingPotion pot;
            List<HealingPotion> pots = new List<HealingPotion>();
            int index;
            List<Node> zone;

            while (this.potionHP <= 0.8f * monsterHP)
            {
                pot = new HealingPotion("Minor Healing Potion of Major Healing" + pots.Count);
                if (this.potionHP + pot.HPvalue <= 0.8f * monsterHP)
                {
                    pots.Add(pot);
                    this.potionHP += pot.HPvalue;
                    pot.id += '(' + pot.HPvalue + ')';
                }
                else break;
            }
            foreach (HealingPotion p in pots)
            {
                zone = dungeon.zone[r.Next(dungeon.zone.Count) + 1];
                index = r.Next(0, zone.Count);
                zone[index].items.Add(p);
            }
        }

        /*
         * A single update turn to the game. 
         */
        public Boolean update(Command userCommand)
        {
            Logger.log("Player does " + userCommand);
			userCommand.ExecuteCommand(player);
            if (player.inCombat) player.location.fight(player, seed, true);
            else
            {
                List<Pack> toMove = new List<Pack>();
                foreach (List<Node> l in dungeon.zone.Values)
                {
                    foreach (Node n in l)
                    {
                        foreach (Pack p in n.packs)
                        {
                            toMove.Add(p);
                        }
                    }
                }
                foreach (Pack p in toMove)
				{
					p.move(p.chooseDestination(player, r));
				}
            }
            return true;
        }
    }

    public class GameCreationException : Exception
    {
        public GameCreationException() { }
        public GameCreationException(String explanation) : base(explanation) { }
    }
}
