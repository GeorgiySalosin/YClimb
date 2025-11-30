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

        public string UserNickname => _post.User?.Nickname ?? "Unknown";
        public string Title => _post.Title;
        public string Content => _post.Content;
        public DateTime CreatedAt => _post.CreatedAt;

        public List<ImageViewModel> Images { get; private set; } = new List<ImageViewModel>();

        public PostViewModel(Post post)
        {
            _post = post;
            _postService = new PostService(new ApplicationContext());
            LoadImages();
        }

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
}
