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
using YClimb.Controls.Content;

namespace YClimb.Controls.Common
{
    /// <summary>
    /// Логика взаимодействия для Ctrl_MainPage.xaml
    /// </summary>
    public partial class Ctrl_MainPage : UserControl
    {
        public Ctrl_MainPage()
        {
            InitializeComponent();
        }

        private void Button_Feed_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Feed);
            PageData.Content = new Ctrl_EmptyPage();
        }

        private void Button_Routes_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Routes);
            PageData.Content = new Routes();
        }

        private void Button_Events_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Events);
            PageData.Content = new Events();
        }

        private void Button_Schedule_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Schedule);
            PageData.Content = new Schedule();
        }

        private void Button_Profile_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Profile);
            PageData.Content = new Profile();
        }


        private void ChangeButtonTo(Button button)
        {
            Button_Feed.IsEnabled = true;
            Button_Routes.IsEnabled = true;
            Button_Events.IsEnabled = true;
            Button_Schedule.IsEnabled = true;
            Button_Profile.IsEnabled = true;

            button.IsEnabled = false;
        }

    }
}
