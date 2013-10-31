using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sokoban
{
	class SokobanState
	{
		protected const int WALL = 0x01;
		protected const int BOX = 0x02;
		protected const int TARGET = 0x04;
		protected const int PERSON = 0x08;

		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType() == typeof(SokobanState))
			{
				HashSet<SokobanMovement>objMovementSet = ((SokobanState)obj).movementSet;
				if (movementSet.Count != objMovementSet.Count)
				{
					return false;
				}
				bool result = true;
				foreach (SokobanMovement movement in movementSet)
				{
					if (!objMovementSet.Contains(movement))
					{
						result = false;
						break;
					}
				}
				if (result == true)
				{
					int[,] objMatrix = ((SokobanState)obj).matrix;
					int xCount = matrix.GetLength(0);
					int yCount = matrix.GetLength(1);
					for (int x = 0; x < xCount && result == true; x++)
					{
						for (int y = 0; y < yCount && result == true; y++)
						{
							if (matrix[x, y] == objMatrix[x, y])
							{
								result = false;
							}
						}
					}
				}
				return result;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int code = 0;
			foreach (SokobanMovement movement in movementSet)
			{
				code += movement.GetHashCode();
			}
			return code;
		}

		public override string ToString()
		{
			string result = "";
			int xCount = matrix.GetLength(0);
			int yCount = matrix.GetLength(1);
			for (int y = 0; y < yCount; y++)
			{
				for (int x = 0; x < xCount; x++)
				{
					result += matrix[x, y] + " ";
				}
				result += "\n";
			}
			return result;
		}

		public bool IsSolved()
		{
			bool result = true;
			int xCount = matrix.GetLength(0);
			int yCount = matrix.GetLength(1);
			for (int x = 0; x < xCount; x++)
			{
				for (int y = 0; y < yCount; y++)
				{
					if ((matrix[x, y] & BOX) == BOX && (matrix[x, y] & TARGET) == 0)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		public HashSet<SokobanState> moveAll()
		{
			HashSet<SokobanState> resultSet = new HashSet<SokobanState>();
			foreach (SokobanMovement movement in movementSet)
			{
				resultSet.Add(move(movement));
			}
			return resultSet;
		}

		protected SokobanState move(SokobanMovement movement)
		{
			int[,] newMatrix = (int[,])matrix.Clone();
			int personX = -1;
			int personY = -1;
			switch(movement.direction)
			{
				case SokobanMovement.Direction.LEFT:
					newMatrix[movement.point.x, movement.point.y] &= ~BOX;
					newMatrix[movement.point.x - 1, movement.point.y] |= BOX;
					newMatrix[person.x, person.y] &= ~PERSON;
					newMatrix[movement.point.x, movement.point.y] |= PERSON;
					personX = movement.point.x;
					personY = movement.point.y;
					break;
				case SokobanMovement.Direction.RIGHT:
					newMatrix[movement.point.x, movement.point.y] &= ~BOX;
					newMatrix[movement.point.x + 1, movement.point.y] |= BOX;
					newMatrix[person.x, person.y] &= ~PERSON;
					newMatrix[movement.point.x, movement.point.y] |= PERSON;
					personX = movement.point.x;
					personY = movement.point.y;
					break;
				case SokobanMovement.Direction.TOP:
					newMatrix[movement.point.x, movement.point.y] &= ~BOX;
					newMatrix[movement.point.x, movement.point.y - 1] |= BOX;
					newMatrix[person.x, person.y] &= ~PERSON;
					newMatrix[movement.point.x, movement.point.y] |= PERSON;
					personX = movement.point.x;
					personY = movement.point.y;
					break;
				case SokobanMovement.Direction.BOTTOM:
					newMatrix[movement.point.x, movement.point.y] &= ~BOX;
					newMatrix[movement.point.x, movement.point.y + 1] |= BOX;
					newMatrix[person.x, person.y] &= ~PERSON;
					newMatrix[movement.point.x, movement.point.y] |= PERSON;
					personX = movement.point.x;
					personY = movement.point.y;
					break;
				default:
					Debug.Assert(false);
					break;
			}
			return new SokobanState(newMatrix, personX, personY);
		}

		public SokobanState(int[,] initMatrix)
		{
			matrix = initMatrix;
			movementSet = new HashSet<SokobanMovement>();
			int xCount = matrix.GetLength(0);
			int yCount = matrix.GetLength(1);
			for(int x = 0; x < xCount; x++)
			{
				for (int y = 0; y < yCount; y++)
				{
					if ((matrix[x, y] & PERSON) == PERSON)
					{
						person = new SokobanPoint(x, y);
					}
				}
			}
			if (person != null)
			{
				CalculateMovement();
			}
		}

		protected SokobanState(int[,] initMatrix, int personX, int personY)
		{
			matrix = initMatrix;
			movementSet = new HashSet<SokobanMovement>();
			person = new SokobanPoint(personX, personY);
			CalculateMovement();
		}

		protected void CalculateMovement()
		{
			Debug.Assert(matrix != null && person != null);
			movementSet.Clear();
			HashSet<SokobanPoint> checkedSet = new HashSet<SokobanPoint>();
			Queue<SokobanPoint> checkQueue = new Queue<SokobanPoint>();
			checkedSet.Add(person);
			checkQueue.Enqueue(person);

			while (checkQueue.Count > 0)
			{
				SokobanPoint point = checkQueue.Dequeue();
				
				// Left direction
				int leftPointValue = getPointValue(point.x - 1, point.y);
				if ((leftPointValue & BOX) == BOX)
				{
					int farLeftPointValue = getPointValue(point.x - 2, point.y);
					if ((farLeftPointValue & (BOX | WALL)) == 0)
					{
						movementSet.Add(new SokobanMovement(point.x - 1, point.y, SokobanMovement.Direction.LEFT));
					}
				}
				if ((leftPointValue & (BOX | WALL)) == 0)
				{
					if (checkedSet.Add(new SokobanPoint(point.x - 1, point.y)))
						checkQueue.Enqueue(new SokobanPoint(point.x - 1, point.y));
				}

				// Right direction
				int rightPointValue = getPointValue(point.x + 1, point.y);
				if ((rightPointValue & BOX) == BOX)
				{
					int farRightPointValue = getPointValue(point.x + 2, point.y);
					if ((farRightPointValue & (BOX | WALL)) == 0)
					{
						movementSet.Add(new SokobanMovement(point.x + 1, point.y, SokobanMovement.Direction.RIGHT));
					}
				}
				if ((rightPointValue & (BOX | WALL)) == 0)
				{
					if (checkedSet.Add(new SokobanPoint(point.x + 1, point.y)))
						checkQueue.Enqueue(new SokobanPoint(point.x + 1, point.y));
				}

				// Top direction
				int topPointValue = getPointValue(point.x, point.y - 1);
				if ((topPointValue & BOX) == BOX)
				{
					int farTopPointValue = getPointValue(point.x, point.y - 2);
					if ((farTopPointValue & (BOX | WALL)) == 0)
					{
						movementSet.Add(new SokobanMovement(point.x, point.y - 1, SokobanMovement.Direction.TOP));
					}
				}
				if ((topPointValue & (BOX | WALL)) == 0)
				{
					if (checkedSet.Add(new SokobanPoint(point.x, point.y - 1)))
						checkQueue.Enqueue(new SokobanPoint(point.x, point.y - 1));
				}

				// Bottom direction
				int bottomPointValue = getPointValue(point.x, point.y + 1);
				if ((bottomPointValue & BOX) == BOX)
				{
					int farBottomPointValue = getPointValue(point.x, point.y + 2);
					if ((farBottomPointValue & (BOX | WALL)) == 0)
					{
						movementSet.Add(new SokobanMovement(point.x, point.y + 1, SokobanMovement.Direction.BOTTOM));
					}
				}
				if ((bottomPointValue & (BOX | WALL)) == 0)
				{
					if (checkedSet.Add(new SokobanPoint(point.x, point.y + 1)))
						checkQueue.Enqueue(new SokobanPoint(point.x, point.y + 1));
				}
			}
		}

		protected int getPointValue(int x, int y)
		{
			if ((x >= 0 && x < matrix.GetLength(0)) &&
				(y >= 0 && y < matrix.GetLength(1)))
			{
				return matrix[x, y];
			}
			return WALL;
		}

		protected int[,] matrix;
		protected HashSet<SokobanMovement> movementSet;
		protected SokobanPoint person;

		public class SokobanPoint
		{
			public SokobanPoint(int x, int y)
			{
				this.x = x;
				this.y = y;
			}

			public override bool Equals(object obj)
			{
				if (obj != null && obj.GetType() == typeof(SokobanPoint))
				{
					return this.x == ((SokobanPoint)obj).x && this.y == ((SokobanPoint)obj).y;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return x + y;
			}

			public override string ToString()
			{
				return "(" + x +"," + y + ")";
			}

			public static bool operator ==(SokobanPoint point1, SokobanPoint point2)
			{
				return point1.Equals(point2);
			}

			public static bool operator !=(SokobanPoint point1, SokobanPoint point2)
			{
				return !point1.Equals(point2);
			}

			public int x;
			public int y;
		}

		public class SokobanMovement
		{
			public enum Direction
			{
				LEFT,
				RIGHT,
				TOP,
				BOTTOM
			}

			public SokobanMovement(int x, int y, Direction direction)
			{
				this.point = new SokobanPoint(x, y);
				this.direction = direction;
			}

			public override bool Equals(object obj)
			{
				if (obj != null && obj.GetType() == typeof(SokobanMovement))
				{
					return this.point == ((SokobanMovement)obj).point && this.direction == ((SokobanMovement)obj).direction;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return point.GetHashCode() + direction.GetHashCode();
			}

			public override string ToString()
			{
				return point + " - " + direction;
			}

			public SokobanPoint point;
			public Direction direction;
		}
	}
}
