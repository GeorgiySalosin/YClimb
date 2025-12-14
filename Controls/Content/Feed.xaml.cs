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

            ShowCreatePostButton.Visibility = _currentUser.IsAdmin == true
                ? Visibility.Visible
                : Visibility.Collapsed;
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
            ShowCreatePostButton.Visibility = _currentUser.IsAdmin == true
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private async void OnPostCreated(object? sender, PostCreatedEventArgs e)
        {
            HideCreatePostForm();
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
                PostsStackPanel.Children.Clear();

                foreach (var post in posts)
                {
                    var postControl = new Ctrl_FeedPost
                    {
                        Post = post,
                        Margin = new Thickness(0, 0, 0, 20)
                    };

                    var viewModel = new PostViewModel(post, _currentUser);
                    viewModel.OnPostDeleted += async (s, e) => await RefreshPosts();
                    postControl.DataContext = viewModel;

                    PostsStackPanel.Children.Add(postControl);
                }

                


                if (!posts.Any())
                {
                    Image noPostsImage = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Images/DefaultPage.png")),
                        Width = 512,
                        Height = 512,
                        Stretch = Stretch.Uniform
                    };

                    string messageText = _currentUser.IsAdmin == true
                        ? "No posts yet. Be the first to create one!"
                        : "No posts yet. Administration did not place any!";

                    TextBlock noPostsText = new TextBlock
                    {
                        Text = messageText,
                        FontSize = 36,
                        FontWeight = FontWeights.DemiBold,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888")),
                        FontFamily = new FontFamily("Bahnschrift"),
                        Opacity = 0.8,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(48, -48, 0, 0)
                    };

                    PostsStackPanel.Children.Add(noPostsImage);
                    PostsStackPanel.Children.Add(noPostsText);
                }
            }
            catch (Exception ex)
            {
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