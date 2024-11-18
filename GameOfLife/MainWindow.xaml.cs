using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameOfLife.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Canvas = System.Windows.Controls.Canvas;

namespace GameOfLife
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MainViewModel _viewModel = new MainViewModel();
		private int sizeX;
		private int sizeY;
		private volatile bool[,] cells;
		private Logic.Logic logic;
		public int scale;
		private bool isCircle = false;
		private Brush color = new SolidColorBrush(Colors.Black);
		private int colorIndex = 0;
		public MainWindow()
		{
			InitializeComponent();
			DataContext = _viewModel;
			sizeX = Convert.ToInt32(TextSizeX.Text);
			sizeY = Convert.ToInt32(TextSizeY.Text);
			cells = _viewModel.Cells;
			scale = Convert.ToInt32(TextZoom.Text);
			logic = new Logic.Logic(cells, sizeX, sizeY, MyCanvas, this);
			logic.Initialize();
		}

		private void UpdateStats()
		{
			TextStats.Text += $"turn {logic.turn}, {logic.newborns} new cells, {logic.totalAliveCount} alive\n";
		}
		
		public Action UpdateUI()
		{
			return () =>
			{
				UpdateStats();
				MyCanvas.Children.Clear();
				double cellSizeHeight = MyCanvas.Height / sizeY * scale;
				double cellSizeWidth = MyCanvas.Width / sizeX * scale;
				color.Freeze();
				for (int i = 0; i < sizeY / scale; i++)
				{
					for (int j = 0; j < sizeX / scale; j++)
					{
						if (cells[i, j])
						{
							Shape cell;
							if (isCircle)
							{
								cell = new Ellipse
								{
									Width = cellSizeWidth,
									Height = cellSizeHeight,
									Fill = color
								};
							}
							else
							{
								cell = new Rectangle
								{
									Width = cellSizeWidth,
									Height = cellSizeHeight,
									Fill = color
								};
							}

							Canvas.SetTop(cell, cellSizeHeight * i);
							Canvas.SetLeft(cell, cellSizeWidth * j);
							MyCanvas.Children.Add(cell);
						}
					}
				}
			};
		}

		private void buttonInit_Click(object sender, RoutedEventArgs e)
		{
			buttonNewSize_Click(sender, e);
		}

		private void emptyState()
		{
			logic.cancelToken.Cancel();
			for (int i = 0; i < sizeY; i++)
			{
				for (int j = 0; j < sizeX; j++)
				{
					cells[i, j] = false;
				}
			}
			MyCanvas.Children.Clear();
		}
		private void buttonClear_Click_1(object sender, RoutedEventArgs e)
		{
			emptyState();
		}

		private void buttonNewSize_Click(object sender, RoutedEventArgs e)
		{
			sizeX = Convert.ToInt32(TextSizeX.Text);
			sizeY = Convert.ToInt32(TextSizeY.Text);
			cells = new bool [sizeY, sizeX];
			emptyState();
			
			// logic.Dispose();
			logic = new Logic.Logic(cells, sizeX, sizeY, MyCanvas, this);
			logic.Initialize();
		}

		private void ButtonLoop_Click(object sender, RoutedEventArgs e)
		{
			logic.delay = Convert.ToInt32(TextDelay.Text);
			if (logic.isLoop == false)
			{
				logic.isLoop = true;
				logic.loop = new Task(logic.Update);
				logic.loop.Start();	
			}
		}

		private void ButtonStep_Click(object sender, RoutedEventArgs e)
		{
			logic.isLoop = false;
			logic.delay = 0;
			logic.Update();
		}
		
		private void ButtonZoom_Click(object sender, RoutedEventArgs e)
		{
			scale = Convert.ToInt32(TextZoom.Text);
		}

		private void ButtonMode_Click(object sender, RoutedEventArgs e)
		{
			isCircle = !isCircle;
		}

		private void ButtonColor_Click(object sender, RoutedEventArgs e)
		{
			colorIndex++;
			if (colorIndex > 2)
				colorIndex = 0;
			switch (colorIndex)
			{
				case 0:
					color = new SolidColorBrush(Colors.Black);
					break;
				case 1:
					color = new SolidColorBrush(Colors.Navy);
					break;
				case 2:
					color = new SolidColorBrush(Colors.SaddleBrown);
					break;
			}
		}

		private void ButtonExport_Click(object sender, RoutedEventArgs e)
		{
			TextWriter writer = null;
			try
			{
				var jsonCells = JsonConvert.SerializeObject(cells);
				writer = new StreamWriter(@".\write.txt", false);
				writer.Write(jsonCells);
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
		}

		private void ButtonImport_Click(object sender, RoutedEventArgs e)
		{
			TextReader reader = null;
			try
			{
				reader = new StreamReader(@".\write.txt");
				var fileContents = reader.ReadToEnd();
				emptyState();

				cells = JsonConvert.DeserializeObject<bool[,]>(fileContents);
				sizeX = cells.GetLength(1);
				sizeY = cells.GetLength(0);
			
				logic = new Logic.Logic(cells, sizeX, sizeY, MyCanvas, this);
				logic.loop = new Task(logic.Update);
				logic.loop.Start();
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
		}

		private void AddCellWithMouse(object sender, MouseButtonEventArgs e)
		{
			var point = e.GetPosition(MyCanvas);
			
			double cellSizeHeight = MyCanvas.Height / sizeY * scale;
			double cellSizeWidth = MyCanvas.Width / sizeX * scale;
			int x = (int)(point.X/cellSizeWidth);
			int y = (int)(point.Y/cellSizeHeight);
			logic.SetCell(x,y, true);
			Dispatcher.Invoke(UpdateUI());
		}	

		private void ButtonFrog_Click(object sender, RoutedEventArgs e)
		{
			emptyState();
			logic = new Logic.Logic(cells, sizeX, sizeY, MyCanvas, this);
			logic.Frog();
		}

		private void ButtonGlider_Click(object sender, RoutedEventArgs e)
		{
			emptyState();
			logic = new Logic.Logic(cells, sizeX, sizeY, MyCanvas, this);
			logic.Glider();
		}
	}
}