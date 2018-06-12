using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;
using System.Diagnostics;

namespace STVRogue
{
    /* A dummy top-level program to run the STVRogue game */
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(5, 2, 20);
			//RecordGamePlay recording = new RecordGamePlay(game);
            int width = Console.LargestWindowWidth;
            int height = Console.LargestWindowHeight;
            Console.SetWindowSize(width, height);
            while (true)
            {
                game.ui.drawUI(game.dungeon, game.player);
				string input = Console.ReadLine();
				Command command = ParseInput(input);
				//recording.RecordTurn(command);
                game.update(command);
            }
        }

		static Command ParseInput(string input)
		{
			return new Command();
		}
    }
}
