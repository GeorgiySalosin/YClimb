using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using YClimb.Entities;
using YClimb.Utilities;

namespace YClimb.Controls.Common
{
    public partial class Ctrl_FeedPost : UserControl
    {
        public static readonly DependencyProperty PostProperty =
            DependencyProperty.Register("Post", typeof(Post), typeof(Ctrl_FeedPost),
                new PropertyMetadata(null, OnPostChanged));

        public Post Post
        {
            get => (Post)GetValue(PostProperty);
            set => SetValue(PostProperty, value);
        }

        public Ctrl_FeedPost()
        {
            InitializeComponent();
            DataContext = this;
        }

        private static void OnPostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Ctrl_FeedPost)d;
            if (e.NewValue is Post newPost)
            {
                control.UpdatePostData(newPost);
            }
        }

        private void UpdatePostData(Post post)
        {
            // Обновляем привязки
            var postViewModel = new PostViewModel(post);
            DataContext = postViewModel;
        }
    }

    // ViewModel для поста
    public class PostViewModel : INotifyPropertyChanged
    {
        private readonly Post _post;
        private readonly PostService _postService;
        private readonly User _currentUser;
        private readonly bool _isCurrentUserAdmin;

        public string UserNickname => _post.User?.Nickname ?? "Unknown";
        public string Title => _post.Title;
        public string Content => _post.Content;
        public DateTime CreatedAt => _post.CreatedAt;

        // Свойства для видимости кнопки удаления
        public Visibility DeleteButtonVisibility { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public List<ImageViewModel> Images { get; private set; } = new List<ImageViewModel>();

        public PostViewModel(Post post, User currentUser = null)
        {
            _post = post;
            _currentUser = currentUser;
            _isCurrentUserAdmin = currentUser?.IsAdmin ?? false;
            _postService = new PostService(new ApplicationContext());

            // Проверяем, может ли текущий пользователь удалить этот пост
            DeleteButtonVisibility = CanDeletePost() ? Visibility.Visible : Visibility.Collapsed;

            // Команда удаления
            DeleteCommand = new RelayCommand(async () => await DeletePostAsync());

            LoadImages();
        }

        private bool CanDeletePost()
        {
            if (_currentUser == null)
                return false;

            // Автор или админ
            return _post.UserId == _currentUser.Id || _isCurrentUserAdmin;
        }

        private async Task DeletePostAsync()
        {
            if (_currentUser == null)
                return;

            var result = MessageBox.Show(
                "Are you sure you want to delete this post?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = await _postService.DeletePostAsync(
                        _post.Id,
                        _currentUser.Id,
                        _isCurrentUserAdmin
                    );

                    if (success)
                    {
                        MessageBox.Show("Post deleted successfully!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Вызываем событие удаления
                        OnPostDeleted?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        MessageBox.Show("You don't have permission to delete this post.",
                            "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting post: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event EventHandler OnPostDeleted;

        private void LoadImages()
        {
            foreach (var postImage in _post.Images.OrderBy(pi => pi.Order))
            {
                var imageViewModel = new ImageViewModel
                {
                    ImageSource = _postService.ConvertByteArrayToBitmapImage(postImage.ImageData)
                };
                Images.Add(imageViewModel);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ImageViewModel
    {
        public BitmapImage ImageSource { get; set; } = new BitmapImage();
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
