using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.Model
{
	internal class Board
	{
		public Board(int sizeX, int sizeY)
		{
			this.sizeX = sizeX;
			this.sizeY = sizeY;
			cells = new bool[sizeY, sizeX];
			for (int i = 0; i < sizeY; i++)
			{
				for (int j = 0; j < sizeX; j++)
				{
					cells[i,j] = false;
				}
			}
		}

		public void Alive(int x, int y)
		{
			cells[y,x] = true;
		}
		public void Dead(int x, int y)
		{
			cells[y,x] = false;
		}

		public int sizeX { get; }
		public int sizeY { get; }
		public bool[,] cells { get; }
	}
}
