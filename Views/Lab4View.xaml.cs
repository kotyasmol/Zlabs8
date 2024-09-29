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
using System.Windows.Shapes;
using Zlabs8.ViewModels;

namespace Zlabs8.Views
{
    /// <summary>
    /// Логика взаимодействия для Lab4View.xaml
    /// </summary>
    public partial class Lab4View : Window
    {
        public Lab4View()
        {
            InitializeComponent();
            DataContext = new Lab4ViewModel();
        }
    }
}
