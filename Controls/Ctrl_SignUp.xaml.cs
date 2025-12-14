using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using YClimb.Entities;
using YClimb.Utilities;

namespace YClimb.Controls
{
    /// <summary>
    /// Логика взаимодействия для Ctrl_nickname.xaml
    /// </summary>
    /// 
    public partial class Ctrl_SignUp : UserControl
    {
        ApplicationContext db = new ApplicationContext();
        public Ctrl_SignUp()
        {
            InitializeComponent();
            Loaded += SignUp_Loaded;

            uc_nickname.Content = new Ctrl_TextField("Nickname", 32);
            uc_email.Content = new Ctrl_TextField("Email", 32);
            uc_password.Content = new Ctrl_TextField("Password");
            uc_password_confirm.Content = new Ctrl_TextField("Confirm Password");
        }

        private void SignUp_Loaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();
            db.Users.Load();
            DataContext = db.Users.Local.ToObservableCollection();
        }

        private string GetNickname()
        {
            return ((Ctrl_TextField)uc_nickname.Content).TB.Text;
        }
        private string GetEmail()
        {
            return ((Ctrl_TextField)uc_email.Content).TB.Text;
        }
        private string GetPassword()
        {
            return ((Ctrl_TextField)uc_password.Content).TB.Text;
        }

        private bool ValidateInput()
        {
            try
            {
                string nickname = ((Ctrl_TextField)uc_nickname.Content).TB.Text;
                string email = ((Ctrl_TextField)uc_email.Content).TB.Text;
                string password = ((Ctrl_TextField)uc_password.Content).TB.Text;
                string confirmPassword = ((Ctrl_TextField)uc_password_confirm.Content).TB.Text;


                if (password == string.Empty && email == string.Empty)
                {
                    return false;
                }


                if (password.Length < 8)
                {
                    MessageBox.Show("Password must be longer than 8 characters!");
                    return false;
                }

                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Invalid email!");
                    return false;
                }


                if (password != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match!");
                    return false;
                }


                if (db.Users.Any(u => u.Nickname == nickname))
                {
                    MessageBox.Show("User with this nickname already exists!");
                    return false;
                }


                if (db.Users.Any(u => u.Email == email))
                {
                    MessageBox.Show("User with this email already exists!");
                    return false;
                }



                return true;
            }
            catch { }
            return false;
        }

        private void ButtonConfirmSignUpClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string nickname = GetNickname();
            string email = GetEmail();
            string password = GetPassword();


            // Creating a user with hashed password version

            string hashedPassword = PasswordHelper.HashPassword(password);
            User user = new(nickname, email, hashedPassword)
            {
                Avatar = null,
                IsAdmin = false
            };

            db.Users.Add(user);
            db.SaveChanges();

            MainWindow.Instance.CurrentControl.Content = new Ctrl_LogIn(GetNickname(), GetPassword()); 
            
        }

        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.CurrentControl.Content = new Ctrl_LogIn();
        }
    }


}
