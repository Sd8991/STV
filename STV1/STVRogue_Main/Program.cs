using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;
using STVRogue.Utils;
using System.Diagnostics;

namespace STVRogue
{
    /* A dummy top-level program to run the STVRogue game */
    class Program
    {
        static void Main(string[] args)
        {
            Game game = setupGame();
            RecordGamePlay recording = new RecordGamePlay(game);
            int width = Console.LargestWindowWidth;
            int height = Console.LargestWindowHeight;
            Console.SetWindowSize(width, height);
            while (true)
            {
                if (game.player.location == game.dungeon.exitNode)
                {
                    Logger.log("VICTORY!");
                    break;
                }
                if (game.player.HP <= 0)
                {
                    Logger.log("YOU DIED");
                    break;
                }
                game.ui.drawDungeon(game.dungeon, game.player);
                game.ui.drawUI(game.dungeon, game.player);
                string input = Console.ReadLine();
                if (input == "Quit")
                    break;

                Command command = ParseInput(input, game.player);
                Console.Clear();
                recording.RecordTurn(command);
                game.update(command);
            }
            Logger.log("Do you want to save the recording of the game? {Yes, No}");
            if (Console.ReadLine() == "Yes")
            {
                Logger.log("Save recording as:");
                string filename = Console.ReadLine();
                recording.saveToFile(filename);
            }
        }

		static Game setupGame()
		{
			Logger.log("Give Game Parameters: DifficultyLevel, NodeCapcityMultiplier, NumberOfMonsters, Seed. separeted by spaces or type 'dif' for default values (5 2 20 11)");
			string input = Console.ReadLine();
			if (input == "dif")
				return new Game(5, 2, 20);
			else
				try
				{
					string[] split = input.Split();
					return new Game(uint.Parse(split[0]), uint.Parse(split[1]), uint.Parse(split[2]), int.Parse(split[3]));
				}
				catch (Exception)
				{
					Logger.log("invalidInput");
					return setupGame();
				}
		}

        static Command ParseInput(string input, Player player)
        {
            string[] parse = input.Split();
            int index;
            switch (parse[0])
            {
                case "Move":
                    index = int.Parse(parse[1]) - 1;
                    if (index >= player.location.neighbors.Count)
                    {
                        Logger.log("Invalid destination, choose again");
                        return ParseInput(Console.ReadLine(), player);
                    }
                    return new MoveCommand(index);
                case "Attack":
                    string[] pacMon = parse[1].Split('_');//split into pack id and monster index
                    index = int.Parse(pacMon[1]);
                    Pack pack = player.location.packs.Find(p => p.id == pacMon[0]);//find selected pack
                    Creature target = pack.members.Find(P => P.id == parse[1]);//find selected monster
                        return new AttackCommand(target.id);
                    
                case "Use":
                    try
                    {
                        index = int.Parse(parse[1]);
                        return new ItemCommand(index);
                    }
                    catch
                    {
                        Logger.log("Invalid item, choose again");
                        return ParseInput(Console.ReadLine(), player);
                    }
                case "Nothing":
                    return new Command();
                default:
                    Logger.log("Invalid input, try again (can't quit this turn now)");
                    return ParseInput(Console.ReadLine(), player);
            }
        }
    }
}
