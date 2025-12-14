using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YClimb.Entities;

namespace YClimb.Utilities
{
    public class UserService
    {
        private readonly AvatarService _avatarService;

        public UserService(ApplicationContext context)
        {
            _avatarService = new AvatarService();
        }



        private BitmapImage GetUserAvatar(int userId)
        {
            using (var context = new ApplicationContext())
            {
                var user = context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Id == userId);

                return _avatarService.ConvertByteArrayToBitmapImage(user.Avatar);
            }
        }

        private string GetUserNickname(int userId)
        {
            using (var context = new ApplicationContext())
            {
                var user = context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Id == userId);

                return user.Nickname;
            }
        }

        private string GetUserEmail(int userId)
        {
            using (var context = new ApplicationContext())
            {
                var user = context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Id == userId);

                return user.Email;
            }
        }





        public void LoadUserAvatar(int userId, Border border, Button buttonRemoveAvatar = null)
        {
            var avatar = GetUserAvatar(userId);

            buttonRemoveAvatar?.Visibility = System.Windows.Visibility.Visible;
            if (avatar == null)
            {
               avatar = new AvatarService().GetDefaultAvatarImage();
               buttonRemoveAvatar?.Visibility = System.Windows.Visibility.Hidden;
            }




            var imageBrush = new ImageBrush
            {
                ImageSource = avatar,
                Stretch = Stretch.UniformToFill
            };
            border.Background = imageBrush;

        }

        public void LoadUserNickname(int userId, TextBlock textBlock)
        {
            string nickname = GetUserNickname(userId);
            textBlock.Text = nickname;
        }

        public void LoadUserEmail(int userId, TextBlock textBlock)
        {
            string email = GetUserEmail(userId);
            textBlock.Text = email;
        }





        public void UpdateUserAvatar(int userId, string imageFilePath, Border border = null)
        {
            using (var context = new ApplicationContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    var processedAvatar = _avatarService.ProcessAndResizeImage(imageFilePath);
                    user.Avatar = processedAvatar;
                    context.SaveChanges();
                }
                if (border != null)
                {
                    LoadUserAvatar(userId, border);
                }
            }
        }

        public void UpdateUserNickname(int userId, string newNickname, TextBlock textBlock = null)
        {
            using (var context = new ApplicationContext())
            {
                if (context.Users.Any(u => u.Nickname == newNickname))
                {
                    MessageBox.Show("User with this nickname already exists!");
                    return;
                }

                var user = context.Users.FirstOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    
                    user.Nickname = newNickname;
                    context.SaveChanges();
                }
                if (textBlock != null)
                {
                    LoadUserNickname(userId, textBlock);
                }
            }
        }

        public void UpdateUserEmail(int userId, string newEmail, TextBlock textBlock = null)
        {
            using (var context = new ApplicationContext())
            {
                if (context.Users.Any(u => u.Email == newEmail))
                {
                    MessageBox.Show("User with this email already exists!");

                    return;
                }

                if (!Regex.IsMatch(newEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Invalid email!");
                    return;
                }

                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.Email = newEmail;
                    context.SaveChanges();
                }
                if (textBlock != null)
                {
                    LoadUserEmail(userId, textBlock);
                }
            }
        }



        public void RemoveUserAvatar(int userId, Border border = null)
        {
            using (var context = new ApplicationContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.Avatar = null;
                    context.SaveChanges();
                }
                if (border != null)
                {
                    LoadUserAvatar(userId, border);
                }
            }
        }

    }
}