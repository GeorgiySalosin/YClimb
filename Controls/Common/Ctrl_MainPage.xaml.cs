using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YClimb.Controls.Content;
using YClimb.Entities;
using YClimb.Utilities;

namespace YClimb.Controls.Common
{
    public partial class Ctrl_MainPage : UserControl
    {
        ApplicationContext db = new ApplicationContext();
        User User { get; set; }
        private readonly UserService _userService;

        public Ctrl_MainPage(User user)
        {
            User = user;
            _userService = new UserService(new ApplicationContext());
            InitializeComponent();
            Loaded += MainPage_Loaded;

            Profile.AvatarUpdated += OnAvatarUpdated;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _userService.LoadUserAvatar(User.Id, AvatarBorder);
            Button_Feed_Click(sender, e);
        }

        private void Ctrl_MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Profile.AvatarUpdated -= OnAvatarUpdated;
        }

        private void Button_Feed_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Feed);
            PageData.Content = new Feed(User);
        }

        private void Button_Routes_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Routes);
            PageData.Content = new Ctrl_EmptyPage();
        }

        private void Button_Events_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Events);
            PageData.Content = new Ctrl_EmptyPage();
        }

        private void Button_Schedule_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Schedule);
            PageData.Content = new Schedule();
        }

        private void Button_Profile_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonTo(Button_Profile);
            PageData.Content = new Profile(User);
        }


        private void OnAvatarUpdated(object sender, EventArgs e)
        {
           _userService.LoadUserAvatar(User.Id, AvatarBorder);
           
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

        private void ButtonLogOutClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to log out?", "Log Out",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Вызываем метод выхода из MainWindow
                MainWindow.Instance.LogoutUser();
            }
        }
    }
}