using Microsoft.EntityFrameworkCore;
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
using YClimb.Controls.Common;
using YClimb.Utilities;

namespace YClimb.Controls
{
    /// <summary>
    /// Логика взаимодействия для Ctrl_LogIn.xaml
    /// </summary>
    public partial class Ctrl_LogIn : UserControl
    {
        ApplicationContext db = new ApplicationContext();
        public Ctrl_LogIn()
        {
            InitializeComponent();

            uc_login.Content = new Ctrl_TextField("Nickname or Email", 32);

            uc_password.Content = new Ctrl_TextField("Password");

            Loaded += LogIn_Loaded;
        }

        public Ctrl_LogIn(string nickname, string password): this()
        {
            ((Ctrl_TextField)uc_login.Content).TB.Text = nickname;
            ((Ctrl_TextField)uc_password.Content).TB.Text = password;
        }

        private void LogIn_Loaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();
            db.Users.Load();
        }

        private bool ValidateCredentials(string login, string password)
        {
            // Ищем пользователя по nickname ИЛИ email
            var user = db.Users.FirstOrDefault(u =>
                u.Nickname == login || u.Email == login);

            if (user == null)
            {
                MessageBox.Show("User not found!");
                return false;
            }

            
            if (!PasswordHelper.VerifyPassword(password, user.Password))
            {
                MessageBox.Show("Invalid password!");
                return false;
            }

            return true;
        }

        private void ButtonSignInClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = ((Ctrl_TextField)uc_login.Content).TB.Text;
                string password = ((Ctrl_TextField)uc_password.Content).TB.Text;

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both login and password!");
                    return;
                }

                if (ValidateCredentials(login, password))
                {
                    // Получаем данные пользователя
                    var user = db.Users.FirstOrDefault(u =>
                        u.Nickname == login || u.Email == login);

                    // Используем метод LoginUser из MainWindow для сохранения сессии
                    MainWindow.Instance.LoginUser(user);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during login: {ex.Message}");
            }
        }

        private void ButtonSignUpClick(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.CurrentControl.Content = new Ctrl_SignUp();
        }
    }
}
