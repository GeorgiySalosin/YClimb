using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YClimb.Controls;
using YClimb.Controls.Common;
using YClimb.Entities;
using YClimb.Utilities;

namespace YClimb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public UserControl CurrentControl
        {
            get => CurrentCtrl;
        }


        
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            
            if (TryRestoreUserSession())
            {
                return;
            }

            ShowLoginPage();
        }

        
        private bool TryRestoreUserSession()
        {
            try
            {
                var currentUserId = AppSettings.CurrentUserId;
                if (currentUserId.HasValue)
                {
                    using var db = new ApplicationContext();
                    var user = db.Users.Find(currentUserId.Value);

                    if (user != null)
                    {
                        // success
                        ShowMainPage(user);
                        return true;
                    }
                    else
                    {
                        // fail
                        AppSettings.ClearUserSession();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring session: {ex.Message}");
                AppSettings.ClearUserSession();
            }

            return false;
        }

        public void ShowLoginPage()
        {
            CurrentCtrl.Content = new Ctrl_LogIn();
        }

        public void ShowMainPage(User user)
        {
            CurrentCtrl.Content = new Ctrl_MainPage(user);
        }


        public void LoginUser(User user)
        {
            // saving user id to allow him instantly log in nt
            AppSettings.CurrentUserId = user.Id;
            ShowMainPage(user);
        }

        public void LogoutUser()
        {
            AppSettings.ClearUserSession();
            ShowLoginPage();
        }
    }
}