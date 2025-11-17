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

namespace YClimb.Controls
{
    /// <summary>
    /// Логика взаимодействия для Ctrl_Main.xaml
    /// </summary>
    public partial class Ctrl_Feed : UserControl
    {
        public Ctrl_Feed()
        {
            InitializeComponent();
            PageData.Content = new Common.Ctrl_FeedPost();
        }

        

        
        private void Button_Routes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.CurrentControl.Content = new Ctrl_Routes();
        }
        private void Button_Events_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.CurrentControl.Content = new Ctrl_Events();
        }
        private void Button_Schedule_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.CurrentControl.Content = new Ctrl_Schedule();
        }
        private void Button_Profile_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.CurrentControl.Content = new Ctrl_Profile();
        }
    }
}
