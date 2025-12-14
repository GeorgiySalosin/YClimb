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

namespace YClimb.Controls.Common
{
    /// <summary>
    /// Interaction logic for Ctrl_CreatePost.xaml
    /// </summary>
    public partial class Ctrl_CreatePost : UserControl
    {
        public event EventHandler<PostCreatedEventArgs>? PostCreated;
        public event EventHandler? PostCancelled;

        private readonly User _currentUser;
        private readonly PostService _postService;

        private List<ImageFile> _attachedImages = new List<ImageFile>();

        public Ctrl_CreatePost(User user)
        {
            _currentUser = user;
            _postService = new PostService(new ApplicationContext());

            InitializeComponent();
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                Multiselect = true,
                Title = "Select images for post"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filename in openFileDialog.FileNames)
                {
                    try
                    {
                        var imageData = File.ReadAllBytes(filename);
                        var preview = CreateBitmapImage(imageData);

                        _attachedImages.Add(new ImageFile
                        {
                            FileName = System.IO.Path.GetFileName(filename),
                            ImageData = imageData,
                            Preview = preview
                        });
                    }
                    catch (Exception ex)
                    {
                    }
                }

                RefreshImageList();
            }
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string fileName)
            {
                _attachedImages.RemoveAll(img => img.FileName == fileName);
                RefreshImageList();
            }
        }


        private void RefreshImageList()
        {
            SelectedImagesControl.ItemsSource = null;
            SelectedImagesControl.ItemsSource = _attachedImages;
        }

        private BitmapImage CreateBitmapImage(byte[] imageData)
        {
            var bitmap = new BitmapImage();
            using (var stream = new MemoryStream(imageData))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            bitmap.Freeze();
            return bitmap;
        }

        private async void CreatePostButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter a title");
                return;
            }

            if (string.IsNullOrWhiteSpace(ContentTextBox.Text) && _attachedImages.Count == 0)
            {
                MessageBox.Show("No content is attached");
                return;
            }

            try
            {
                CreatePostButton.IsEnabled = false;
                CancelButton.IsEnabled = false;
                var imageDataList = _attachedImages.Select(img => img.ImageData).ToList();

                
                var post = await _postService.CreatePostAsync(
                    TitleTextBox.Text.Trim(),
                    ContentTextBox.Text.Trim(),
                    _currentUser.Id,
                    imageDataList
                );

                
                PostCreated?.Invoke(this, new PostCreatedEventArgs(post));

                ClearForm();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                CreatePostButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            PostCancelled?.Invoke(this, EventArgs.Empty);
            ClearForm();
        }

        private void ClearForm()
        {
            TitleTextBox.Text = string.Empty;
            ContentTextBox.Text = string.Empty;
            _attachedImages.Clear();
            RefreshImageList();
        }

        
    }

    


    public class ImageFile
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = [];
        public BitmapImage Preview { get; set; } = new BitmapImage();
    }

    public class PostCreatedEventArgs : EventArgs
    {
        public Post Post { get; }

        public PostCreatedEventArgs(Post post)
        {
            Post = post;
        }
    }
}


