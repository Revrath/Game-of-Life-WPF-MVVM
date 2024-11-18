using System.Collections.ObjectModel;
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
		private bool[,] cells;
		private Logic.Logic logic;
		public MainWindow()
		{
			InitializeComponent();
			DataContext = _viewModel;
			sizeX = _viewModel.SizeX;
			sizeY = _viewModel.SizeY;
			cells = _viewModel.Cells;
			logic = new Logic.Logic(cells, sizeX, sizeY, MyCanvas, this);
			logic.Run();
		}


		public Action UpdateUI()
		{
			return () =>
			{
				MyCanvas.Children.Clear();
				double cellSize = MyCanvas.Height / sizeX;
				for (int i = 0; i < sizeY; i++)
				{
					for (int j = 0; j < sizeX; j++)
					{
						if (cells[i ,j])
						{
							var cell = new Rectangle
							{
								Width = cellSize,
								Height = cellSize,
								Fill = Brushes.MidnightBlue
							};			
							Canvas.SetTop(cell, cellSize * i);
							Canvas.SetLeft(cell, cellSize * j);
							MyCanvas.Children.Add(cell);
						}
					}
				}
			};
		}

		private void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{

		}

		private void buttonInit_Click(object sender, RoutedEventArgs e)
		{
			logic.Initialize();
		}

		private void emptyState()
		{
			for (int i = 0; i < sizeY; i++)
			{
				for (int j = 0; j < sizeX; j++)
				{
					cells[j, i] = false;
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
			cells = new bool [sizeX, sizeY];
			emptyState();
			logic = new Logic.Logic(cells, sizeX, sizeY, MyCanvas, this);
		}
	}
}