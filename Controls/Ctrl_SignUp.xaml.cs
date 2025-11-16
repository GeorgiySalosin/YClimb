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
    /// 
    public partial class Ctrl_SignUp : UserControl
    {
        public Ctrl_SignUp()
        {
            InitializeComponent();

            uc_nickname.Content = new Ctrl_TextField("Nickname", 24);

            uc_email.Content = new Ctrl_TextField("Email", 24);

            uc_password.Content = new Ctrl_TextField("Password");

            uc_password_confirm.Content = new Ctrl_TextField("Confirm Password");
        }

        private string GetNickName()
        {
            return ((Ctrl_TextField)uc_nickname.Content).TB.Text;
        }

        private string GetPassword()
        {
            return ((Ctrl_TextField)uc_password.Content).TB.Text;
        }

        private bool ValidateInput()
        {
            try
            {
                string login = ((Ctrl_TextField)uc_nickname.Content).TB.Text;
                string password = ((Ctrl_TextField)uc_password.Content).TB.Text;
                if (login != string.Empty && password != string.Empty)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        private void ButtonConfirmSignUpClick(object sender, RoutedEventArgs e)
        {
            if (ValidateInput()) MainWindow.Instance.CurrentControl.Content = new Ctrl_LogIn(GetNickName(), GetPassword()); 
        }
    }


}
