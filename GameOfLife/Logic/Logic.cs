using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GameOfLife.Model;
using Microsoft.UI.Xaml.Controls;
using Canvas = System.Windows.Controls.Canvas;

namespace GameOfLife.Logic
{
	internal class Logic : IDisposable
	{
		static int sizeX;
		static int sizeY;
		private Board board = new Board(sizeX, sizeY);
		private volatile bool[,] cells;
		private volatile bool[,] copy;
		private int aliveCount;
		private int xCurrent;
		private int yCurrent;
		private Canvas canvas;
		public Task loop;
		private MainWindow mw;
		public CancellationTokenSource cancelToken = new CancellationTokenSource();
		public int delay = 1000;
		public bool isLoop = true;
		public int turn = 0;
		public int totalAliveCount = 0;
		public int newborns = 0;
		public Shape shape;
		public Logic(bool[,] cells, int x, int y, Canvas canvas, MainWindow mainWindow)
		{
			this.cells = cells;
			sizeX = x;
			sizeY = y;
			this.canvas = canvas;
			mw = mainWindow;
			copy = new bool[sizeY, sizeX];
		}

		private void Underpopulation()
		{
			if (aliveCount < 2)
			{
				SetCell(xCurrent, yCurrent, false);
			}

		}
		private void Reproduction()
		{
			if (aliveCount == 3)
			{
				SetCell(xCurrent, yCurrent, true);
				newborns++;
			}
		}
		private void Overpopulation()
		{
			if (aliveCount > 3)
			{
				SetCell(xCurrent, yCurrent, false);
			}
		}

		private void AliveNeighboursCount(int x, int y)
		{
			aliveCount = 0;
			int count = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if ((i !=0 || j != 0) && IsAlive(x + i, y + j))
						count++;
				}
			}
			aliveCount = count;
		}

		private bool OutOfBounds(int x, int y)
		{
			if (x < 0 || y < 0 || x >= sizeX || y >= sizeY)
				return true;
			return false;
		}

		private bool GetCell(int x, int y)
		{
			// return cells[x + y * sizeX];
			return copy[y, x];
		}
		public void SetCell(int x, int y, bool val)
		{
			cells[y, x] = val;
		}

		private bool IsAlive(int x, int y)
		{
			if (OutOfBounds(x, y))
				return false;
			return GetCell(x, y);
		}

		public void Initialize()
		{
			Random random = new Random();
			for (int i = 0; i < sizeY; i++)
			{
				for (int j = 0; j < sizeX; j++)
				{
					if (random.Next(1, 5) == 4)
						SetCell(j, i, true);
				}
			}
			loop = new Task(Update);
			loop.Start();
		}

		public void Glider()
		{
			SetCell(3,1,true);
			SetCell(1,2,true);
			SetCell(3,2,true);
			SetCell(2,3,true);
			SetCell(3,3,true);
			loop = new Task(Update);
			loop.Start();
		}

		public void Frog()
		{
			SetCell(1,0,true);
			SetCell(1,1,true);
			SetCell(1,2,true);
			SetCell(2,1,true);
			SetCell(2,2,true);
			SetCell(2,3,true);
			loop = new Task(Update);
			loop.Start();
		}

		public void Update()
		{
			do
			{
				if (cancelToken.IsCancellationRequested)
				{
					return;
				}

				for (int i = 0; i < sizeY; i++)
				{
					for (int j = 0; j < sizeX; j++)
					{
						copy[i, j] = cells[i, j];
					}
				}

				for (int i = 0; i < sizeY; i++)
				{
					yCurrent = i;
					for (int j = 0; j < sizeX; j++)
					{
						copy[i,j] = cells[i,j];
						xCurrent = j;
						AliveNeighboursCount(j, i);
						Underpopulation();
						Overpopulation();
						Reproduction();
						if(IsAlive(j,i))
							totalAliveCount++;
					}
				}
				mw.Dispatcher.Invoke(mw.UpdateUI());
				Thread.Sleep(delay);

				newborns = 0;
				totalAliveCount = 0;
				turn++;
			} while (isLoop);
		}
		
		public void Dispose()
		{
			loop.Dispose();
		}
	}
}