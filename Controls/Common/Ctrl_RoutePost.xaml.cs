using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YClimb.Entities;
using YClimb.Utilities;

namespace YClimb.Controls.Common
{
    public partial class Ctrl_RoutePost : UserControl
    {
        public static readonly DependencyProperty RouteProperty =
            DependencyProperty.Register("Route", typeof(Route), typeof(Ctrl_RoutePost),
                new PropertyMetadata(null, OnRouteChanged));

        public Route Route
        {
            get => (Route)GetValue(RouteProperty);
            set => SetValue(RouteProperty, value);
        }

        public Ctrl_RoutePost()
        {
            InitializeComponent();
            DataContext = this;
        }

        private static void OnRouteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Ctrl_RoutePost)d;
            if (e.NewValue is Route newRoute)
            {
                control.UpdateRouteData(newRoute);
            }
        }

        private void UpdateRouteData(Route route)
        {
            var routeViewModel = new RouteViewModel(route);
            DataContext = routeViewModel;
        }
    }


    public class RouteViewModel : INotifyPropertyChanged
    {
        private readonly Route _route;
        private readonly PostService _postService;
        private readonly User _currentUser;
        private readonly bool _isCurrentUserAdmin;
        private BitmapImage _routeImage;

        public string UserNickname => _route.User?.Nickname ?? "Unknown";
        public string Title => _route.Title;
        public DateTime CreatedAt => _route.CreatedAt;



        public Visibility DeleteButtonVisibility { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public BitmapImage RouteImage
        {
            get => _routeImage;
            set
            {
                _routeImage = value;
                OnPropertyChanged();
            }
        }

        public RouteViewModel(Route route, User currentUser = null)
        {
            _route = route;
            _currentUser = currentUser;
            _isCurrentUserAdmin = currentUser?.IsAdmin ?? false;
            _postService = new PostService(new ApplicationContext());


            DeleteButtonVisibility = CanDeleteRoute() ? Visibility.Visible : Visibility.Collapsed;
            DeleteCommand = new RelayCommand(async () => await DeleteRouteAsync());

            LoadImage();
        }

        private bool CanDeleteRoute()
        {
            if (_currentUser == null)
                return false;

            return _route.UserId == _currentUser.Id || _isCurrentUserAdmin;
        }

        private async Task DeleteRouteAsync()
        {
            if (_currentUser == null)
                return;

            var result = MessageBox.Show(
                "Are you sure you want to delete this route?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = await _postService.DeleteRouteAsync(
                        _route.Id,
                        _currentUser.Id,
                        _isCurrentUserAdmin
                    );

                    if (success)
                    {
                        MessageBox.Show("Route deleted successfully!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        OnRouteDeleted?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        MessageBox.Show("You don't have permission to delete this route.",
                            "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting route: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event EventHandler OnRouteDeleted;

        private void LoadImage()
        {
            try
            {
                byte[] imageData = null;

                // Пытаемся получить отредактированное изображение, если нет - оригинальное
                if (_route.Image?.EditedImageData != null && _route.Image.EditedImageData.Length > 0)
                {
                    imageData = _route.Image.EditedImageData;
                }
                else if (_route.Image?.OriginalImageData != null && _route.Image.OriginalImageData.Length > 0)
                {
                    imageData = _route.Image.OriginalImageData;
                }

                if (imageData != null)
                {
                    RouteImage = _postService.ConvertByteArrayToBitmapImage(imageData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading route image: {ex.Message}");
                RouteImage = new BitmapImage();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}