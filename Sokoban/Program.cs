using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban
{
	class Program
	{
		static void Main(string[] args)
		{
			int[,] availableMatrix = 
			{
				{4,0,0,0},
				{1,2,0,1},
				{0,0,0,8},
				{0,2,4,1}
			};
			SokobanState initState = new SokobanState(availableMatrix);
			HashSet<SokobanState> stateSet = new HashSet<SokobanState>();
			Queue<SokobanState> stateQueue = new Queue<SokobanState>();
			stateSet.Add(initState);
			stateQueue.Enqueue(initState);

			while (stateQueue.Count > 0)
			{
				SokobanState currentState = stateQueue.Dequeue();

				if (currentState.IsSolved())
				{
					System.Console.WriteLine("GOAL!");
					break;
				}

				HashSet<SokobanState> expandStateSet = currentState.moveAll();
				foreach (SokobanState expandState in expandStateSet)
				{
					if (stateSet.Add(expandState))
					{
						stateQueue.Enqueue(expandState);
					}
				}
			}
			System.Console.ReadKey();
		}
	}
}
