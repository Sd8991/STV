using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
	public abstract class GamePlay
	{
		protected BinaryFormatter formatter;
		protected Game initial;
		protected List<Command> turnList;
		protected Tuple<uint, uint, uint, int, List<Command>> gameTuple;
	}

	public class RecordGamePlay : GamePlay
	{
		public RecordGamePlay(Game g)
		{	
			
			initial = new Game(g.getDificultitylevel,g.getNodeCapcityMultiplier,g.getNumberOfMonsters,g.GetSeed);
			//TODO: make method to check wether 2 games are equel
			//if (g != initial)
				//throw new Exception("Could not recreate same game instance based on parameters");

			turnList = new List<Command>();
			formatter = new BinaryFormatter();
		}

		public void RecordTurn(Command c)
		{
			turnList.Add(c);
		}

		public void saveToFile(string fileName, string filepath = "../../../Recordings/")
		{
			try
			{
				gameTuple = new Tuple<uint, uint, uint, int, List<Command>>(initial.getDificultitylevel, initial.getNodeCapcityMultiplier, initial.getNumberOfMonsters, initial.GetSeed, turnList);
				FileStream writerFileStream = new FileStream(filepath + fileName, FileMode.Create, FileAccess.Write);
				formatter.Serialize(writerFileStream, gameTuple);
				writerFileStream.Close();
			}
			catch (Exception)
			{
				Console.WriteLine("save failed");
				Console.ReadLine();
			}
		}

	}
	public class ReplayGamePlay :GamePlay
	{
		int currentTurn,nTurns;
		string fileName;
		public string FileName { get { return fileName; } }
		Game playing;

		public ReplayGamePlay(string filename, string path = "../../../Recordings/")
		{
			this.fileName = filename;
			formatter = new BinaryFormatter();
			if (File.Exists(path + filename))
			{
				try
				{
					FileStream readerFileStream = new FileStream(path + filename, FileMode.Open, FileAccess.Read);
					gameTuple = (Tuple<uint, uint, uint, int, List<Command>>)this.formatter.Deserialize(readerFileStream);
					readerFileStream.Close();
				}
				catch (Exception)
				{
					Console.WriteLine("could not load file");
				}
				reset();
				turnList = gameTuple.Item5;
				initial = new Game(gameTuple.Item1, gameTuple.Item2, gameTuple.Item3, gameTuple.Item4);
				playing = initial;
				nTurns = turnList.Count();
			}
			else
				Console.WriteLine("file not found");

		}
		public void reset()
		{
			currentTurn = 0;
			playing = initial;
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
			return currentTurn < nTurns;
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
				else
					return false;
			}
			return true;
		}
	}
}
