using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
	class GamePlay
	{
		bool fromFile;
		Game initial, playing;
		List<Command> turnList;
		int currentTurn;


		public GamePlay(string file)
		{
			fromFile = true;
			reset();
			throw new NotImplementedException("construct from file");
			turnList = 
			initial = 
			playing = initial;

		}
		/// <summary>
		/// Constructor to record a gameplay
		/// </summary>
		/// <param name="g"></param>
		public GamePlay(Game g)
		{
			fromFile = false;
			initial = g;
			turnList = new List<Command>();
		}

		public void RecordTurn(Command c)
		{
			if (fromFile)
				throw new MethodAccessException("tried to add turn to a recorded gameplay");
			turnList.Add(c);
		}

		public void saveToFile(string fileName, string filepath = "../../")
		{

		}

		public void reset()
		{
			currentTurn = 0;
		}

		public void replayTurn()
		{
			playing.update(turnList[currentTurn]);
			currentTurn++;
		}

		public Game getState()
		{
			return playing;
		}

		private bool hasNextTurn()
		{
			return currentTurn < turnList.Count();
		}

		public bool replay(Specification S)
		{
			reset();
			while (true)
			{
				bool ok = S.Test(getState());
				if (ok)
					if (hasNextTurn())
						replayTurn();
					else
						break;
				return false;
			}
			return true;
		}
	}
}
