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
    /// Логика взаимодействия для Ctrl_LogIn.xaml
    /// </summary>
    public partial class Ctrl_LogIn : UserControl
    {
        public Ctrl_LogIn()
        {
            InitializeComponent();

            uc_login.Content = new Ctrl_TextField("Nickname or Email", 24);

            uc_password.Content = new Ctrl_TextField("Password");
        }

        private void ButtonSignInClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = ((Ctrl_TextField)uc_login.Content).TB.Text;
                string password = ((Ctrl_TextField)uc_password.Content).TB.Text;
                if (login != string.Empty && password != string.Empty)
                {
                    MainWindow.Instance.CurrentControl.Content = new Ctrl_Feed();
                }
            }
            catch (Exception)
            {

            }
            
        }

        private void ButtonSignUpClick(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.CurrentControl.Content = new Ctrl_SignUp();
        }
    }
}
