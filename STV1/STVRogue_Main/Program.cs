﻿using System;
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
            Game game = new Game(5, 2, 20);
			RecordGamePlay recording = new RecordGamePlay(game);
            int width = Console.LargestWindowWidth;
            int height = Console.LargestWindowHeight;
            Console.SetWindowSize(width, height);
            while (true)
            {
				if(game.player.location == game.dungeon.exitNode)
				{
					Logger.log("VICTORY!");
					break;
				}
                game.ui.drawDungeon(game.dungeon, game.player);
                game.ui.drawUI(game.dungeon, game.player);
				string input = Console.ReadLine();
				if (input == "Quit")
					break;
				Command command = ParseInput(input,game.player);
                Console.Clear();
                recording.RecordTurn(command);
                game.update(command);
            }
			Logger.log("Do you want to save the recording of the game? {Yes, No}");
			if(Console.ReadLine() == "Yes")
			{
				Logger.log("Save recording as:");
				string filename = Console.ReadLine();
				recording.saveToFile(filename);
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
                        return new MoveCommand(index);
				case "Attack":
                        string[] pacMon = parse[1].Split('_');//split into pack id and monster index
                        index = int.Parse(pacMon[1]);
                        Pack pack = player.location.packs.Find(p => p.id == pacMon[0]);//find selected pack
                        Creature target = pack.members[index];//find selected monster
                    try
                    {
                        return new AttackCommand(target.id);
                    }
                    catch
                    {
                        Logger.log("Invalid monster, choose again");
                        return ParseInput(Console.ReadLine(), player);
                    }
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
					return ParseInput(Console.ReadLine(),player);					
			}
		}
    }
}
