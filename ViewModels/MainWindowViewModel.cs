using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Zlabs8.Views;

namespace Zlabs8.ViewModels
{
    public class MainWindowViewModel
    {
        public ICommand OpenLab2Command { get; }
        public ICommand OpenLab3Command { get; }
        public ICommand OpenLab4Command { get; }
        public ICommand OpenLab5Command { get; }
        public ICommand OpenLab6Command { get; }
        public ICommand OpenLab7Command { get; }
        public ICommand OpenLab8Command { get; }

        public MainWindowViewModel()
        {
            OpenLab2Command = new RelayCommand(OpenLab2);
            OpenLab3Command = new RelayCommand(OpenLab3);
            OpenLab4Command = new RelayCommand(OpenLab4);
            OpenLab5Command = new RelayCommand(OpenLab5);
            /*OpenLab6Command = new RelayCommand(OpenLab6);
            OpenLab7Command = new RelayCommand(OpenLab7);
            OpenLab8Command = new RelayCommand(OpenLab8);*/
        }

        private void OpenLab2() => OpenWindow(new Lab2View());
        private void OpenLab3() => OpenWindow(new Lab3View());
        private void OpenLab4() => OpenWindow(new Lab4View());
        private void OpenLab5() => OpenWindow(new Lab5View());
        /*private void OpenLab6() => OpenWindow(new Lab6View());
        private void OpenLab7() => OpenWindow(new Lab7View());
        private void OpenLab8() => OpenWindow(new Lab8View());*/

        private void OpenWindow(Window labWindow)
        {
            labWindow.Show();
        }
    }
}