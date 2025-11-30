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
using YClimb.Entities;
using YClimb.Utilities;

namespace YClimb.Controls.Content
{
    /// <summary>
    /// Логика взаимодействия для Feed.xaml
    /// </summary>
    public partial class Feed : UserControl
    {
        private readonly User _currentUser;
        private readonly PostService _postService;
        private readonly ApplicationContext _dbContext;

        public Feed(User user)
        {
            _currentUser = user;
            _dbContext = new ApplicationContext();
            _postService = new PostService(_dbContext);

            InitializeComponent();
            Loaded += Feed_Loaded;
        }

        private async void Feed_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPosts();
        }
        public async Task RefreshPosts()
        {
            await LoadPosts();
        }


        private void ShowCreatePostButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCreatePostForm();
        }

        private void ShowCreatePostForm()
        {
            var createPostControl = new Ctrl_CreatePost(_currentUser);
            createPostControl.PostCreated += OnPostCreated;
            createPostControl.PostCancelled += OnPostCancelled;

            CreatePostContainer.Content = createPostControl;
            ShowCreatePostButton.Visibility = Visibility.Collapsed;
        }

        private void HideCreatePostForm()
        {
            CreatePostContainer.Content = null;
            ShowCreatePostButton.Visibility = Visibility.Visible;
        }

        private async void OnPostCreated(object? sender, PostCreatedEventArgs e)
        {
            HideCreatePostForm();

            // update feed
            await LoadPosts();

            ScrollToTop();
        }

        private void OnPostCancelled(object? sender, EventArgs e)
        {
            HideCreatePostForm();
        }

        private async Task LoadPosts()
        {
            try
            {
                var posts = await _postService.GetPostsWithUsersAsync();

                // Очищаем существующие посты
                PostsStackPanel.Children.Clear();

                // Добавляем посты в ленту
                foreach (var post in posts)
                {
                    var postControl = new Ctrl_FeedPost
                    {
                        Post = post,
                        Margin = new Thickness(0, 0, 0, 20)
                    };
                    PostsStackPanel.Children.Add(postControl);
                }

                // Если постов нет, показываем сообщение
                if (!posts.Any())
                {
                    var noPostsText = new TextBlock
                    {
                        Text = "No posts yet. Be the first to post!",
                        FontSize = 24,
                        FontFamily = new FontFamily("Bahnschrift"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 50, 0, 0)
                    };
                    PostsStackPanel.Children.Add(noPostsText);
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                var errorText = new TextBlock
                {
                    Text = $"Error loading posts: {ex.Message}",
                    FontSize = 18,
                    Foreground = Brushes.Red,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                PostsStackPanel.Children.Add(errorText);
            }
        }


        private void ScrollToTop()
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(this);
            scrollViewer?.ScrollToTop();
        }

        private static T? FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T found)
                    return found;

                T? childItem = FindVisualChild<T>(child);
                if (childItem != null) return childItem;
            }
            return null;
        }

        
    }
}
