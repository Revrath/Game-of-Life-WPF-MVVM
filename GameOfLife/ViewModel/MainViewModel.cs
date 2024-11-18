using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<bool> Cells { get; set; } = new ObservableCollection<bool>();
        public int SizeX { get; set; } = 100;
        public int SizeY { get; set; } = 100;
        public MainViewModel()
        {
	        for (int i = 0; i < SizeX * SizeY; i++)
	        {
		        Cells.Add(new bool());
	        }
            
        }
	}
}
