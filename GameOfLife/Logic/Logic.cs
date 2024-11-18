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
	internal class Logic
	{
		static int sizeX;
		static int sizeY;
		private Board board = new Board(sizeX, sizeY);
		private ObservableCollection<bool> cells = new ObservableCollection<bool>();
		private int aliveCount;
		private int xCurrent;
		private int yCurrent;
		private Canvas canvas;
		private Task loop;
		private MainWindow mw;
		public Logic(ObservableCollection<bool> cells, int x, int y, Canvas canvas, MainWindow mainWindow)
		{
			this.cells = cells;
			sizeX = x;
			sizeY = y;
			this.canvas = canvas;
			mw = mainWindow;
		}

		private void Underpopulation()
		{
			if (aliveCount < 2)
			{
				// board.Dead(xCurrent, yCurrent);
				SetCell(xCurrent, yCurrent, false);
			}

		}
		private void Reproduction()
		{
			if (!IsAlive(xCurrent, yCurrent) && aliveCount == 3)
			{
				// board.Alive(xCurrent, yCurrent);
				SetCell(xCurrent, yCurrent, true);
			}
		}
		private void Overpopulation()
		{
			if (aliveCount > 3)
			{
				// board.Dead(xCurrent, yCurrent);
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

			//return x;
		}

		private bool OutOfBounds(int x, int y)
		{
			if (x < 0 || y < 0 || x >= sizeX || y >= sizeY)
				return true;
			return false;
		}

		private bool GetCell(int x, int y)
		{
			return cells[x + y * sizeX];
		}
		private void SetCell(int x, int y, bool val)
		{
			cells[x + y * sizeX] = val;
		}

		private bool IsAlive(int x, int y)
		{
			if (OutOfBounds(x, y))
				return false;
			return GetCell(x, y);
		}

		public void Initialize()
		{
			Random random = new Random(451);
			for (int i = 0; i < sizeY; i++)
			{
				for (int j = 0; j < sizeX; j++)
				{
					if (random.Next(1, 5) == 4)
						SetCell(j, i, true);
				}
			}
		}

		private void Update()
		{
			while (true)
			{
				mw.Dispatcher.Invoke(mw.UpdateUI());
				mw.UpdateUI();
				Thread.Sleep(1000);

				for (int i = 0; i < sizeY; i++)
				{
					yCurrent = i;
					for (int j = 0; j < sizeX; j++)
					{
						xCurrent = j;
						AliveNeighboursCount(j, i);
						Underpopulation();
						Overpopulation();
						Reproduction();
					}
				}
			}
		}

		public void Run()
		{
			Initialize();
			loop = new Task(Update);
			loop.Start();
		}
	}
}