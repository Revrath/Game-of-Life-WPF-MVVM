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
        public bool[,] Cells { get; set; }
        public int SizeX { get; set; } = 100;
        public int SizeY { get; set; } = 100;
        public MainViewModel()
        {
	        Cells = new bool[SizeY, SizeX]; 

        }
	}
}
