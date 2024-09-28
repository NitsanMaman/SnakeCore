using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //initialize all the XAML
            InitializeComponent();

            // Set the DataContext for connecting the View (MainWindow) to the ViewModel (SnakeViewModel)
            this.DataContext = new SnakeViewModel();
        }
    }
}

