using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using YClimb.Entities;
using YClimb.Utilities;
using YClimb.Windows;

namespace YClimb.Controls.Common
{
    public partial class Ctrl_CreateRoute : UserControl
    {
        public event EventHandler<RouteCreatedEventArgs>? RouteCreated;
        public event EventHandler? RouteCancelled;

        private readonly User _currentUser;
        private byte[] _selectedImageBytes;

        public Ctrl_CreateRoute(User user)
        {
            _currentUser = user;
            InitializeComponent();
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select route image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _selectedImageBytes = File.ReadAllBytes(openFileDialog.FileName);

                    var originalDimensions = ImageHelper.GetImageDimensions(_selectedImageBytes);
                    Console.WriteLine($"Original image: {originalDimensions.Width}x{originalDimensions.Height}");

                    // Preview scaling

                    var previewBytes = ImageHelper.ResizeToWidth(_selectedImageBytes, 400);
                    var previewImage = ImageHelper.CreateBitmapImage(previewBytes);

                    PreviewImage.Source = previewImage;
                    ImagePreviewBorder.Visibility = Visibility.Visible;

                    OpenImageEditor();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenImageEditor()
        {
            if (_selectedImageBytes == null)
                return;

            var editorWindow = new ImageEditorWindow(_selectedImageBytes)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (editorWindow.ShowDialog() == true)
            {
                _selectedImageBytes = editorWindow.EditedImageBytes;

                var previewImage = ImageHelper.CreateBitmapImage(_selectedImageBytes);
                PreviewImage.Source = previewImage;

            }
        }


        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter route title");
                return;
            }

            if (_selectedImageBytes == null)
            {
                MessageBox.Show("Please upload an image for the route");
                return;
            }

            try
            {
                CreateButton.IsEnabled = false;
                CreateButton.Content = "Creating...";

                
                var finalImageBytes = _selectedImageBytes;

                using (var context = new ApplicationContext())
                {
                    var postService = new PostService(context);

                    var route = await postService.CreateRouteAsync(
                        TitleTextBox.Text,
                        _currentUser.Id,
                        finalImageBytes
                    );

                    RouteCreated?.Invoke(this, new RouteCreatedEventArgs(route));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating route: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CreateButton.IsEnabled = true;
                CreateButton.Content = "Create Route";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RouteCancelled?.Invoke(this, EventArgs.Empty);
        }
    }



    public class RouteCreatedEventArgs : EventArgs
    {
        public Route Route { get; }

        public RouteCreatedEventArgs(Route route)
        {
            Route = route;
        }
    }
}