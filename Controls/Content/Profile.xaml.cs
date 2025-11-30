using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YClimb.Controls.Common;
using YClimb.Entities;
using YClimb.Utilities;

namespace YClimb.Controls.Content
{
    public partial class Profile : UserControl
    {
        private readonly UserService _userService;
        private readonly User _currentUser;

        
        public static event EventHandler AvatarUpdated;

        public Profile(User user)
        {
            _currentUser = user;
            _userService = new UserService(new ApplicationContext());

            InitializeComponent();
            DataContext = this;

            var nicknameControl = new Ctrl_TextFieldProfile(32);
            nicknameControl.TextSaved += OnNicknameSaved;
            uc_nickname.Content = nicknameControl;

            var emailControl = new Ctrl_TextFieldProfile(32);
            emailControl.TextSaved += OnEmailSaved;
            uc_email.Content = emailControl;

            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            _userService.LoadUserAvatar(_currentUser.Id, AvatarBorder, ButtonRemoveAvatar);
            _userService.LoadUserNickname(_currentUser.Id, ((Ctrl_TextFieldProfile)uc_nickname.Content).TextBlock);
            _userService.LoadUserEmail(_currentUser.Id, ((Ctrl_TextFieldProfile)uc_email.Content).TextBlock);

        }



        private void ChangePFP(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files (*.*)|*.*";
            openFileDialog.Title = "Select an Image";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _userService.UpdateUserAvatar(_currentUser.Id, openFileDialog.FileName, AvatarBorder);
                    ButtonRemoveAvatar.Visibility = Visibility.Visible;

                    // Вызываем статическое событие
                    AvatarUpdated?.Invoke(this, EventArgs.Empty);

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке аватара: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemovePFP(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Удалить аватар?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _userService.RemoveUserAvatar(_currentUser.Id, AvatarBorder);
                ButtonRemoveAvatar.Visibility = Visibility.Hidden;

                // Вызываем статическое событие
                AvatarUpdated?.Invoke(this, EventArgs.Empty);
            }
        }


        private void OnNicknameSaved(object sender, string newNickname)
        {
            try
            {
                _userService.UpdateUserNickname(_currentUser.Id, newNickname);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating nickname: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnEmailSaved(object sender, string newEmail)
        {
            try
            {
                _userService.UpdateUserEmail(_currentUser.Id, newEmail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating email: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}