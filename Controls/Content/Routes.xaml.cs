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
    public partial class Routes : UserControl
    {
        private readonly User _currentUser;
        private readonly PostService _postService;
        private readonly ApplicationContext _dbContext;

        public Routes(User user)
        {
            _currentUser = user;
            _dbContext = new ApplicationContext();
            _postService = new PostService(_dbContext);

            InitializeComponent();
            Loaded += Routes_Loaded;
        }

        private async void Routes_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRoutes();
        }

        public async Task RefreshRoutes()
        {
            await LoadRoutes();
        }

        private void ShowCreateRouteButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCreateRouteForm();
        }

        private void ShowCreateRouteForm()
        {
            var createRouteControl = new Ctrl_CreateRoute(_currentUser);
            createRouteControl.RouteCreated += OnRouteCreated;
            createRouteControl.RouteCancelled += OnRouteCancelled;

            CreateRouteContainer.Content = createRouteControl;
            ShowCreateRouteButton.Visibility = Visibility.Collapsed;
        }

        private void HideCreateRouteForm()
        {
            CreateRouteContainer.Content = null;
            ShowCreateRouteButton.Visibility = Visibility.Visible;
        }

        private async void OnRouteCreated(object? sender, RouteCreatedEventArgs e)
        {
            HideCreateRouteForm();
            await LoadRoutes();
            ScrollToTop();
        }

        private void OnRouteCancelled(object? sender, EventArgs e)
        {
            HideCreateRouteForm();
        }

        private async Task LoadRoutes()
        {
            try
            {
                var routes = await _postService.GetRoutesWithUsersAsync();
                RoutesStackPanel.Children.Clear();

                foreach (var route in routes)
                {
                    var routeControl = new Ctrl_RoutePost
                    {
                        Route = route,
                        Margin = new Thickness(0, 0, 0, 20)
                    };

                    var viewModel = new RouteViewModel(route, _currentUser);
                    viewModel.OnRouteDeleted += async (s, e) => await RefreshRoutes();
                    routeControl.DataContext = viewModel;

                    RoutesStackPanel.Children.Add(routeControl);
                }

                if (!routes.Any())
                {
                    Image noRoutesImage = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Images/DefaultPage.png")),
                        Width = 512,
                        Height = 512,
                        Stretch = Stretch.Uniform
                    };

                    TextBlock noRoutesText = new TextBlock
                    {
                        Text = "No routes yet. Be the first to create one!",
                        FontSize = 36,
                        FontWeight = FontWeights.DemiBold,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888")),
                        FontFamily = new FontFamily("�c�e������W5"),
                        Opacity = 0.8,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(48, -48, 0, 0)
                    };

                    RoutesStackPanel.Children.Add(noRoutesImage);
                    RoutesStackPanel.Children.Add(noRoutesText);
                }
            }
            catch (Exception ex)
            {
                var errorText = new TextBlock
                {
                    Text = $"Error loading routes: {ex.Message}",
                    FontSize = 18,
                    Foreground = Brushes.Red,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                RoutesStackPanel.Children.Add(errorText);
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